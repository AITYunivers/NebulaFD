using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks
{
    public abstract class IntChunk : Chunk
    {
        public int Value;

        public override void ReadCCN(ByteReader reader)
        {
            Value = reader.ReadInt();
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {
            writer.WriteInt32(Value);
        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
