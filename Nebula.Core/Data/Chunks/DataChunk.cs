using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks
{
    public abstract class DataChunk : Chunk
    {
        public byte[] Value = new byte[0];

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadBytes();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteBytes(Value);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
