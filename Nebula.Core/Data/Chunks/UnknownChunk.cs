using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks
{
    public class UnknownChunk : Chunk
    {
        public UnknownChunk()
        {
            ChunkName = "Unknown Chunk";
            ChunkID = 0x0;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            Logger.Log(this, "Unknown Chunk: 0x" + ChunkID.ToString("X"));
        }
    }
}
