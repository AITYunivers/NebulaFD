using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks
{
    public abstract class IntChunk : Chunk
    {
        public int Value;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadInt();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Value);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
