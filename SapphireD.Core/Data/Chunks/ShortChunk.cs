using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks
{
    public abstract class ShortChunk : Chunk
    {
        public short Value;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt16(Value);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
