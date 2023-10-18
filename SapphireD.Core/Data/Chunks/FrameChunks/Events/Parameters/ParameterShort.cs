using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterShort : ParameterChunk
    {
        public short Value;

        public ParameterShort()
        {
            ChunkName = "ParameterShort";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadShort();
        }
    }
}
