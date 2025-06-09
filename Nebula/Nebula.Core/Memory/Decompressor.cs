using System.IO.Compression;

namespace Nebula.Core.Memory
{
    public static class Decompressor
    {
        public static byte[] DecompressZlib(byte[] compressedData)
        {
            return DecompressZlib(new ByteReader(compressedData), out int _);
        }

        public static byte[] DecompressZlib(ByteReader exeReader, out int decompressed)
        {
            var decompSize = exeReader.ReadInt();
            var compSize = exeReader.ReadInt();
            decompressed = decompSize;
            return DecompressZlib(exeReader, compSize);
        }

        public static byte[] DecompressZlib(ByteReader reader, int size)
        {
            return DecompressZlibRaw(reader.ReadBytes(size));
        }

        public static byte[] DecompressZlibRaw(byte[] data)
        {
            using MemoryStream inputStream = new MemoryStream(data);
            using Stream deflateStream = IsZlib(data) 
                ? new ZLibStream(inputStream, CompressionMode.Decompress)
                : new DeflateStream(inputStream, CompressionMode.Decompress);
            using MemoryStream outputStream = new MemoryStream();
            deflateStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }

        public static bool IsZlib(byte[] check)
        {
            if (check.Length < 2)
                return false;

            bool isZlib = check[0] == 0x78; // Zlib Header
            if (isZlib)
            {
                isZlib = false;
                isZlib |= check[1] == 0x01; // No Compression/Low Compression
                isZlib |= check[1] == 0x5E; // Fast Compression
                isZlib |= check[1] == 0x9C; // Default Compression
                isZlib |= check[1] == 0xDA; // Best Compression
            }
            return isZlib;
        }
    }
}