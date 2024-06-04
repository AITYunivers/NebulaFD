#define Joveler
#if Ionic
using Ionic.Zlib;
#endif
#if Joveler
using Joveler.Compression.ZLib;
using System.Diagnostics;

#endif
#if !Ionic || !Joveler
using System.IO.Compression;
#endif

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
#if Ionic
            return ZlibStream.UncompressBuffer(data);
#else
            using (var inputStream = new MemoryStream(data, 2, data.Length - 6))
            {
                using (var deflateStream = new System.IO.Compression.DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        deflateStream.CopyTo(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
#endif
        }

        public static byte[] CompressBlock(byte[] data)
        {
#if Joveler
            var compOpts = new ZLibCompressOptions();
            compOpts.Level = ZLibCompLevel.Default;
            var decompressedStream = new MemoryStream(data);
            var compressedStream = new MemoryStream();
            byte[] compressedData = null;
            var zs = new Joveler.Compression.ZLib.ZLibStream(compressedStream, compOpts);
            decompressedStream.CopyTo(zs);
            zs.Close();

            compressedData = compressedStream.ToArray();

            return compressedData;
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