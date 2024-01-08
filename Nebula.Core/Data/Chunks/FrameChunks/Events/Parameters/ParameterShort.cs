using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
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

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
