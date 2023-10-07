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
    public class FrameRect : Chunk
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public FrameRect()
        {
            ChunkName = "FrameRect";
            ChunkID = 0x3342;
        }

        public override void ReadCCN(ByteReader reader)
        {
            left = reader.ReadInt();
            top = reader.ReadInt();
            right = reader.ReadInt();
            bottom = reader.ReadInt();

            Frame.curFrame.FrameRect = this;
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
