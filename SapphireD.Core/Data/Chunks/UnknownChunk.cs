using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks
{
    public class UnknownChunk : Chunk
    {
        public UnknownChunk()
        {
            ChunkName = "Unknown Chunk";
            ChunkID = 0x0;
        }

        public override void ReadCCN(ByteReader reader)
        {
            //Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void ReadMFA(ByteReader reader)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void WriteCCN(ByteWriter writer)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void WriteMFA(ByteWriter writer)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }
    }
}
