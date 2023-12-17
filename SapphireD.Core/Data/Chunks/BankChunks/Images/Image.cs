using SapphireD.Core.FileReaders;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class Image : Chunk
    {
        /*
         * ===GRAPHIC MODES===
         *
         * 0 - android, transparency, 4 bytes per pixel, 8 bits per channel
         * 1 - android, transparency, 2 bytes per pixel, 4 bits per channel
         * 2 - android, transparency, 2 bytes per pixel, 5 bits per channel
         * 3 - android, no transparency, 3 bytes per pixel, 8 bits per channel
         * 4 - android, no transparency, 2 bytes per pixel, 5 bits per channel
         * 4 - normal, 24 bits per color, 8 bit-deep alpha mask at the end
         * 4 - mmf1.5, i don't like that
         * 5 - android, no transparency, JPEG
         * 6 - normal, 15 bits per pixel, but it's actually 16 but retarded
         * 7 - normal, 16 bits per pixel
         * 8 - 2.5+, 32 bits per pixel, 8 bits per color
         * 9 - Flash, basically 8 but backwards
         */

        public uint Handle;
        public int Checksum;
        public int References;
        public short Width;
        public short Height;
        public byte GraphicMode;
        public short HotspotX;
        public short HotspotY;
        public short ActionPointX;
        public short ActionPointY;
        public Color TransparentColor = Color.Black;

        public byte[] ImageData = new byte[0];
        private Bitmap? BitmapCache = null;

        public BitDict Flags = new BitDict(new string[]
        {
            "RLE",
            "RLEW",
            "RLET",
            "LZX",
            "Alpha",
            "ACE",
            "Mac",
            "RGBA"
        });

        public Image()
        {
            ChunkName = "Image";
        }

        public static Image NewImage()
        {
            if (SapDCore.MFA)
                return new ImageMFA();
            else if (SapDCore.CurrentReader is OpenFileReader)
                return new ImageOpen();
            else if (SapDCore.Android || SapDCore.iOS)
                return new ImageMobile();
            else if (SapDCore.Flash)
                return new ImageFlash();
            else if (SapDCore.PackageData.ExtendedHeader.CompressionFlags["OptimizeImageSize"])
                return new Image25Plus();
            return new Image25();
        }

        public Bitmap GetBitmap()
        {
            if (BitmapCache == null)
            {
                BitmapCache = new Bitmap(Width, Height);
                var bmpData = BitmapCache.LockBits(new Rectangle(0, 0, Width, Height),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format32bppArgb);

                byte[] colorArray = null;
                switch (GraphicMode)
                {
                    case 0:
                        colorArray = ImageTranslator.AndroidMode0ToRGBA(ImageData, Width, Height, false);
                        break;
                    case 1:
                        colorArray = ImageTranslator.AndroidMode1ToRGBA(ImageData, Width, Height, false);
                        break;
                    case 2:
                        colorArray = ImageTranslator.AndroidMode2ToRGBA(ImageData, Width, Height, false);
                        break;
                    case 3:
                        colorArray = ImageTranslator.AndroidMode3ToRGBA(ImageData, Width, Height, false);
                        break;
                    case 4:
                        if (SapDCore.Android)
                            colorArray = ImageTranslator.AndroidMode4ToRGBA(ImageData, Width, Height, false);
                        else
                            colorArray = ImageTranslator.Normal24BitMaskedToRGBA(ImageData, Width, Height, Flags["Alpha"], TransparentColor, Flags["RLE"] || Flags["RLEW"] || Flags["RLET"], SapDCore.Fusion == 3f);
                        break;
                    case 5:
                        colorArray = ImageTranslator.AndroidMode5ToRGBA(ImageData, Width, Height, Flags["Alpha"], Flags["RLE"] || Flags["RLEW"] || Flags["RLET"]);
                        break;
                    case 6:
                        colorArray = ImageTranslator.Normal15BitToRGBA(ImageData, Width, Height, false, TransparentColor);
                        break;
                    case 7:
                        colorArray = ImageTranslator.Normal16BitToRGBA(ImageData, Width, Height, false, TransparentColor, Flags["RLE"] || Flags["RLEW"] || Flags["RLET"]);
                        break;
                    case 8:
                        colorArray = ImageTranslator.TwoFivePlusToRGBA(ImageData, Width, Height, Flags["Alpha"], TransparentColor, Flags["RGBA"], SapDCore.Fusion == 3f);
                        break;
                    case 9:
                        colorArray = ImageTranslator.FlashToRGBA(ImageData, Width, Height);
                        break;
                }

                Marshal.Copy(colorArray, 0, bmpData.Scan0, colorArray.Length);
                BitmapCache.UnlockBits(bmpData);
            }

            return BitmapCache;
        }

        public void FromBitmap(Bitmap bmp)
        {
            Width = (short)bmp.Width;
            Height = (short)bmp.Height;
            GraphicMode = 4;

            var bitmapData = bmp.LockBits(new Rectangle(0, 0, Width, Height),
                                          ImageLockMode.ReadOnly,
                                          PixelFormat.Format24bppRgb);
            var copyPad = ImageHelper.GetPadding(Width, 4);
            var length = bitmapData.Height * bitmapData.Stride + copyPad * 4;

            var bytes = new byte[length];
            var stride = bitmapData.Stride;
            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            bmp.UnlockBits(bitmapData);

            ImageData = new byte[Width * Height * 6];
            var position = 0;
            var pad = ImageHelper.GetPadding(Width, 3);

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var newPos = y * stride + x * 3;
                    ImageData[position] = bytes[newPos];
                    ImageData[position + 1] = bytes[newPos + 1];
                    ImageData[position + 2] = bytes[newPos + 2];
                    position += 3;
                }

                position += 3 * pad;
            }

            var bitmapDataAlpha = bmp.LockBits(new Rectangle(0, 0, Width, Height),
                                                ImageLockMode.ReadOnly,
                                                PixelFormat.Format32bppArgb);
            var copyPadAlpha = ImageHelper.GetPadding(Width, 1);
            var lengthAlpha = bitmapDataAlpha.Height * bitmapDataAlpha.Stride + copyPadAlpha * 4;

            var bytesAlpha = new byte[lengthAlpha];
            var strideAlpha = bitmapDataAlpha.Stride;
            Marshal.Copy(bitmapDataAlpha.Scan0, bytesAlpha, 0, lengthAlpha);
            bmp.UnlockBits(bitmapDataAlpha);

            var aPad = ImageHelper.GetPadding(Width, 1, 4);
            var alphaPos = position;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    ImageData[alphaPos] = bytesAlpha[y * strideAlpha + x * 4 + 3];
                    alphaPos += 1;
                }

                alphaPos += aPad;
            }
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            PrepareForMfa();
            Flags["LZX"] = true;

            byte[] compressedImg = Decompressor.CompressBlock(ImageData);

            writer.WriteUInt(Handle);
            writer.WriteInt(Checksum);
            writer.WriteInt(References);
            writer.WriteInt(compressedImg.Length + 4);
            writer.WriteShort(Width);
            writer.WriteShort(Height);
            writer.WriteByte(GraphicMode);
            writer.WriteByte((byte)Flags.Value);
            writer.WriteShort(0);
            writer.WriteShort(HotspotX);
            writer.WriteShort(HotspotY);
            writer.WriteShort(ActionPointX);
            writer.WriteShort(ActionPointY);
            writer.WriteColor(TransparentColor);
            writer.WriteInt(ImageData.Length);
            writer.WriteBytes(compressedImg);
        }

        public void PrepareForMfa()
        {
            switch (GraphicMode)
            {
                case 0:
                    ImageData = ImageTranslator.AndroidMode0ToRGBA(ImageData, Width, Height, Flags["Alpha"]);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    GraphicMode = 4;
                    break;
                case 1:
                    ImageData = ImageTranslator.AndroidMode1ToRGBA(ImageData, Width, Height, Flags["Alpha"]);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    GraphicMode = 4;
                    break;
                case 2:
                    ImageData = ImageTranslator.AndroidMode2ToRGBA(ImageData, Width, Height, Flags["Alpha"]);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    GraphicMode = 4;
                    break;
                case 3:
                    ImageData = ImageTranslator.AndroidMode3ToRGBA(ImageData, Width, Height, Flags["Alpha"]);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    GraphicMode = 4;
                    break;
                case 4:
                    if (SapDCore.Android)
                    {
                        ImageData = ImageTranslator.AndroidMode4ToRGBA(ImageData, Width, Height, Flags["Alpha"]);
                        ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    }
                    else if (SapDCore.Fusion > 2.5f)
                    {
                        ImageData = ImageTranslator.Normal24BitMaskedToRGBA(ImageData, Width, Height, Flags["Alpha"], TransparentColor, Flags["RLE"] || Flags["RLEW"] || Flags["RLET"], true);
                        ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    }
                    break;
                case 5:
                    ImageData = ImageTranslator.AndroidMode5ToRGBA(ImageData, Width, Height, Flags["Alpha"], Flags["RLE"] || Flags["RLEW"] || Flags["RLET"]);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"]);
                    GraphicMode = 4;
                    break;
                case 8:
                    ImageData = ImageTranslator.TwoFivePlusToRGBA(ImageData, Width, Height, Flags["Alpha"], TransparentColor, Flags["RGBA"], SapDCore.Fusion > 2.5f);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"], TransparentColor, Flags["RGBA"]);
                    GraphicMode = 4;
                    break;
                case 9:
                    ImageData = ImageTranslator.FlashToRGBA(ImageData, Width, Height);
                    ImageData = ImageTranslator.RGBAToRGBMasked(ImageData, Width, Height, Flags["Alpha"], TransparentColor, Flags["RGBA"]);
                    GraphicMode = 4;
                    break;
            }
        }

        public Image Clone()
        {
            Image newImg = new Image();
            newImg.Handle = Handle;
            newImg.Checksum = Checksum;
            newImg.References = References;
            newImg.Width = Width;
            newImg.Height = Height;
            newImg.GraphicMode = GraphicMode;
            newImg.HotspotX = HotspotX;
            newImg.HotspotY = HotspotY;
            newImg.ActionPointX = ActionPointX;
            newImg.ActionPointY = ActionPointY;
            newImg.TransparentColor = TransparentColor;

            newImg.ImageData = ImageData;
            newImg.BitmapCache = BitmapCache;

            newImg.Flags.Value = Flags.Value;
            return newImg;
        }
    }
}
