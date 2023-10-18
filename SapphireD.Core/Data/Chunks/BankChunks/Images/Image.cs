using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            else if (SapDCore.Plus)
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
                }

                Marshal.Copy(colorArray, 0, bmpData.Scan0, colorArray.Length);
                BitmapCache.UnlockBits(bmpData);
            }

            return BitmapCache;
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

        }
    }
}
