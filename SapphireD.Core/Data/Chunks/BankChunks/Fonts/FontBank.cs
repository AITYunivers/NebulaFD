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

        public override void ReadCCN(ByteReader reader)
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

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
