using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Fonts
{
    public class FontOffsets : Chunk
    {
        public int[] Offsets = new int[0];
        public List<int> SortedOffsets = new List<int>();

        public FontOffsets()
        {
            ChunkName = "FontOffsets";
            ChunkID = 0x5556;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Offsets = new int[reader.Size() / 4];
            for (int i = 0; i < Offsets.Length; i++)
                Offsets[i] = reader.ReadInt();

            SortedOffsets = Offsets.ToList();
            SortedOffsets.RemoveAll(x => x == 0);
            SortedOffsets.Sort();

            NebulaCore.PackageData.FontOffsets = this;
            if (NebulaCore.Fusion == 1.5f)
            {
                FontBank bnk = NebulaCore.PackageData.FontBank;
                ByteReader bnkReader = new ByteReader(bnk.ChunkData);
                int count = bnk.Count;
                if (Offsets.Length > 0)
                    count = Offsets.Length;
                for (int i = 0; i < count; i++)
                {
                    Font fnt = new Font();
                    if (Offsets.Length > 0 && Offsets[i] == 0)
                        continue;
                    if (Offsets.Length > 0)
                        bnkReader.Seek(Offsets[i] - 4);
                    fnt.ReadCCN(bnkReader);
                    bnk[fnt.Handle] = fnt;
                }
            }
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
