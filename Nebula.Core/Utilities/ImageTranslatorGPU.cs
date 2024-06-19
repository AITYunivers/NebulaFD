using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;
using System;
using System.Diagnostics;
using System.Drawing;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;

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
        public static int GetPadding(Image img)
        {
            int colorModeSize = 3;
            switch (img.GraphicMode)
            {
                case 1:
                case 8:
                    colorModeSize = 4;
                    break;
                case 4:
                case 5:
                    colorModeSize = 3;
                    break;
                case 6:
                case 7:
                    colorModeSize = 2;
                    break;
                case 2:
                case 3:
                    colorModeSize = 1;
                    break;
                default:
                    colorModeSize = 3;
                    break;
            }

            if (!img.Flags["RLET"] || NebulaCore.Plus || NebulaCore.Fusion >= 2.0f)
                return img.Width * colorModeSize % 2;
            else if (NebulaCore.Android)
                return img.Width * colorModeSize;
            else if (NebulaCore.Build < 280)
                return img.Width * colorModeSize % 2 * colorModeSize;
            else
                return img.Width % 2 * colorModeSize;
        }

        public static int GetAlphaPadding(Image img)
        {
            if (NebulaCore.Android)
                return 0;
            else
                return (4 - (img.Width % 4)) % 4;
        }

        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, int, ArrayView<byte>>? _normal24BitMaskedToRGBAKernel = null;
        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, int, ArrayView<byte>> GetNormal24BitMaskedToRGBAKernel(Accelerator accelerator)
        {
            if (_normal24BitMaskedToRGBAKernel == null)
            {
                _normal24BitMaskedToRGBAKernel = accelerator.LoadAutoGroupedStreamKernel(
                (Index1D i,
                ArrayView<byte> data,
                int width,
                int height,
                int pad,
                int alphaPad,
                int flipRGB,
                int alpha,
                int transparent,
                ArrayView<byte> output) =>
                {
                    int x = i % width;
                    int y = (int)i / width;
                    int position = (x * 3) + (y * (width * 3)) + (y * (pad * 3));

                    byte r = data[position + 0];
                    byte g = data[position + 1];
                    byte b = data[position + 2];

                    if (flipRGB == 1)
                    {
                        r = data[position + 2];
                        b = data[position + 0];
                    }

                    int newPos = (y * (width * 4)) + (x * 4);
                    output[newPos + 0] = r;
                    output[newPos + 1] = g;
                    output[newPos + 2] = b;
                    output[newPos + 3] = 255;
                    if (alpha != 1)
                    {
                        if (b == ((transparent >> 16) & 0xFF) &&
                            g == ((transparent >> 8) & 0xFF) &&
                            r == (transparent & 0xFF))
                            output[newPos + 3] = 0;
                    }
                    else
                    {
                        position = (width * height * 3) + x + (y * width) + (y * alphaPad);
                        if (position >= data.Length || newPos + 3 >= output.Length)
                            return;
                        output[newPos + 3] = data[position];
                    }
                });
            }
            return _normal24BitMaskedToRGBAKernel;
        }

        public static byte[] Normal24BitMaskedToRGBA(Image img)
        {
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                return ImageTranslatorCPU.Normal24BitMaskedToRGBA(img);

            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(img.ImageData);
            var deviceOutput = accelerator.Allocate1D(new byte[img.Width * img.Height * 4]);
            var loadedKernel = GetNormal24BitMaskedToRGBAKernel(accelerator);

            loadedKernel(
                img.Width * img.Height,
                deviceData.View,
                img.Width,
                img.Height,
                GetPadding(img),
                GetAlphaPadding(img),
                NebulaCore.Seeded ? 1 : 0,
                img.Flags["Alpha"] ? 1 : 0,
                (img.TransparentColor.R << 16) | (img.TransparentColor.G << 8) | img.TransparentColor.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = new byte[img.Width * img.Height * 4];
            deviceOutput.CopyToCPU(output);
            deviceData.Dispose();
            deviceOutput.Dispose();
            return output;
        }

        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, ArrayView<byte>>? _normal16BitToRGBAKernel = null;
        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, ArrayView<byte>> GetNormal16BitToRGBAKernel(Accelerator accelerator)
        {
            if (_normal16BitToRGBAKernel == null)
            {
                _normal16BitToRGBAKernel = accelerator.LoadAutoGroupedStreamKernel(
                (Index1D i,
                ArrayView<byte> data,
                int width,
                int height,
                int pad,
                int alphaPad,
                int alpha,
                int transparent,
                ArrayView<byte> output) =>
                {
                    int x = i % width;
                    int y = (int)i / width;
                    int position = (x * 2) + (y * (width * 2)) + (y * (pad * 2));

                    ushort newShort = (ushort)(data[position] | data[position + 1] << 8);
                    byte r = (byte)(((newShort & 63488) >> 11) << 3);
                    byte g = (byte)(((newShort & 2016) >> 5) << 2);
                    byte b = (byte)((newShort & 31) << 3);

                    int newPos = (y * (width * 4)) + (x * 4);
                    output[newPos + 2] = r;
                    output[newPos + 1] = g;
                    output[newPos + 0] = b;
                    output[newPos + 3] = 255;
                    if (alpha != 1)
                    {
                        if (r == ((transparent >> 16) & 0xFF) &&
                            g == ((transparent >> 8) & 0xFF) &&
                            b == (transparent & 0xFF))
                            output[newPos + 3] = 0;
                    }
                    else
                    {
                        position = (width * height * 2) + x + (y * width) + (y * alphaPad);
                        if (position >= data.Length || newPos + 3 >= output.Length)
                            return;
                        output[newPos + 3] = data[position];
                    }
                });
            }
            return _normal16BitToRGBAKernel;
        }

        public static byte[] Normal16BitToRGBA(Image img)
        {
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                return ImageTranslatorCPU.Normal16BitToRGBA(img);

            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(img.ImageData);
            var deviceOutput = accelerator.Allocate1D(new byte[img.Width * img.Height * 4]);
            var loadedKernel = GetNormal16BitToRGBAKernel(accelerator);

            loadedKernel(
                img.Width * img.Height,
                deviceData.View,
                img.Width,
                img.Height,
                GetPadding(img),
                GetAlphaPadding(img),
                img.Flags["Alpha"] ? 1 : 0,
                (img.TransparentColor.R << 16) | (img.TransparentColor.G << 8) | img.TransparentColor.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = new byte[img.Width * img.Height * 4];
            deviceOutput.CopyToCPU(output);
            deviceData.Dispose();
            deviceOutput.Dispose();
            return output;
        }

        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, ArrayView<byte>>? _normal15BitToRGBAKernel = null;
        private static Action<Index1D, ArrayView<byte>, int, int, int, int, int, int, ArrayView<byte>> GetNormal15BitToRGBAKernel(Accelerator accelerator)
        {
            if (_normal15BitToRGBAKernel == null)
            {
                _normal15BitToRGBAKernel = accelerator.LoadAutoGroupedStreamKernel(
                (Index1D i,
                ArrayView<byte> data,
                int width,
                int height,
                int pad,
                int alphaPad,
                int alpha,
                int transparent,
                ArrayView<byte> output) =>
                {
                    int x = i % width;
                    int y = (int)i / width;
                    int position = (x * 2) + (y * (width * 2)) + (y * (pad * 2));

                    ushort newShort = (ushort)(data[position] | data[position + 1] << 8);
                    byte r = (byte)(((newShort & 31744) >> 10) << 3);
                    byte g = (byte)(((newShort & 992) >> 5) << 3);
                    byte b = (byte)((newShort & 31) << 3);

                    int newPos = (y * (width * 4)) + (x * 4);
                    output[newPos + 2] = r;
                    output[newPos + 1] = g;
                    output[newPos + 0] = b;
                    output[newPos + 3] = 255;
                    if (alpha != 1)
                    {
                        if (r == ((transparent >> 16) & 0xFF) &&
                            g == ((transparent >> 8) & 0xFF) &&
                            b == (transparent & 0xFF))
                            output[newPos + 3] = 0;
                    }
                    else
                    {
                        position = (width * height * 2) + x + (y * width) + (y * alphaPad);
                        if (position >= data.Length || newPos + 3 >= output.Length)
                            return;
                        output[newPos + 3] = data[position];
                    }
                });
            }
            return _normal15BitToRGBAKernel;
        }

        public static byte[] Normal15BitToRGBA(Image img)
        {
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                return ImageTranslatorCPU.Normal15BitToRGBA(img);

            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(img.ImageData);
            var deviceOutput = accelerator.Allocate1D(new byte[img.Width * img.Height * 4]);
            var loadedKernel = GetNormal15BitToRGBAKernel(accelerator);

            loadedKernel(
                img.Width * img.Height,
                deviceData.View,
                img.Width,
                img.Height,
                GetPadding(img),
                GetAlphaPadding(img),
                img.Flags["Alpha"] ? 1 : 0,
                (img.TransparentColor.R << 16) | (img.TransparentColor.G << 8) | img.TransparentColor.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = new byte[img.Width * img.Height * 4];
            deviceOutput.CopyToCPU(output);
            deviceData.Dispose();
            deviceOutput.Dispose();
            return output;
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
                        position = (width * height * 4) + x + (y * width) + (y * alphaPad);
                        if (position >= data.Length || newPos + 3 >= output.Length)
                            return;
                        output[newPos + 3] = data[position];
                    }
                });
            }
            return _twoFivePlusToRGBAKernel;
        }

        public static byte[] TwoFivePlusToRGBA(Image img)
        {
            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(img.ImageData);
            var deviceOutput = accelerator.Allocate1D(new byte[img.Width * img.Height * 4]);
            var loadedKernel = GetTwoFivePlusToRGBAKernel(accelerator);

            loadedKernel(
                img.Width * img.Height,
                deviceData.View, 
                img.Width, 
                img.Height, 
                GetPadding(img),
                GetAlphaPadding(img),
                NebulaCore.Seeded ? 1 : 0,
                (img.Flags["Alpha"] || img.Flags["RGBA"]) ? 1 : 0,
                img.Flags["RGBA"] ? 1 : 0,
                NebulaCore.PackageData.ExtendedHeader.Flags["PremultipliedAlpha"] ? 1 : 0,
                (img.TransparentColor.R << 16) | (img.TransparentColor.G << 8) | img.TransparentColor.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = new byte[img.Width * img.Height * 4];
            deviceOutput.CopyToCPU(output);
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
                    int y = (int)i / width;
                    int position = (x * 3) + (y * width * 3) + (y * (pad * 3));

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
                    
                    if (alpha == 1)
                    {
                        position = (width * height * 3) + x + (y * width) + (y * alphaPad);
                        if (position >= output.Length || pos + 3 >= data.Length)
                            return;
                        output[position] = data[pos + 3];
                    }
                });
            }
            return _rgbaToRGBMaskedKernel;
        }

        public static byte[] RGBAToRGBMasked(Image img)
        {
            Accelerator accelerator = GetAccelerator();
            var deviceData = accelerator.Allocate1D(img.ImageData);
            var deviceOutput = accelerator.Allocate1D(new byte[img.Width * img.Height * 4]);
            var loadedKernel = GetRGBAToRGBMaskedKernel(accelerator);

            loadedKernel(
                img.Width * img.Height,
                deviceData.View,
                img.Width,
                img.Height,
                GetPadding(img),
                GetAlphaPadding(img),
                img.Flags["Alpha"] ? 1 : 0,
                img.Flags["RGBA"] ? 1 : 0,
                (img.TransparentColor.R << 16) | (img.TransparentColor.G << 8) | img.TransparentColor.B,
                deviceOutput.View);

            accelerator.Synchronize();

            byte[] output = new byte[img.Width * img.Height * 4];
            deviceOutput.CopyToCPU(output);
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
