using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
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

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            left = reader.ReadInt();
            top = reader.ReadInt();
            right = reader.ReadInt();
            bottom = reader.ReadInt();

            ((Frame)extraInfo[0]).FrameRect = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            left = reader.ReadInt();
            top = reader.ReadInt();
            right = reader.ReadInt();
            bottom = reader.ReadInt();

            ((Frame)extraInfo[0]).FrameRect = this;
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
