using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts
{
    public class TrueTypeFontBank : Chunk
    {
        public List<TrueTypeFont> Fonts = new();
        public Dictionary<uint, int> OffsetToIndex = new();

        public TrueTypeFontBank()
        {
            ChunkName = "TrueTypeFontBank";
            ChunkID = 0x2259;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            while (reader.HasMemory(8))
            {
                OffsetToIndex.Add((uint)reader.Tell(), Fonts.Count);
                TrueTypeFont ttf = new TrueTypeFont();
                ttf.ReadCCN(reader, false);
                Fonts.Add(ttf);
            }

            NebulaCore.PackageData.TrueTypeFontBank = this;
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

        public TrueTypeFont this[int key]
        {
            get => Fonts[key];
            set => Fonts[key] = value;
        }

        public TrueTypeFont this[uint key]
        {
            get => Fonts[(int)key];
            set => Fonts[(int)key] = value;
        }
    }
}
