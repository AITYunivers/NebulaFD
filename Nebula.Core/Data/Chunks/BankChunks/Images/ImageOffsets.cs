using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class ImageOffsets : Chunk
    {
        public int[] Offsets = new int[0];
        public List<int> SortedOffsets = new List<int>();

        public ImageOffsets()
        {
            ChunkName = "ImageOffsets";
            ChunkID = 0x5555;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Offsets = new int[reader.Size() / 4];
            for (int i = 0; i < Offsets.Length; i++)
                Offsets[i] = reader.ReadInt();

            SortedOffsets = Offsets.ToList();
            SortedOffsets.RemoveAll(x => x == 0);
            SortedOffsets.Sort();

            NebulaCore.PackageData.ImageOffsets = this;
            if (NebulaCore.Fusion == 1.5f)
            {
                ImageBank bnk = NebulaCore.PackageData.ImageBank;
                ByteReader bnkReader = new ByteReader(bnk.ChunkData);
                int count = bnk.ImageCount;
                if (Offsets.Length > 0)
                    count = Offsets.Length;
                for (int i = 0; i < count; i++)
                {
                    Image img = Image.NewImage();
                    if (Offsets.Length > 0 && Offsets[i] == 0)
                        continue;
                    if (Offsets.Length > 0)
                        bnkReader.Seek(Offsets[i] - 4);
                    img.ReadCCN(bnkReader);
                    bnk[img.Handle] = img;
                }

                foreach (Task task in ImageBank.TaskManager)
                    task.Wait();
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
