using System.Drawing;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;

namespace Nebula.Core.Utilities
{
    public static class ImageTranslatorCPU
    {
        public static int GetPadding(Image img)
        {
            int colorModeSize = 3;
            switch (img.GraphicMode)
            {
                case 0:
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

            if (!img.Flags["RLET"] || NebulaCore.Plus || NebulaCore.Fusion < 2.0f)
                return img.Width * colorModeSize % 2;
            else if (NebulaCore.Android || NebulaCore.iOS)
                return img.Width * colorModeSize;
            else if (NebulaCore.Build < 280)
                return img.Width * colorModeSize % 2 * colorModeSize;
            else
                return img.Width % 2 * colorModeSize;
        }

        public static int GetAlphaPadding(Image img)
        {
            if (NebulaCore.Android || NebulaCore.iOS)
                return 0;
            else
                return (4 - (img.Width % 4)) % 4;
        }

        public static byte[] Normal24BitMaskedToRGBA(Image img)
        {
            byte[] colorArray = new byte[img.Width * img.Height * 4];
            int stride = img.Width * 4;
            int pad = GetPadding(img);
            int position = 0;
            int command = img.ImageData[position];
            bool rleLoop = false;
            bool rleCommander = false;
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                position++;

            byte r = 0;
            byte g = 0;
            byte b = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (!rle || !rleLoop || rleCommander)
                    {
                        r = img.ImageData[position++];
                        g = img.ImageData[position++];
                        b = img.ImageData[position++];
                        rleLoop = true;
                    }

                    int newPos = (y * stride) + (x * 4);
                    if (NebulaCore.Fusion == 3.0f && !NebulaCore.Seeded)
                    {
                        colorArray[newPos + 0] = b;
                        colorArray[newPos + 1] = g;
                        colorArray[newPos + 2] = r;
                    }
                    else
                    {
                        colorArray[newPos + 0] = r;
                        colorArray[newPos + 1] = g;
                        colorArray[newPos + 2] = b;
                    }
                    colorArray[newPos + 3] = 255;
                    if (!img.Flags["Alpha"])
                    {
                        if (colorArray[newPos + 0] == img.TransparentColor.B && 
                            colorArray[newPos + 1] == img.TransparentColor.G &&
                            colorArray[newPos + 2] == img.TransparentColor.R)
                            colorArray[newPos + 3] = 0;
                    }

                    if (rle && --command == 0)
                    {
                        command = img.ImageData[position++];
                        rleCommander = false;
                        rleLoop = false;

                        if (command > 128)
                        {
                            command -= 128;
                            rleCommander = true;
                        }
                        else if (command == 0)
                            rleLoop = true;
                    }
                }

                position += pad * 3;
            }

            if (img.Flags["Alpha"])
            {
                int aPad = GetAlphaPadding(img);
                int aStride = img.Width * 4;
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = img.ImageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;
        }
        public static byte[] Normal16BitToRGBA(Image img)
        {
            byte[] colorArray = new byte[img.Width * img.Height * 4];
            int stride = img.Width * 4;
            int pad = GetPadding(img);
            int position = 0;
            int command = img.ImageData[position];
            bool rleLoop = false;
            bool rleCommander = false;
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                position++;

            byte r = 0;
            byte g = 0;
            byte b = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (!rle || !rleLoop || rleCommander)
                    {
                        ushort newShort = (ushort)(img.ImageData[position++] | img.ImageData[position++] << 8);
                        r = (byte)((newShort & 63488) >> 11);
                        g = (byte)((newShort & 2016) >> 5);
                        b = (byte)((newShort & 31));

                        r = (byte)(r << 3);
                        g = (byte)(g << 2);
                        b = (byte)(b << 3);
                        rleLoop = true;
                    }

                    int newPos = (y * stride) + (x * 4);
                    colorArray[newPos + 2] = r;
                    colorArray[newPos + 1] = g;
                    colorArray[newPos + 0] = b;
                    colorArray[newPos + 3] = 255;
                    if (!img.Flags["Alpha"])
                    {
                        if (colorArray[newPos + 2] == img.TransparentColor.R && 
                            colorArray[newPos + 1] == img.TransparentColor.G &&
                            colorArray[newPos + 0] == img.TransparentColor.B)
                            colorArray[newPos + 3] = 0;
                    }

                    if (rle && --command == 0)
                    {
                        command = img.ImageData[position++];
                        rleCommander = false;
                        rleLoop = false;

                        if (command > 128)
                        {
                            command -= 128;
                            rleCommander = true;
                        }
                        else if (command == 0)
                            rleLoop = true;
                    }
                }

                position += pad * 2;
            }
            if (img.Flags["Alpha"])
            {
                int aPad = GetAlphaPadding(img);
                int aStride = img.Width * 4;
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = img.ImageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;
        }

        public static byte[] Normal15BitToRGBA(Image img)
        {
            byte[] colorArray = new byte[img.Width * img.Height * 4];
            int stride = img.Width * 4;
            int pad = GetPadding(img);
            int position = 0;
            int command = img.ImageData[position];
            bool rleLoop = false;
            bool rleCommander = false;
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                position++;
            byte r = 0;
            byte g = 0;
            byte b = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (!rle || !rleLoop || rleCommander)
                    {
                        ushort newShort = (ushort)(img.ImageData[position++] | img.ImageData[position++] << 8);
                        r = (byte)((newShort & 31744) >> 10);
                        g = (byte)((newShort & 992) >> 5);
                        b = (byte)((newShort & 31));

                        r = (byte)(r << 3);
                        g = (byte)(g << 3);
                        b = (byte)(b << 3);
                        rleLoop = true;
                    }

                    int newPos = (y * stride) + (x * 4);
                    colorArray[newPos + 2] = r;
                    colorArray[newPos + 1] = g;
                    colorArray[newPos + 0] = b;
                    colorArray[newPos + 3] = 255;
                    if (!img.Flags["Alpha"])
                    {
                        if (colorArray[newPos + 2] == img.TransparentColor.R && 
                            colorArray[newPos + 1] == img.TransparentColor.G &&
                            colorArray[newPos + 0] == img.TransparentColor.B)
                            colorArray[newPos + 3] = 0;
                    }

                    if (rle && --command == 0)
                    {
                        command = img.ImageData[position++];
                        rleCommander = false;
                        rleLoop = false;

                        if (command > 128)
                        {
                            command -= 128;
                            rleCommander = true;
                        }
                        else if (command == 0)
                            rleLoop = true;
                    }
                }

                position += pad;
            }
            if (img.Flags["Alpha"])
            {
                int aPad = GetAlphaPadding(img);
                int aStride = img.Width * 4;
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = img.ImageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;

        }

        public static byte[] AndroidMode0ToRGBA(Image img)
        {
            var colorArray = new byte[img.Width * img.Height * 4];
            var stride = img.Width * 4;
            var position = 0;
            var pad = GetPadding(img);
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    colorArray[y * stride + x * 4 + 2] = img.ImageData[position];
                    colorArray[y * stride + x * 4 + 1] = img.ImageData[position + 1];
                    colorArray[y * stride + x * 4 + 0] = img.ImageData[position + 2];
                    colorArray[y * stride + x * 4 + 3] = img.ImageData[position + 3];
                    position += 4;
                }

                position += pad * 3;
            }

            return colorArray;
        }
        public static byte[] AndroidMode1ToRGBA(Image img)
        {
            var colorArray = new byte[img.Width * img.Height * 4];
            var stride = img.Width * 4;
            var position = 0;
            var pad = GetPadding(img);
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var newShort = (ushort)(img.ImageData[position] | (img.ImageData[position + 1] << 8));

                    var a = (byte)((newShort & 0xf) >> 0);
                    var r = (byte)((newShort & 0xF000) >> 12);
                    var g = (byte)((newShort & 0xF00) >> 8);
                    var b = (byte)((newShort & 0xf0) >> 4);

                    r = (byte)(r << 4);
                    g = (byte)(g << 4);
                    b = (byte)(b << 4);
                    a = (byte)(a << 4);
                    //r done
                    //g partially done

                    colorArray[y * stride + x * 4 + 2] = r;
                    colorArray[y * stride + x * 4 + 1] = g;
                    colorArray[y * stride + x * 4 + 0] = b;
                    colorArray[y * stride + x * 4 + 3] = a;

                    position += 2;
                }

                position += pad * 2;
            }

            return colorArray;
        }
        public static byte[] AndroidMode2ToRGBA(Image img)
        {
            var colorArray = new byte[img.Width * img.Height * 4];
            var stride = img.Width * 4;
            var position = 0;
            var pad = GetPadding(img);
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var newShort = (ushort)(img.ImageData[position] | (img.ImageData[position + 1] << 8));

                    var a = (byte)((newShort & 0xf) >> 0);
                    var r = (byte)((newShort & 0xf800) >> 11);
                    var g = (byte)((newShort & 0x7c0) >> 6);
                    var b = (byte)((newShort & 0x3e) >> 1);

                    r = (byte)(r << 3);
                    g = (byte)(g << 3);
                    b = (byte)(b << 3);
                    a = (byte)(a << 4);
                    //r done
                    //g partially done

                    colorArray[y * stride + x * 4 + 2] = r;
                    colorArray[y * stride + x * 4 + 1] = g;
                    colorArray[y * stride + x * 4 + 0] = b;
                    colorArray[y * stride + x * 4 + 3] = 255;
                    if (newShort == 0) colorArray[y * stride + x * 4 + 3] = 0;

                    position += 2;
                }

                position += pad * 2;
            }
            return colorArray;
        }
        public static byte[] AndroidMode3ToRGBA(Image img)
        {
            var colorArray = new byte[img.Width * img.Height * 4];
            var stride = img.Width * 4;
            var position = 0;
            var pad = GetPadding(img);
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    colorArray[y * stride + x * 4 + 2] = img.ImageData[position];
                    colorArray[y * stride + x * 4 + 1] = img.ImageData[position + 1];
                    colorArray[y * stride + x * 4 + 0] = img.ImageData[position + 2];
                    colorArray[y * stride + x * 4 + 3] = 255;
                    position += 3;
                }

                position += pad;

            }
            return colorArray;
        }
        public static byte[] AndroidMode4ToRGBA(Image img)
        {
            var colorArray = new byte[img.Width * img.Height * 4];
            var stride = img.Width * 4;
            var position = 0;
            var pad = GetPadding(img);
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var newShort = (ushort)(img.ImageData[position] | (img.ImageData[position + 1] << 8));

                    var r = (byte)((newShort & 0xF800) >> 11);
                    var g = (byte)((newShort & 0x7E0) >> 5);
                    var b = (byte)(newShort & 0x1F);

                    r = (byte)(r << 3);
                    g = (byte)(g << 2);
                    b = (byte)(b << 3);
                    //r done
                    //g partially done

                    colorArray[y * stride + x * 4 + 2] = r;
                    colorArray[y * stride + x * 4 + 1] = g;
                    colorArray[y * stride + x * 4 + 0] = b;
                    colorArray[y * stride + x * 4 + 3] = 255;

                    position += 2;
                }

                position += pad * 2;
            }
            return colorArray;
        }
        public static byte[] AndroidMode5ToRGBA(Image img)
        {
            var newImg = new Data.Chunks.BankChunks.Images.Image();
            newImg.FromBitmap(new Bitmap(Bitmap.FromStream(new MemoryStream(img.ImageData))));
            return Normal24BitMaskedToRGBA(newImg);
        }
        public static byte[] TwoFivePlusToRGBA(Image img)
        {
            //Logger.Log("TwoFivePlusToRGBA, Image Data Size: " + img.ImageData.Length + ", Size: " + img.Width + "x" + img.Height + ", Alpha: " + img.Flags["Alpha"] + ", Transparent Color: " + img.TransparentColor + ", img.Flags["RGBA"]: " + img.Flags["RGBA"] + ", Flip RGB: " + flipRGB);
            byte[] colorArray = new byte[img.Width * img.Height * 4];
            int stride = img.Width * 4;
            int pad = GetPadding(img);
            int position = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int newPos = (y * stride) + (x * 4);
                    if (NebulaCore.Fusion == 3.0f && !NebulaCore.Seeded)
                    {
                        colorArray[newPos + 0] = img.ImageData[position + 2];
                        colorArray[newPos + 1] = img.ImageData[position + 1];
                        colorArray[newPos + 2] = img.ImageData[position + 0];
                    }
                    else
                    {
                        colorArray[newPos + 0] = img.ImageData[position + 0];
                        colorArray[newPos + 1] = img.ImageData[position + 1];
                        colorArray[newPos + 2] = img.ImageData[position + 2];
                    }
                    colorArray[newPos + 3] = 255;
                    if (img.Flags["Alpha"] || img.Flags["RGBA"])
                    {
                        if (NebulaCore.PackageData.ExtendedHeader.Flags["PremultipliedAlpha"])
                        {
                            float a = img.ImageData[position + 3] / 255f;
                            colorArray[newPos + 0] = (byte)(colorArray[newPos + 0] / a);
                            colorArray[newPos + 1] = (byte)(colorArray[newPos + 1] / a);
                            colorArray[newPos + 2] = (byte)(colorArray[newPos + 2] / a);
                        }
                        colorArray[newPos + 3] = img.ImageData[position + 3];
                    }
                    else
                    {
                        if (img.ImageData[newPos + 2] == img.TransparentColor.R &&
                            img.ImageData[newPos + 1] == img.TransparentColor.G &&
                            img.ImageData[newPos + 0] == img.TransparentColor.B)
                            colorArray[newPos + 3] = 0;
                    }
                    position += 4;
                }

                position += pad * 4;
            }
            if (position == img.ImageData.Length)
                return colorArray;
            if (img.Flags["Alpha"] && !img.Flags["RGBA"])
            {
                int aPad = GetAlphaPadding(img);
                int aStride = img.Width * 4;
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = img.ImageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;
        }
        public static byte[] FlashToRGBA(Image img)
        {
            //Logger.Log("FlashToRGBA, Image Data Size: " + img.ImageData.Length + ", Size: " + img.Width + "x" + img.Height + ", Alpha: " + img.Flags["Alpha"] + ", Transparent Color: " + img.TransparentColor + ", img.Flags["RGBA"]: " + img.Flags["RGBA"] + ", Flip RGB: " + flipRGB);
            byte[] colorArray = new byte[img.Width * img.Height * 4];
            int stride = img.Width * 4;
            int pad = GetPadding(img);
            int position = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int newPos = (y * stride) + (x * 4);
                    colorArray[newPos + 0] = img.ImageData[position + 3];
                    colorArray[newPos + 1] = img.ImageData[position + 2];
                    colorArray[newPos + 2] = img.ImageData[position + 1];

                    colorArray[newPos + 3] = 255;
                    colorArray[(y * stride) + (x * 4) + 3] = img.ImageData[position + 0];
                    position += 4;
                }

                position += pad * 4;
            }
            if (position == img.ImageData.Length)
                return colorArray;

            int aPad = GetPadding(img);
            int aStride = img.Width * 4;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    colorArray[(y * aStride) + (x * 4) + 3] = img.ImageData[position];
                    position += 1;
                }
                position += aPad;
            }

            return colorArray;
        }

        public static byte[] RGBAToRGBMasked(Image img)
        {
            //Logger.Log("RGBAToRGBMasked, Image Data Size: " + img.ImageData.Length + ", Size: " + img.Width + "x" + img.Height + ", Alpha: " + img.Flags["Alpha"] + ", Transparent Color: " + img.TransparentColor + ", img.Flags["RGBA"]: " + img.Flags["RGBA"]);
            byte[] colorArray = new byte[img.Width * img.Height * 8];
            int stride = img.Width * 4;
            int pad = GetPadding(img);
            int position = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int pos = (y * stride) + (x * 4);
                    colorArray[position + 0] = img.ImageData[pos + 0];
                    colorArray[position + 1] = img.ImageData[pos + 1];
                    colorArray[position + 2] = img.ImageData[pos + 2];
                    colorArray[position + 3] = img.ImageData[pos + 3];

                    if (!img.Flags["Alpha"] && img.Flags["RGBA"] && img.ImageData[pos + 3] != 255)
                    {
                        colorArray[position + 2] = img.TransparentColor.R;
                        colorArray[position + 1] = img.TransparentColor.G;
                        colorArray[position + 0] = img.TransparentColor.B;
                    }

                    position += 3;
                }

                position += pad * 3;
            }

            if (img.Flags["Alpha"])
            {
                int aPad = GetAlphaPadding(img);

                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        int pos = (y * stride) + (x * 4);
                        if (position >= colorArray.Length || pos + 3 >= img.ImageData.Length)
                            break;
                        colorArray[position] = img.ImageData[pos + 3];
                        position += 1;
                    }

                    position += aPad;
                }
            }

            Array.Resize(ref colorArray, position);
            return colorArray;
        }
        public static byte[] ColorPaletteToRGBA(Image img, List<Color> palette)
        {
            byte[] colorArray = new byte[img.Width * img.Height * 4];
            int stride = img.Width * 4;
            int position = 0;
            int command = img.ImageData[position];
            bool rleLoop = false;
            bool rleCommander = false;
            bool rle = img.Flags["RLE"] || img.Flags["RLEW"] || img.Flags["RLET"];
            if (rle)
                position++;

            Color rgb = Color.White;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (!rle || !rleLoop || rleCommander)
                    {
                        rgb = palette[img.ImageData[position++]];
                        rleLoop = true;
                    }

                    int newPos = (y * stride) + (x * 4);
                    colorArray[newPos + 2] = rgb.R;
                    colorArray[newPos + 1] = rgb.G;
                    colorArray[newPos + 0] = rgb.B;
                    colorArray[newPos + 3] = 255;
                    if (colorArray[newPos + 2] == img.TransparentColor.R &&
                        colorArray[newPos + 1] == img.TransparentColor.G &&
                        colorArray[newPos + 0] == img.TransparentColor.B)
                        colorArray[newPos + 3] = 0;

                    if (rle && --command == 0)
                    {
                        command = img.ImageData[position++];
                        rleCommander = false;
                        rleLoop = false;

                        if (command > 128)
                        {
                            command -= 128;
                            rleCommander = true;
                        }
                        else if (command == 0)
                            rleLoop = true;
                    }
                }
            }

            return colorArray;
        }
    }
}
