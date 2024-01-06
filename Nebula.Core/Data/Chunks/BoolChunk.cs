using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks
{
    public abstract class BoolChunk : Chunk
    {
        public bool Value;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadInt() != 0;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Value ? 1 : 0);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
