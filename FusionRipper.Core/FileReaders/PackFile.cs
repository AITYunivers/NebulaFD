using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;

namespace FusionRipper.Core.FileReaders
{
    public class PackFile
    {
        public string PackFilename = string.Empty;
        public byte[] Data = new byte[0];
        public int DataSize;
        public bool Compressed;

        public void Read(ByteReader reader)
        {
            if (FRipCore.Fusion > 1.5f)
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

            if (reader.PeekShort() == -9608)
            {
                Data = Decompressor.DecompressBlock(reader, DataSize);
                Compressed = true;
            }
            else Data = reader.ReadBytes(DataSize);

            Logger.Log(this, $"New packfile: {PackFilename}" + (Compressed ? " (Compressed)" : ""));
        }
    }
}