using Nebula.Core.Data.Image;
using Nebula.Core.Utilities;
using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace Nebula.Core.ImageTranslation
{
    public static class RGBMaskedToRGBA
    {
        public static void Translate(Stream dataStream, BaseImage baseImage, Span<byte> result)
        {
            // Store local variables
            int width = baseImage.Width;
            int height = baseImage.Height;
            bool hasAlpha = (baseImage.Flags & EImageFlags.Alpha) != 0;

            // Check the result span is the correct size
            ArgumentOutOfRangeException.ThrowIfNotEqual(result.Length, checked(width * height * 4));

            // Calculate the paddings
            var colorPadding = width * 3 % 2;
            var alphaPadding = (4 - (width % 4)) % 4;

            // Read out the bgr and alpha streams into pooled arrays
            var bgrPaddedByteCount = checked((width + colorPadding) * 3 * height);
            byte[] bgrData = ArrayPool<byte>.Shared.Rent(bgrPaddedByteCount);
            dataStream.ReadExactly(bgrData.AsSpan(0, bgrPaddedByteCount));

            // Alpha check
            int alphaPaddedByteCount = checked((width + alphaPadding) * height);
            byte[] alphaData = ArrayPool<byte>.Shared.Rent(alphaPaddedByteCount);
            if (hasAlpha)
                dataStream.ReadExactly(alphaData.AsSpan(0, alphaPaddedByteCount));

            // Remove padding in color bgr data
            var bgrByteCount = width * 3 * height;
            if (colorPadding != 0)
            {
                var strideSrc = (width + colorPadding) * 3;
                var strideDst = width * 3;
                Span<byte> src = bgrData.AsSpan(0, bgrPaddedByteCount)[strideSrc..];
                Span<byte> dst = bgrData.AsSpan(0, bgrByteCount)[strideDst..];
                while (src.Length > 0)
                {
                    src[..strideDst].CopyTo(dst[..strideDst]);
                    src = src[strideSrc..];
                    dst = dst[strideDst..];
                }
            }

            // Remove padding in alpha data
            var alphaByteCount = width * height;
            if (hasAlpha && alphaPadding != 0)
            {
                var strideSrc = width + alphaPadding;
                var strideDst = width;
                Span<byte> src = alphaData.AsSpan(0, alphaPaddedByteCount)[strideSrc..];
                Span<byte> dst = alphaData.AsSpan(0, alphaByteCount)[strideDst..];
                while (src.Length > 0)
                {
                    src[..strideDst].CopyTo(dst[..strideDst]);
                    src = src[strideSrc..];
                    dst = dst[strideDst..];
                }
            }

            // Combine the BGR and alpha data into Bgra data in our result span
            Combine(result, bgrData.AsSpan(0, bgrByteCount), alphaData.AsSpan(0, alphaByteCount));

            // Return the rented arrays we used as BGR and alpha buffers
            ArrayPool<byte>.Shared.Return(bgrData);
            ArrayPool<byte>.Shared.Return(alphaData);

            // Perform alpha premultiplication
            if (hasAlpha)
                Premultiply(result);

            // Apply Transparent Color
            else
            {
                uint transparent = (baseImage.TransparentColor & 0x0000FF00) |
                                   (baseImage.TransparentColor & 0x00FF0000) >> 16 |
                                   (baseImage.TransparentColor & 0x000000FF) << 16;

                for (int i = 0; i < width * height; i++)
                {
                    uint color = MemoryMarshal.Read<uint>(result.Slice(i * 4));
                    int idx = i * 4 + 3;
                    if (color == transparent)
                        result[idx] = 0;
                    else
                        result[idx] = 255;
                }
            }
        }

        private static void Combine(Span<byte> combinedMemory, ReadOnlySpan<byte> colorMemory, ReadOnlySpan<byte> alphaMemory)
        {
            // Debug assertion for span sizes
            Debug.Assert(alphaMemory.Length * 3 == colorMemory.Length);
            Debug.Assert(alphaMemory.Length * 4 == combinedMemory.Length);

            // Check if we can vectorise color operations
            if (colorMemory.Length >= Vector128<byte>.Count && Vector128.IsHardwareAccelerated)
            {
                // Get a reference to the first byte in our source, first byte in our destination, and last byte in our source that it is safe to do in the loop
                ref var src = ref MemoryMarshal.GetReference(colorMemory);
                ref var dst = ref MemoryMarshal.GetReference(combinedMemory);
                ref var srcEndM1V = ref Unsafe.AddByteOffset(ref src, colorMemory.Length - Unsafe.SizeOf<Vector128<byte>>()); // 1 V128 back from the "element" just past the end of the span

                // Loop whilever a whole vector's worth of data is still available
                while (Unsafe.IsAddressLessThan(ref src, ref srcEndM1V))
                {
                    // Read a vector's worth of data, then shuffle our BGRBGRBGRBGRBGRB to BGR0BGR0BGR0BGR0, then store it in our BGRA result buffer
                    var value = Vector128.LoadUnsafe(in src);
                    value = Vector128.Shuffle(value, Vector128.Create((byte)0, 1, 2, 16, 3, 4, 5, 16, 6, 7, 8, 16, 9, 10, 11, 16));
                    Vector128.StoreUnsafe(value, ref dst);

                    // Move to next 4 pixels - in src we have 3 bytes per pixel, and in dst we have 4 bytes per pixel - we do 4 pixels at a time since that is how many V128 can fit
                    src = ref Unsafe.Add(ref src, 12);
                    dst = ref Unsafe.Add(ref dst, 16);
                }

                // Last part - we haven't processed a bit at the end
                {
                    // Read the last vector's worth of values, this gives us 5 pixels worth of color in the order RBGRBGRBGRBGRBGR (the leading R is from the previous pixel, which we have already processed)
                    var value = Vector128.LoadUnsafe(in srcEndM1V);

                    // Move dst to the 5th last pixel & shuffle our BGR data into BGRA for 4 pixels - we take the first 4 pixels from the BGR data
                    dst = ref Unsafe.Add(ref MemoryMarshal.GetReference(combinedMemory), combinedMemory.Length - 20);
                    var value2 = Vector128.Shuffle(value, Vector128.Create((byte)1, 2, 3, 16, 4, 5, 6, 16, 7, 8, 9, 16, 10, 11, 12, 16));
                    Vector128.StoreUnsafe(value2, ref dst);

                    // Move dst to the 4th last pixel & shuffle our BGR data into BGRA for 4 pixels - we take the last 4 pixels from the BGR data
                    dst = ref Unsafe.AddByteOffset(ref dst, 4);
                    value2 = Vector128.Shuffle(value, Vector128.Create((byte)4, 5, 6, 16, 7, 8, 9, 16, 10, 11, 12, 16, 13, 14, 15, 16));
                    Vector128.StoreUnsafe(value2, ref dst);
                }
            }

            // Scalar fallback for color operations
            else
            {
                var srcSp = colorMemory;
                var dstSp = combinedMemory;
                for (int i = 0, j = 0; i < srcSp.Length; i += 3, j += 4)
                {
                    dstSp[j] = srcSp[i]; // B
                    dstSp[j + 1] = srcSp[i + 1]; // G
                    dstSp[j + 2] = srcSp[i + 2]; // R
                    dstSp[j + 3] = 0; // A
                }
            }

            // Check if we can vectorise alpha operations
            if (alphaMemory.Length >= Vector128<byte>.Count && Vector128.IsHardwareAccelerated)
            {
                // Get a reference to the first byte in our source, first byte in our destination, and last byte in our source that it is safe to do in the loop
                ref var src = ref MemoryMarshal.GetReference(alphaMemory);
                ref var dst = ref MemoryMarshal.GetReference(combinedMemory);
                ref var srcEndM1V = ref Unsafe.Add(ref src, alphaMemory.Length - Unsafe.SizeOf<Vector128<byte>>()); // 1 V128 back from the "element" just past the end of the span

                // Loop whilever a whole vector's worth of data is still available
                while (Unsafe.IsAddressLessThan(ref src, ref srcEndM1V))
                {
                    // Read the vector's worth of data, we get AAAAAAAAAAAAAAAA
                    var value = Vector128.LoadUnsafe(in src);

                    // Shuffle the first 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    var value2 = Vector128.LoadUnsafe(in dst);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 0, 16, 16, 16, 1, 16, 16, 16, 2, 16, 16, 16, 3));
                    Vector128.StoreUnsafe(value2, ref dst);

                    // Shuffle the next 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    value2 = Vector128.LoadUnsafe(in dst, 16);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 4, 16, 16, 16, 5, 16, 16, 16, 6, 16, 16, 16, 7));
                    Vector128.StoreUnsafe(value2, ref dst, 16);

                    // Shuffle the next 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    value2 = Vector128.LoadUnsafe(in dst, 32);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 8, 16, 16, 16, 9, 16, 16, 16, 10, 16, 16, 16, 11));
                    Vector128.StoreUnsafe(value2, ref dst, 32);

                    // Shuffle the last 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    value2 = Vector128.LoadUnsafe(in dst, 48);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 12, 16, 16, 16, 13, 16, 16, 16, 14, 16, 16, 16, 15));
                    Vector128.StoreUnsafe(value2, ref dst, 48);

                    // We have processed 16 pixels, move accordingly
                    src = ref Unsafe.Add(ref src, 16);
                    dst = ref Unsafe.Add(ref dst, 64);
                }

                // Last part - we haven't processed a bit at the end
                {
                    // There is less than a vector's worth of data available, so just move 1 vector from the end (for source, we have 16 pixels worth of data, and 4x as much bytes in dst), and read out our vector's worth of alpha data
                    // We will be 16 pixels away from the end after this, so doing our normal loop again here will work
                    var value = Vector128.LoadUnsafe(in srcEndM1V);
                    dst = ref Unsafe.Add(ref MemoryMarshal.GetReference(combinedMemory), combinedMemory.Length - 64);

                    // Shuffle the first 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    var value2 = Vector128.LoadUnsafe(in dst);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 0, 16, 16, 16, 1, 16, 16, 16, 2, 16, 16, 16, 3));
                    Vector128.StoreUnsafe(value2, ref dst);

                    // Shuffle the next 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    value2 = Vector128.LoadUnsafe(in dst, 16);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 4, 16, 16, 16, 5, 16, 16, 16, 6, 16, 16, 16, 7));
                    Vector128.StoreUnsafe(value2, ref dst, 16);

                    // Shuffle the next 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    value2 = Vector128.LoadUnsafe(in dst, 32);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 8, 16, 16, 16, 9, 16, 16, 16, 10, 16, 16, 16, 11));
                    Vector128.StoreUnsafe(value2, ref dst, 32);

                    // Shuffle the last 4 pixels worth of data to 000A000A000A000A, then OR with the existing values for the pixels, giving our desired BGRABGRABGRABGRA
                    value2 = Vector128.LoadUnsafe(in dst, 48);
                    value2 |= Vector128.Shuffle(value, Vector128.Create((byte)16, 16, 16, 12, 16, 16, 16, 13, 16, 16, 16, 14, 16, 16, 16, 15));
                    Vector128.StoreUnsafe(value2, ref dst, 48);
                }
            }

            // Scalar fallback for alpha operations
            else
            {
                var srcSp = alphaMemory;
                var dstSp = combinedMemory;
                for (int i = 0; i < srcSp.Length; i++) dstSp[i * 4 + 3] = srcSp[i]; // A
            }
        }

        // Fixed point division constants for dividing by 255 with ushort
        const int fixedPointDivision255Factor = 0x8081;
        const int fixedPointDivision255Shift = 0x17;

        // Calculates (ushort)(((uint)x * y) >> 16) element-wise
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<ushort> MultiplyHigh(Vector128<ushort> left, Vector128<ushort> right)
        {
            // Debug check for hardware intrinsics
            Debug.Assert(Sse2.IsSupported || AdvSimd.Arm64.IsSupported);

            // Call to direct Sse2 intrinsic if on x86
            if (Sse2.IsSupported) return Sse2.MultiplyHigh(left, right);

            // Do in 2 halves on arm & then re-combine
            var resultLow = AdvSimd.MultiplyWideningLower(left.GetLower(), right.GetLower());
            var resultHigh = AdvSimd.MultiplyWideningUpper(left, right);
            return AdvSimd.Arm64.UnzipOdd(resultLow.AsUInt16(), resultHigh.AsUInt16());
        }

        // Calculates (byte)(((ushort)x * y) / 255) element-wise
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<byte> MultiplyWideningThenDivideBy255(Vector128<byte> left, Vector128<byte> right)
        {
            // Debug check for hardware intrinsics
            Debug.Assert(Sse2.IsSupported || AdvSimd.Arm64.IsSupported);

            // Arm implementation
            if (AdvSimd.Arm64.IsSupported)
            {
                // Multiply the low & high halves
                var resultLow = AdvSimd.MultiplyWideningLower(left.GetLower(), right.GetLower());
                var resultHigh = AdvSimd.MultiplyWideningUpper(left, right);

                // Perform the fixed point division on these halves
                resultLow = MultiplyHigh(resultLow, Vector128.Create((ushort)fixedPointDivision255Factor));
                resultHigh = MultiplyHigh(resultHigh, Vector128.Create((ushort)fixedPointDivision255Factor));
                resultLow >>= fixedPointDivision255Shift & ~0x10; // we get the 0x10 from multiply high instead of normal multiply
                resultHigh >>= fixedPointDivision255Shift & ~0x10; // we get the 0x10 from multiply high instead of normal multiply

                // Take the low byte from each of these & re-combine them
                return AdvSimd.Arm64.UnzipEven(resultLow.AsByte(), resultHigh.AsByte());
            }

            // x86 implementation
            else
            {
                // Put into 2 halves, we do this like so: B0D0F0H0 and A0C0E0G0
                var left1 = Vector128.Shuffle(left, Vector128.Create((byte)1, 16, 3, 16, 5, 16, 7, 16, 9, 16, 11, 16, 13, 16, 15, 16));
                var right1 = Vector128.Shuffle(right, Vector128.Create((byte)1, 16, 3, 16, 5, 16, 7, 16, 9, 16, 11, 16, 13, 16, 15, 16));
                var left2 = left & Vector128.Create(-1, 0, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0).AsByte();
                var right2 = right & Vector128.Create(-1, 0, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0).AsByte();

                // Multiply the 2 halves & perform fixed point division on them
                var result1 = left1.AsUInt16() * right1.AsUInt16();
                var result2 = left2.AsUInt16() * right2.AsUInt16();
                result1 = MultiplyHigh(result1, Vector128.Create((ushort)fixedPointDivision255Factor));
                result2 = MultiplyHigh(result2, Vector128.Create((ushort)fixedPointDivision255Factor));
                result1 >>= fixedPointDivision255Shift & ~0x10; // we get the 0x10 from multiply high instead of normal multiply
                result2 >>= fixedPointDivision255Shift & ~0x10; // we get the 0x10 from multiply high instead of normal multiply

                // Re-combine them
                return Vector128.Shuffle(result1.AsByte(), Vector128.Create((byte)16, 0, 16, 2, 16, 4, 16, 6, 16, 8, 16, 10, 16, 12, 16, 14)) | result2.AsByte();
            }
        }

        // Pre-multiply alpha helper
        private static void Premultiply(Span<byte> data)
        {
            // Debug assertion for span size
            Debug.Assert(data.Length % 4 == 0);

            // Check if vectorisation is possible
            if (data.Length >= Vector128<byte>.Count && (Sse2.IsSupported || AdvSimd.Arm64.IsSupported))
            {
                // Get a reference to the first byte in our source, and last byte in our source that it is safe to do in the loop
                ref var src = ref MemoryMarshal.GetReference(data);
                ref var srcEndM1V = ref Unsafe.Add(ref src, data.Length - Unsafe.SizeOf<Vector128<byte>>()); // 1 V128 back from the "element" just past the end of the span

                // Loop whilever a whole vector's worth of data is still available
                while (Unsafe.IsAddressLessThan(ref src, ref srcEndM1V))
                {
                    // Read the vector's worth of data, we get BGRABGRABGRABGRA
                    var value = Vector128.LoadUnsafe(in src);

                    // Extract the alpha values so that we get AAAAAAAAAAAAAAAA (repeat A of pixel to every position in that pixel)
                    var alpha = Vector128.Shuffle(value, Vector128.Create((byte)3, 3, 3, 3, 7, 7, 7, 7, 11, 11, 11, 11, 15, 15, 15, 15));

                    // Premultiply all the values of that pixel by the alpha (including the alpha, which we don't want to do)
                    var premultiplied = MultiplyWideningThenDivideBy255(value, alpha);

                    // Select just the premultiplied BGR, and keep the original A values
                    value = Vector128.ConditionalSelect(Vector128.Create(-1, -1, -1, 0, -1, -1, -1, 0, -1, -1, -1, 0, -1, -1, -1, 0).AsByte(), premultiplied, value);

                    // Write the vector back & move to next vector
                    Vector128.StoreUnsafe(value, ref src);
                    src = ref Unsafe.Add(ref src, 16);
                }

                // Last part - we haven't processed a bit at the end
                {
                    // Repeat the same steps as in our loop for the last vector
                    var value = Vector128.LoadUnsafe(in srcEndM1V);
                    var alpha = Vector128.Shuffle(value, Vector128.Create((byte)3, 3, 3, 3, 7, 7, 7, 7, 11, 11, 11, 11, 15, 15, 15, 15));
                    var premultiplied = MultiplyWideningThenDivideBy255(value, alpha);

                    // We don't want to re-premultiply any vector we have already done - we have done all the values up to (excluding) src,
                    // and we are doing from srcEndM1V onwards here; so we keep, from our existing value, those with index less than &src - &srcEndM1V;
                    // we write this as index + 1 > &src - &srcEndM1V, which gives 0 for keep original and 'all bits set' (-1) for replace.
                    var mask = Vector128.GreaterThan(Vector128.Create((byte)1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16), Vector128.Create((byte)Unsafe.ByteOffset(ref srcEndM1V, ref src)));
                    value = Vector128.ConditionalSelect(Vector128.Create(-1, -1, -1, 0, -1, -1, -1, 0, -1, -1, -1, 0, -1, -1, -1, 0).AsByte() & mask, premultiplied, value);
                    Vector128.StoreUnsafe(value, ref srcEndM1V);
                }
            }

            // Scalar fallback
            else
            {
                // Loop through each pixel & pre-multiply it
                var max = data.Length & ~3;
                for (int i = 0; i < max; i += 4)
                {
                    var a = data[i + 3];
                    data[i] = (byte)((data[i] * a * fixedPointDivision255Factor) >>> fixedPointDivision255Shift);
                    data[i + 1] = (byte)((data[i + 1] * a * fixedPointDivision255Factor) >>> fixedPointDivision255Shift);
                    data[i + 2] = (byte)((data[i + 2] * a * fixedPointDivision255Factor) >>> fixedPointDivision255Shift);
                }
            }
        }
    }
}
