using SapphireD.Core.Memory;

namespace SapphireD.Core.FileReaders
{
    public class PackData
    {
        public PackFile[]? Items;

        public void Read(ByteReader reader)
        {
            reader.Skip(28);
            Items = new PackFile[reader.ReadUInt32()];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new PackFile();
                Items[i].Read(reader);
            }
        }
    }
}
