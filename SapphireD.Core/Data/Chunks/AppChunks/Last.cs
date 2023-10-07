using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class Last : Chunk
    {
        public Last()
        {
            ChunkName = "Last";
            ChunkID = 0x7F7F;
        }

        public override void ReadCCN(ByteReader reader)
        {

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
