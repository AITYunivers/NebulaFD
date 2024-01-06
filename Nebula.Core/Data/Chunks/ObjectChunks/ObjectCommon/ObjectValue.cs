using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectValue : Chunk
    {
        public short Size;
        public int Initial;
        public int Minimum;
        public int Maximum;

        public ObjectValue()
        {
            ChunkName = "ObjectValue";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Size = reader.ReadShort();
            Initial = reader.ReadInt();
            Minimum = reader.ReadInt();
            Maximum = reader.ReadInt();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
