using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterChunk : Chunk
    {
        public int? Code = null;

        public ParameterChunk(int? code = null)
        {
            ChunkName = "ParameterChunk";
            Code = code;
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

        public override string ToString()
        {
            return "Unknown Parameter Chunk " + Code == null ? "" : Code.ToString()!;
        }
    }
}
