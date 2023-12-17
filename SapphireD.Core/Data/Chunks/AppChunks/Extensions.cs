using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class Extensions : Chunk
    {
        public Extension[] Exts = new Extension[0];

        public Extensions()
        {
            ChunkName = "Extensions";
            ChunkID = 0x2234;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Exts = new Extension[reader.ReadUShort()];
            reader.Skip(2);

            for (int i = 0; i < Exts.Length; i++)
            {
                Exts[i] = new Extension();
                Exts[i].ReadCCN(reader);
            }

            SapDCore.PackageData.Extensions = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Exts = new Extension[reader.ReadInt()];
            for (int i = 0; i < Exts.Length; i++)
            {
                Exts[i] = new Extension();
                Exts[i].ReadMFA(reader);
            }
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
