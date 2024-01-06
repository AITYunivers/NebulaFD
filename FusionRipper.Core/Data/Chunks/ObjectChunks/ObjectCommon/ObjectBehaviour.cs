using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectBehaviour : Chunk
    {
        public string Name = string.Empty;
        public byte[] Data = new byte[0];

        public ObjectBehaviour()
        {
            ChunkName = "ObjectBehaviour";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Name = reader.ReadAutoYuniversal();
            Data = reader.ReadBytes(reader.ReadInt());
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Data.Length);
            writer.WriteBytes(Data);
        }
    }
}
