using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class Extensions : Chunk
    {
        public Extension[] Exts = new Extension[0];
        public ushort Conditions;

        public Extensions()
        {
            ChunkName = "Extensions";
            ChunkID = 0x2234;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Exts = new Extension[reader.ReadUShort()];
            Conditions = reader.ReadUShort();

            for (int i = 0; i < Exts.Length; i++)
            {
                Extension ext = new Extension();
                ext.ReadCCN(reader);
                Exts[i] = ext;
            }

            SapDCore.PackageData.Extensions = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Exts.Length);
            foreach (Extension ext in Exts)
                ext.WriteMFA(writer);
        }
    }
}
