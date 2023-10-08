using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Fonts
{
    public class FontBank : Chunk
    {
        public Dictionary<uint, Font> Fonts;

        public FontBank()
        {
            ChunkName = "FontBank";
            ChunkID = 0x6667;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Fonts = new();
            var count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Font fnt = new Font();
                fnt.ReadCCN(reader);
                Fonts[fnt.Handle] = fnt;
            }
            SapDCore.PackageData.FontBank = this;
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
