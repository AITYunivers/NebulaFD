using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.FileReaders
{
    public class PackFile
    {
        public string? PackFilename;
        public byte[]? Data;
        public int DataSize;
        public bool Compressed;

        public void Read(ByteReader reader)
        {
            if (SapDCore.Fusion > 1.5f)
            {
                short len = reader.ReadShort();
                PackFilename = reader.ReadYuniversal(len);
                DataSize = reader.ReadInt();
                DataSize = reader.ReadInt();
            }
            else
            {
                DataSize = reader.ReadInt();
                PackFilename = reader.ReadYuniversal();
                DataSize -= PackFilename.Length + 1;
            }

            if (reader.PeekInt16() == -9608)
            {
                Data = Decompressor.DecompressBlock(reader, DataSize);
                Compressed = true;
            }
            else Data = reader.ReadBytes(DataSize);

            Logger.Log(this, $"New packfile: {PackFilename}" + (Compressed ? " (Compressed)" : ""));
        }
    }
}