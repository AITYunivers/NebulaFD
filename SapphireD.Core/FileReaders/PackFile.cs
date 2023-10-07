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

        public void Read(ByteReader exeReader)
        {
            PackFilename = exeReader.ReadYuniversal(exeReader.ReadUInt16());
            exeReader.Skip(4);
            DataSize = exeReader.ReadInt32();

            if (exeReader.PeekInt16() == -9608)
            {
                Data = Decompressor.DecompressBlock(exeReader, DataSize);
                Compressed = true;
            }
            else Data = exeReader.ReadBytes(DataSize);

            Logger.Log(this, $"New packfile: {PackFilename}" + (Compressed ? " (Compressed)" : ""));
        }
    }
}