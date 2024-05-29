using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.ChunkTypes
{
    public class UnknownChunk : Chunk
    {
        public UnknownChunk()
        {
            ChunkName = "UnknownChunk";
            ChunkID = 0x0;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            this.Log("Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            this.Log("Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            this.Log("Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            this.Log("Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }
    }
}
