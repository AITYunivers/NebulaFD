using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Music
{
    public class MusicOffsets : Chunk
    {
        public int[] Offsets = new int[0];
        public List<int> SortedOffsets = new List<int>();

        public MusicOffsets()
        {
            ChunkName = "MusicOffsets";
            ChunkID = 0x5558;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Offsets = new int[reader.Size() / 4];
            for (int i = 0; i < Offsets.Length; i++)
                Offsets[i] = reader.ReadInt();

            SortedOffsets = Offsets.ToList();
            SortedOffsets.RemoveAll(x => x == 0);
            SortedOffsets.Sort();

            NebulaCore.PackageData.MusicOffsets = this;
            if (NebulaCore.Fusion == 1.5f)
            {
                MusicBank bnk = NebulaCore.PackageData.MusicBank;
                ByteReader bnkReader = new ByteReader(bnk.ChunkData);
                int count = bnk.Count;
                if (Offsets.Length > 0)
                    count = Offsets.Length;
                for (int i = 0; i < count; i++)
                {
                    Music msc = new Music();
                    if (Offsets.Length > 0 && Offsets[i] == 0)
                        continue;
                    if (Offsets.Length > 0)
                        bnkReader.Seek(Offsets[i] - 4);
                    msc.ReadCCN(bnkReader);
                    bnk[msc.Handle] = msc;
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
