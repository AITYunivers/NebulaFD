using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ChunkTypes
{
    public abstract class BoolChunk : Chunk
    {
        public bool Value;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadBool();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteBool(Value);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
