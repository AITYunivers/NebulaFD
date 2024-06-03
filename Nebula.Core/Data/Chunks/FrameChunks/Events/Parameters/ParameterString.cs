using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
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
            writer.WriteYunicode(Value);
            writer.Skip((263 - Value.Length) * 2);
        }

        public override string ToString()
        {
            return '"' + Value + '"';
        }
    }
}
