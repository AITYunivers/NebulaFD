using System.Drawing;

namespace SapphireD.Core.Utilities
{
    public static class ImageTranslator
    {
        public static int GetPadding(int width, int pointSize, int bytes = 2, bool modular = false)
        {
            if (modular) return (bytes - width * pointSize % bytes) % bytes;

            var pad = bytes - (width * pointSize % bytes);
            if (pad == bytes) return 0;

            return (int)Math.Ceiling(pad / (float)pointSize);
        }

        public static byte[] Normal24BitMaskedToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent, bool flipRGB = false)
        {
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 3);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int newPos = (y * stride) + (x * 4);
                    if (flipRGB)
                    {
                        colorArray[newPos + 0] = imageData[position + 2];
                        colorArray[newPos + 1] = imageData[position + 1];
                        colorArray[newPos + 2] = imageData[position];
                    }
                    else
                    {
                        colorArray[newPos + 0] = imageData[position];
                        colorArray[newPos + 1] = imageData[position + 1];
                        colorArray[newPos + 2] = imageData[position + 2];
                    }
                    colorArray[newPos + 3] = 255;
                    if (!alpha)
                    {
                        if (colorArray[newPos + 0] == transparent.B && 
                            colorArray[newPos + 1] == transparent.G &&
                            colorArray[newPos + 2] == transparent.R)
                            colorArray[newPos + 3] = 0;
                    }
                    position += 3;
                }

                position += pad * 3;
            }

            if (alpha)
            {
                int aPad = GetPadding(width, 1, 4);
                int aStride = width * 4;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = imageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;
        }
        public static byte[] Normal16BitToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent)
        {
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 2);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UInt16 newShort = (ushort)(imageData[position] | imageData[position + 1] << 8);
                    byte r = (byte)((newShort & 63488) >> 11);
                    byte g = (byte)((newShort & 2016) >> 5);
                    byte b = (byte)((newShort & 31));

                    r = (byte)(r << 3);
                    g = (byte)(g << 2);
                    b = (byte)(b << 3);
                    int newPos = (y * stride) + (x * 4);
                    colorArray[newPos + 2] = r;
                    colorArray[newPos + 1] = g;
                    colorArray[newPos + 0] = b;
                    colorArray[newPos + 3] = 255;
                    if (!alpha)
                    {
                        if (colorArray[newPos + 2] == transparent.R && 
                            colorArray[newPos + 1] == transparent.G &&
                            colorArray[newPos + 0] == transparent.B)
                            colorArray[newPos + 3] = 0;
                    }
                    position += 2;
                }

                position += pad * 2;
            }
            if (alpha)
            {
                int aPad = GetPadding(width, 1, 4);
                int aStride = width * 4;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = imageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;
        }
        public static byte[] Normal15BitToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent)
        {
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 2);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UInt16 newShort = (ushort)(imageData[position] | imageData[position + 1] << 8);
                    byte r = (byte)((newShort & 31744) >> 10);
                    byte g = (byte)((newShort & 992) >> 5);
                    byte b = (byte)((newShort & 31));

                    r = (byte)(r << 3);
                    g = (byte)(g << 3);
                    b = (byte)(b << 3);
                    int newPos = (y * stride) + (x * 4);
                    colorArray[newPos + 2] = r;
                    colorArray[newPos + 1] = g;
                    colorArray[newPos + 0] = b;
                    colorArray[newPos + 3] = 255;
                    if (!alpha)
                    {
                        if (colorArray[newPos + 2] == transparent.R && 
                            colorArray[newPos + 1] == transparent.G &&
                            colorArray[newPos + 0] == transparent.B)
                            colorArray[newPos + 3] = 0;
                    }
                    position += 2;
                }

                position += pad * 2;
            }
            if (alpha)
            {
                int aPad = GetPadding(width, 1, 4);
                int aStride = width * 4;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = imageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;

        }
        public static byte[] Normal8BitToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var newImg = new Data.Chunks.BankChunks.Images.Image();
            //newImg.FromBitmap((Bitmap)Bitmap.FromStream(new MemoryStream(imageData)));
            return newImg.ImageData;
        }
        public static byte[] AndroidMode0ToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var colorArray = new byte[width * height * 4];
            var stride = width * 4;
            var position = 0;
            var pad = GetPadding(width, 4);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    colorArray[y * stride + x * 4 + 2] = imageData[position];
                    colorArray[y * stride + x * 4 + 1] = imageData[position + 1];
                    colorArray[y * stride + x * 4 + 0] = imageData[position + 2];
                    colorArray[y * stride + x * 4 + 3] = imageData[position + 3];
                    position += 4;
                }

                position += pad * 3;
            }

            return colorArray;
        }
        public static byte[] AndroidMode1ToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var colorArray = new byte[width * height * 4];
            var stride = width * 4;
            var position = 0;
            var pad = GetPadding(width, 3);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var newShort = (ushort)(imageData[position] | (imageData[position + 1] << 8));

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
        public static byte[] AndroidMode2ToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var colorArray = new byte[width * height * 4];
            var stride = width * 4;
            var position = 0;
            var pad = GetPadding(width, 3);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var newShort = (ushort)(imageData[position] | (imageData[position + 1] << 8));

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
        public static byte[] AndroidMode3ToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var colorArray = new byte[width * height * 4];
            var stride = width * 4;
            var position = 0;
            var pad = GetPadding(width, 3, 4, true);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    colorArray[y * stride + x * 4 + 2] = imageData[position];
                    colorArray[y * stride + x * 4 + 1] = imageData[position + 1];
                    colorArray[y * stride + x * 4 + 0] = imageData[position + 2];
                    colorArray[y * stride + x * 4 + 3] = 255;
                    position += 3;
                }

                position += pad;

            }
            return colorArray;
        }
        public static byte[] AndroidMode4ToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var colorArray = new byte[width * height * 4];
            var stride = width * 4;
            var position = 0;
            var pad = GetPadding(width, 3);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var newShort = (ushort)(imageData[position] | (imageData[position + 1] << 8));

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
        public static byte[] AndroidMode5ToRGBA(byte[] imageData, int width, int height, bool alpha)
        {
            var img = new Data.Chunks.BankChunks.Images.Image();
            //img.FromBitmap((Bitmap)Bitmap.FromStream(new MemoryStream(imageData)));
            return Normal24BitMaskedToRGBA(img.ImageData, width, height, true, Color.Black);
        }
        public static byte[] TwoFivePlusToRGBA(byte[] imageData, int width, int height, bool alpha, Color transparent, bool RGBA, bool flipRGB = false)
        {
            //Logger.Log("TwoFivePlusToRGBA, Image Data Size: " + imageData.Length + ", Size: " + width + "x" + height + ", Alpha: " + alpha + ", Transparent Color: " + transparent + ", RGBA: " + RGBA + ", Flip RGB: " + flipRGB);
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 4);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int newPos = (y * stride) + (x * 4);
                    if (flipRGB)
                    {
                        colorArray[newPos + 0] = imageData[position + 2];
                        colorArray[newPos + 1] = imageData[position + 1];
                        colorArray[newPos + 2] = imageData[position + 0];
                    }
                    else
                    {
                        colorArray[newPos + 0] = imageData[position + 0];
                        colorArray[newPos + 1] = imageData[position + 1];
                        colorArray[newPos + 2] = imageData[position + 2];
                    }
                    colorArray[newPos + 3] = 255;
                    if (alpha || RGBA)
                    {
                        colorArray[(y * stride) + (x * 4) + 3] = imageData[position + 3];
                    }
                    else
                    {
                        if (imageData[newPos + 2] == transparent.R &&
                            imageData[newPos + 1] == transparent.G &&
                            imageData[newPos + 0] == transparent.B)
                            colorArray[newPos + 3] = 0;
                    }
                    position += 4;
                }

                position += pad * 4;
            }
            if (position == imageData.Length)
                return colorArray;
            if (alpha && !RGBA)
            {
                int aPad = GetPadding(width, 1, 4);
                int aStride = width * 4;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        colorArray[(y * aStride) + (x * 4) + 3] = imageData[position];
                        position += 1;
                    }
                    position += aPad;
                }
            }

            return colorArray;
        }
        public static byte[] RGBAToRGBMasked(byte[] imageData, int width, int height, bool alpha, bool RGBA = false) => RGBAToRGBMasked(imageData, width, height, alpha, Color.Black, RGBA);
        public static byte[] RGBAToRGBMasked(byte[] imageData, int width, int height, bool alpha, Color transparent, bool RGBA = false)
        {
            //Logger.Log("RGBAToRGBMasked, Image Data Size: " + imageData.Length + ", Size: " + width + "x" + height + ", Alpha: " + alpha + ", Transparent Color: " + transparent + ", RGBA: " + RGBA);
            byte[] colorArray = new byte[width * height * 8];
            int stride = width * 4;
            int pad = GetPadding(width, 3);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pos = (y * stride) + (x * 4);
                    colorArray[position + 0] = imageData[pos + 0];
                    colorArray[position + 1] = imageData[pos + 1];
                    colorArray[position + 2] = imageData[pos + 2];
                    colorArray[position + 3] = imageData[pos + 3];

                    if (!alpha && RGBA && imageData[pos + 3] != 255)
                    {
                        colorArray[position + 2] = transparent.R;
                        colorArray[position + 1] = transparent.G;
                        colorArray[position + 0] = transparent.B;
                    }

                    position += 3;
                }

                position += pad * 3;
            }

            if (alpha)
            {
                int aPad = GetPadding(width, 1, 4);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int pos = (y * stride) + (x * 4);
                        if (position >= colorArray.Length || pos + 3 >= imageData.Length)
                            break;
                        colorArray[position] = imageData[pos + 3];
                        position += 1;
                    }

                    position += aPad;
                }
            }

            Array.Resize(ref colorArray, position);
            return colorArray;
        }
    }
}
