#define Joveler
#if Joveler
using Joveler.Compression.ZLib;
#endif

using System.IO.Compression;
using DeflateStream = System.IO.Compression.DeflateStream;
using ZLibStream = System.IO.Compression.ZLibStream;

namespace Nebula.Core.Memory
{
    public static class Decompressor
    {
        public static ByteWriter Compress(byte[] buffer)
        {
            var writer = new ByteWriter(new MemoryStream());
            var compressed = CompressBlock(buffer);
            writer.WriteInt(buffer.Length);
            writer.WriteInt(compressed.Length);
            writer.WriteBytes(compressed);
            return writer;
        }

        public static byte[] Decompress(ByteReader exeReader, out int decompressed)
        {
            var decompSize = exeReader.ReadInt();
            var compSize = exeReader.ReadInt();
            decompressed = decompSize;
            return DecompressBlock(exeReader, compSize);
        }

        public static byte[] DecompressOPF(ByteReader exeReader, out int decompressed)
        {
            var decompSize = exeReader.ReadInt();
            decompressed = decompSize;
            TinyInflate.Tinflate(exeReader.ReadBytes(), out List<byte> output, new() { Anaconda = true }, out ulong CRC);
            return output.ToArray();
        }

        public static ByteReader DecompressAsReader(ByteReader exeReader, out int decompressed)
        {
            return new ByteReader(Decompress(exeReader, out decompressed));
        }

        public static byte[] DecompressBlock(ByteReader reader, int size)
        {
            return DecompressBlock(reader.ReadBytes(size));
        }

        public static byte[] DecompressOPFBlock(byte[] data)
        {
            TinyInflate.Error err = TinyInflate.Tinflate(data, out List<byte> output, new() { Anaconda = true }, out ulong CRC);
            if (err != TinyInflate.Error.OK)
                throw new Exception(TinyInflate.GetErrorName(err));
            return output.ToArray();
        }

        public static byte[] DecompressBlock(byte[] data)
        {
            MemoryStream inputStream = new MemoryStream(data);
            Stream deflateStream = IsZlib(data) 
                ? new ZLibStream(inputStream, CompressionMode.Decompress)
                : new DeflateStream(inputStream, CompressionMode.Decompress);
            MemoryStream outputStream = new MemoryStream();
            deflateStream.CopyTo(outputStream);
            byte[] outputData = outputStream.ToArray();
            outputStream.Dispose();
            deflateStream.Dispose();
            inputStream.Dispose();
            return outputData;
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

        public static byte[] CompressBlock(byte[] data)
        {
#if Joveler
            var compOpts = new ZLibCompressOptions();
            compOpts.Level = ZLibCompLevel.Default;
            var decompressedStream = new MemoryStream(data);
            var compressedStream = new MemoryStream();
            var zs = new Joveler.Compression.ZLib.ZLibStream(compressedStream, compOpts);
            decompressedStream.CopyTo(zs);
            zs.Close();
            return compressedStream.ToArray();
#else
            // Zlib header (2 bytes): 0x78, 0x9C
            byte[] zlibHeader = new byte[] { 0x78, 0x9C };

            // Calculate Adler-32 checksum of the uncompressed data
            uint adler32Checksum = Adler32Checksum(data);

            using (var outputStream = new MemoryStream())
            {
                // Write the Zlib header
                outputStream.Write(zlibHeader, 0, zlibHeader.Length);

                // Compress the data using DeflateStream
                using (var deflateStream = new DeflateStream(outputStream, CompressionMode.Compress, true))
                {
                    deflateStream.Write(data, 0, data.Length);
                }

                // Write the Adler-32 checksum (big-endian)
                byte[] adler32Bytes = BitConverter.GetBytes(adler32Checksum);
                outputStream.Write(adler32Bytes, 0, adler32Bytes.Length);

                return outputStream.ToArray();
            }
#endif
        }

        private static uint Adler32Checksum(byte[] data)
        {
            const uint ModAdler = 65521;
            uint a = 1, b = 0;

            foreach (byte c in data)
            {
                a = (a + c) % ModAdler;
                b = (b + a) % ModAdler;
            }

            return (b << 16) | a;
        }
    }
}