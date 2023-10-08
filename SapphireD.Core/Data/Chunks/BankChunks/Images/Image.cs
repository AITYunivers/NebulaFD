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
        public Color TransparentColor;

        public byte[] ImageData;
        private Bitmap BitmapCache;

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
            if (SapDCore.Fusion == 2.5f && !SapDCore.Plus && !SapDCore.Android && !SapDCore.iOS)
                return new Image25();
            return new Image();
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
                            colorArray = ImageTranslator.Normal24BitMaskedToRGBA(ImageData, Width, Height, Flags["Alpha"], TransparentColor, SapDCore.Fusion == 3f);
                        break;
                    case 5:
                        colorArray = ImageTranslator.AndroidMode5ToRGBA(ImageData, Width, Height, Flags["Alpha"]);
                        break;
                    case 6:
                        colorArray = ImageTranslator.Normal15BitToRGBA(ImageData, Width, Height, false, TransparentColor);
                        break;
                    case 7:
                        colorArray = ImageTranslator.Normal16BitToRGBA(ImageData, Width, Height, false, TransparentColor);
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
