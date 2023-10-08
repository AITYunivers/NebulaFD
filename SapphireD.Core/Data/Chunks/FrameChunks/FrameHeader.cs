using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameHeader : Chunk
    {
        public int Width;
        public int Height;
        public Color Background;

        public BitDict Flags = new BitDict(new string[]      // Flags
        {
            "1", "2", "3", "4", "5"
        });

        public FrameHeader()
        {
            ChunkName = "FrameHeader";
            ChunkID = 0x3334;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Background = reader.ReadColor();
            Flags.Value = reader.ReadUInt();

            ((Frame)extraInfo[0]).FrameHeader = this;
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
