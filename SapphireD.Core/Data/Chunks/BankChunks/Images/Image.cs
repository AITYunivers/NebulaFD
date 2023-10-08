using SapphireD.Core.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
