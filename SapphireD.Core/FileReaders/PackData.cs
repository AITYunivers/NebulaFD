using SapphireD.Core.Memory;

namespace SapphireD.Core.FileReaders
{
    public class PackData
    {
        public PackFile[] Items = new PackFile[0];

        public void Read(ByteReader reader)
        {
            if (reader.PeekInt() == 2004318071)
                reader.Skip(28);        // Multimedia Fusion 2 or above
            else if (reader.PeekInt() == 32639)
            {
                SapDCore.Fusion = 1.5f; // Multimedia Fusion 1.5
                SapDCore._unicode = false;
            }
            else if (reader.PeekShort() == 1)
            {
                SapDCore.Fusion = 1.1f; // The Games Factory
                SapDCore._unicode = false;
            }

            if (SapDCore.Fusion > 1.5f)
            {
                Items = new PackFile[reader.ReadUInt32()];
                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i] = new PackFile();
                    Items[i].Read(reader);
                }
            }
            else if (SapDCore.Fusion > 1.1f)
            {
                List<PackFile> itemList = new();
                while (true)
                {
                    int fileType = reader.ReadInt();
                    if (fileType == 32639)
                    {
                        reader.Skip(4);
                        break;
                    }
                    PackFile file = new PackFile();
                    file.Read(reader);
                    itemList.Add(file);
                }
                Items = itemList.ToArray();
            }
            else
            {
                List<PackFile> itemList = new();
                while (true)
                {
                    if (!reader.HasMemory(6))
                        break;
                    reader.Skip(2);
                    PackFile file = new PackFile();
                    file.Read(reader);
                    itemList.Add(file);
                }
                Items = itemList.ToArray();
            }
        }
    }
}
