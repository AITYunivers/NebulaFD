using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;
using System;
using System.Drawing;

namespace Nebula.Core.Utilities
{
    public static class ImageTranslatorGPU
    {
        static Context? _context = null;
        static Accelerator? _accelerator = null;

        public static Accelerator GetAccelerator()
        {
            if (_context == null)
            {
                _context = Context.Create(builder => builder.Cuda().OpenCL());
                if (_context.GetCudaDevices().Count > 0)
                    _accelerator = _context.CreateCudaAccelerator(0);
                else if (_context.GetCLDevices().Count > 0)
                    _accelerator = _context.CreateCLAccelerator(0);
                else
                    throw new CapabilityNotSupportedException("GPU Acceleration not supported on your hardware");
            }
            while (_accelerator == null) {}
            return _accelerator!;
        }

        public static int GetPadding(int width, int pointSize, int bytes = 2, bool modular = false)
        {
            if (modular) return (bytes - width * pointSize % bytes) % bytes;

            var pad = bytes - (width * pointSize % bytes);
            if (pad == bytes) return 0;

            return (int)Math.Ceiling(pad / (float)pointSize);
        }

        public static byte[] Normal24BitMaskedToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent, bool RLE, bool flipRGB = false)
        {
            throw new NotImplementedException();
        }
        public static byte[] Normal15BitToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent, bool RLE)
        {
            throw new NotImplementedException();
        }
        public static byte[] Normal8BitToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            throw new NotImplementedException();
        }
        public static byte[] AndroidMode0ToRGBA(byte[] imageData, int width, int height)
        {
            throw new NotImplementedException();
        }
        public static byte[] AndroidMode1ToRGBA(byte[] imageData, int width, int height)
        {
            throw new NotImplementedException();
        }
        public static byte[] AndroidMode2ToRGBA(byte[] imageData, int width, int height)
        {
            throw new NotImplementedException();
        }
        public static byte[] AndroidMode3ToRGBA(byte[] imageData, int width, int height)
        {
            throw new NotImplementedException();
        }
        public static byte[] AndroidMode4ToRGBA(byte[] imageData, int width, int height)
        {
            throw new NotImplementedException();
        }
        public static byte[] AndroidMode5ToRGBA(byte[] imageData, int width, int height, bool RLE)
        {
            throw new NotImplementedException();
        }

        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, int, int, int, ArrayView<byte>>? _twoFivePlusToRGBAKernel = null;
        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, int, int, int, ArrayView<byte>> GetTwoFivePlusToRGBAKernel(Accelerator accelerator)
        {
            if (_twoFivePlusToRGBAKernel == null)
            {
                _twoFivePlusToRGBAKernel = accelerator.LoadAutoGroupedStreamKernel(
                (Index1D i,
                ArrayView<byte> data,
                int width,
                int height,
                int pad,
                int alphaPad,
                int flipRGB,
                int alpha,
                int rgba,
                int premultiplied,
                int transparent,
                ArrayView<byte> output) =>
                {
                    int x = i % width;
                    int y = (int)i / width;
                    int position = (x * 4) + (y * width * 4) + (y * (pad * 4));

                    int newPos = (y * (width * 4)) + (x * 4);
                    if (flipRGB == 1)
                    {
                        output[newPos + 0] = data[position + 2];
                        output[newPos + 1] = data[position + 1];
                        output[newPos + 2] = data[position + 0];
                    }
                    else
                    {
                        output[newPos + 0] = data[position + 0];
                        output[newPos + 1] = data[position + 1];
                        output[newPos + 2] = data[position + 2];
                    }
                    output[newPos + 3] = 255;
                    if (alpha == 1)
                    {
                        if (premultiplied == 1)
                        {
                            float a = data[position + 3] / 255f;
                            output[newPos + 0] = (byte)(output[newPos + 0] / a);
                            output[newPos + 1] = (byte)(output[newPos + 1] / a);
                            output[newPos + 2] = (byte)(output[newPos + 2] / a);
                        }
                        output[newPos + 3] = data[position + 3];
                    }
                    else
                    {
                        if (data[newPos + 2] == ((transparent >> 16) & 0xFF) &&
                            data[newPos + 1] == ((transparent >> 8) & 0xFF) &&
                            data[newPos + 0] == (transparent & 0xFF))
                            output[newPos + 3] = 0;
                    }

                    if (alpha == 1 && rgba == 0)
                    {
                        position = (width * height) + x + (y * width) + (y * alphaPad);
                        if (position >= output.Length || newPos + 3 >= data.Length)
                            return;
                        output[newPos + 3] = data[position];
                    }
                });
            }
            return _twoFivePlusToRGBAKernel;
        }

        public static byte[] TwoFivePlusToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent, bool RGBA, bool flipRGB = false)
        {
            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(imageData);
            var deviceOutput = accelerator.Allocate1D(new byte[width * height * 4]);
            var loadedKernel = GetTwoFivePlusToRGBAKernel(accelerator);

            loadedKernel(
                width * height,
                deviceData.View, 
                width, 
                height, 
                GetPadding(width, 4),
                GetPadding(width, 1, 4),
                flipRGB ? 1 : 0,
                (alpha || RGBA) ? 1 : 0,
                alpha ? 1 : 0,
                NebulaCore.PackageData.ExtendedHeader.Flags["PremultipliedAlpha"] ? 1 : 0,
                (transparent.R << 16) | (transparent.G << 8) | transparent.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = deviceOutput.GetAsArray1D();
            deviceData.Dispose();
            deviceOutput.Dispose();
            return output;
        }
        public static byte[] FlashToRGBA(byte[] imageData, int width, int height)
        {
            throw new NotImplementedException();
        }

        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, int, ArrayView<byte>>? _rgbaToRGBMaskedKernel = null;
        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, int, ArrayView<byte>> GetRGBAToRGBMaskedKernel(Accelerator accelerator)
        {
            if (_rgbaToRGBMaskedKernel == null)
            {
                _rgbaToRGBMaskedKernel = accelerator.LoadAutoGroupedStreamKernel(
                (Index1D i,
                ArrayView<byte> data,
                int width,
                int height,
                int pad,
                int alphaPad,
                int alpha,
                int RGBA,
                int transparent,
                ArrayView<byte> output) =>
                {
                    int x = i % width;
                    int y = (int)i / width % height;
                    if (i < width * height)
                    {
                        int position = (x * 3) + (y * width * 4) + (y * (pad * 3));

                        int pos = (y * (width * 4)) + (x * 4);
                        output[position + 0] = data[pos + 0];
                        output[position + 1] = data[pos + 1];
                        output[position + 2] = data[pos + 2];
                        output[position + 3] = data[pos + 3];

                        if (alpha == 0 && RGBA == 1 && data[pos + 3] != 255)
                        {
                            output[position + 2] = (byte)((transparent >> 16) & 0xFF);
                            output[position + 1] = (byte)((transparent >> 8) & 0xFF);
                            output[position + 0] = (byte)(transparent & 0xFF);
                        }
                    }
                    else if (alpha == 1)
                    {
                        int position = (width * height) + x + (y * width) + (y * alphaPad);
                        int pos = (y * (width * 4)) + (x * 4);
                        if (position >= output.Length || pos + 3 >= data.Length)
                            return;
                        output[position] = data[pos + 3];
                        position += 1;
                    }
                });
            }
            return _rgbaToRGBMaskedKernel;
        }

        public static byte[] RGBAToRGBMasked(byte[] imageData, int width, int height, bool alpha, bool RGBA = false) => RGBAToRGBMasked(imageData, width, height, alpha, Color.Black, RGBA);
        public static byte[] RGBAToRGBMasked(byte[] imageData, int width, int height, bool alpha, Color transparent, bool RGBA = false)
        {
            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(imageData);
            var deviceOutput = accelerator.Allocate1D(new byte[width * height * 8]);
            var loadedKernel = GetRGBAToRGBMaskedKernel(accelerator);

            loadedKernel(
                width * height * 2,
                deviceData.View,
                width,
                height,
                GetPadding(width, 3),
                GetPadding(width, 1, 4),
                alpha ? 1 : 0,
                RGBA ? 1 : 0,
                (transparent.R << 16) | (transparent.G << 8) | transparent.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = deviceOutput.GetAsArray1D();
            deviceData.Dispose();
            deviceOutput.Dispose();
            return output;
        }
        public static byte[] ColorPaletteToRGBA(byte[] imageData, int width, int height, List<Color> palette, Color transparent, bool RLE)
        {
            throw new NotImplementedException();
        }
    }
}
