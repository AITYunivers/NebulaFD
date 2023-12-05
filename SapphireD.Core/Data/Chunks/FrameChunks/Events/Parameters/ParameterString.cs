using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterString : ParameterChunk
    {
        public string Value = string.Empty;

        public ParameterString()
        {
            ChunkName = "ParameterString";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadYuniversal();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUnicode(Value, true);
        }
    }
}
