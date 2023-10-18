using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterDoubleShort : ParameterChunk
    {
        public short Value1;
        public short Value2;

        public ParameterDoubleShort()
        {
            ChunkName = "ParameterDoubleShort";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value1 = reader.ReadShort();
            Value2 = reader.ReadShort();
        }
    }
}
