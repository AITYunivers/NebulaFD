using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Fonts
{
    public class FontBank : Chunk
    {
        public int Count;
        public Dictionary<uint, Font> Fonts = new();

        public FontBank()
        {
            ChunkName = "FontBank";
            ChunkID = 0x6667;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt();
            for (int i = 0; i < Count; i++)
            {
                Font fnt = new Font();
                fnt.ReadCCN(reader);
                Fonts[fnt.Handle] = fnt;
            }
            SapDCore.PackageData.FontBank = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt();
            for (int i = 0; i < Count; i++)
            {
                Font fnt = new Font();
                fnt.ReadMFA(reader);
                Fonts[fnt.Handle] = fnt;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Count);
            foreach (Font fnt in Fonts.Values)
                fnt.WriteMFA(writer);
        }
    }
}
