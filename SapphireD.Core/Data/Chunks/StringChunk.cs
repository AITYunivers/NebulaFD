using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks
{
    public abstract class StringChunk : Chunk
    {
        public string Value = string.Empty;

        public override void ReadCCN(ByteReader reader)
        {
            Value = reader.ReadYuniversal();
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {
            writer.WriteUnicode(Value);
        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
