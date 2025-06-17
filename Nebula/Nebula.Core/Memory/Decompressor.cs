using System.IO.Compression;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nebula.Core.Memory
{
    public static class Decompressor
    {
        public static byte[] DecompressZlib(byte[] compressedData)
        {
            using ByteReader reader = new ByteReader(compressedData);
            return DecompressZlib(reader, out int _);
        }

        public static byte[] DecompressZlib(ByteReader exeReader, out int decompressed)
        {
            int decompSize = exeReader.ReadInt();
            int compSize = exeReader.ReadInt();
            decompressed = decompSize;
            return DecompressZlib(exeReader, compSize);
        }

        public static byte[] DecompressZlib(ByteReader reader, int size)
        {
            return DecompressZlibRaw(reader.ReadBytes(size));
        }

        public static void DecompressZlib(ByteReader reader, byte[] result, int length)
        {
            byte[] header = reader.ReadBytes(2);
            reader.Skip(-2); // Go back
            DecompressZlib(reader.BaseStream, result, length, IsZlib(header));
        }

        public static void DecompressZlib(Stream stream, byte[] result, int length, bool isZlib)
        {
            using Stream deflateStream = isZlib
                ? new ZLibStream(stream, CompressionMode.Decompress, true)
                : new DeflateStream(stream, CompressionMode.Decompress, true);
            deflateStream.ReadExactly(result, 0, length);
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