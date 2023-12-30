using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterShort : ParameterChunk
    {
        public short Value;
        public string? Extra;

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
            if (Extra != null) writer.WriteUnicode(Extra, 33);
        }
    }
}
