using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterChunk : Chunk
    {
        public ParameterChunk()
        {
            ChunkName = "ParameterChunk";
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
            return "";
        }
    }
}
