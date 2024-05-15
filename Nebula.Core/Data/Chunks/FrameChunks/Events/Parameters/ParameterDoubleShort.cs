using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
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

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Value1);
            writer.WriteShort(Value2);
        }

        public override string ToString()
        {
            return "Double Short " + Value1 + ", " + Value2;
        }
    }
}
