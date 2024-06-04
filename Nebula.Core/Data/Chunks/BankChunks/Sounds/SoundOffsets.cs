using Nebula.Core.Data.Chunks.BankChunks.Fonts;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Sounds
{
    public class SoundOffsets : Chunk
    {
        public int[] Offsets = new int[0];
        public List<int> SortedOffsets = new List<int>();

        public SoundOffsets()
        {
            ChunkName = "SoundOffsets";
            ChunkID = 0x5557;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Offsets = new int[reader.Size() / 4];
            for (int i = 0; i < Offsets.Length; i++)
                Offsets[i] = reader.ReadInt();

            SortedOffsets = Offsets.ToList();
            SortedOffsets.RemoveAll(x => x == 0);
            SortedOffsets.Sort();

            NebulaCore.PackageData.SoundOffsets = this;
            if (NebulaCore.Fusion == 1.5f)
            {
                SoundBank bnk = NebulaCore.PackageData.SoundBank;
                ByteReader bnkReader = new ByteReader(bnk.ChunkData);
                int count = bnk.Count;
                if (Offsets.Length > 0)
                    count = Offsets.Length;
                for (int i = 0; i < count; i++)
                {
                    Sound snd = new Sound();
                    if (Offsets.Length > 0 && Offsets[i] == 0)
                        continue;
                    if (Offsets.Length > 0)
                        bnkReader.Seek(Offsets[i] - 4);
                    snd.ReadCCN(bnkReader);
                    bnk.Sounds[snd.Handle] = snd;
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
