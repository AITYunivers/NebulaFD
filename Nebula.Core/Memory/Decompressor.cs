#define Joveler
#if Ionic
using Ionic.Zlib;
#endif
#if Joveler
using Joveler.Compression.ZLib;
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

        /*public static byte[] DecompressOPF(ByteReader exeReader, out int decompressed)
        {
            uint decompressed_size = exeReader.ReadUInt();
            uint saved_size = decompressed_size;
            byte[] buf = new byte[decompressed_size];
            long start = exeReader.Tell();
            byte[] data = exeReader.ReadBytes();
            byte[] new_data = buf;
            //ulong crc_ret = 0;
            long bytesread = 0;
            bytesread = TinyInflateOLD.TinyInflateOLD.tinf_uncompress(buf, ref decompressed_size, data, (uint)data.Length);
            //bytesread = TinyInflate.tinflate(data, data.Length, ref new_data, decompressed_size, ref crc_ret);
            exeReader.Seek(start + bytesread);
            if (decompressed_size != saved_size)
                throw new Exception($"Decompression failed ({saved_size}, {decompressed_size})");

            decompressed = (int)decompressed_size;
            return new_data;
        }*/

        public static ByteReader DecompressAsReader(ByteReader exeReader, out int decompressed)
        {
            return new ByteReader(Decompress(exeReader, out decompressed));
        }

        public static byte[] DecompressBlock(ByteReader reader, int size)
        {
            return DecompressBlock(reader.ReadBytes(size));
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

        /*public static byte[] DecompressOld(ByteReader reader)
        {
            var decompressedSize = reader.PeekInt32() != -1 ? reader.ReadInt32() : 0;
            var start = reader.Tell();
            var compressedSize = reader.Size();
            var buffer = reader.ReadBytes((int)compressedSize);
            int actualSize;
            var data = DecompressOldBlock(buffer, (int)compressedSize, decompressedSize, out actualSize);
            reader.Seek(start + actualSize);
            return data;
        }

        public static byte[] DecompressOldBlock(byte[] buff, int size, int decompSize, out int actual_size)
        {
            var originalBuff = Marshal.AllocHGlobal(size);
            Marshal.Copy(buff, 0, originalBuff, buff.Length);
            var outputBuff = Marshal.AllocHGlobal(decompSize);
            actual_size = NativeLib.decompressOld(originalBuff, size, outputBuff, decompSize);
            Marshal.FreeHGlobal(originalBuff);
            var data = new byte[decompSize];
            Marshal.Copy(outputBuff, data, 0, decompSize);
            Marshal.FreeHGlobal(outputBuff);
            return data;
        }*/

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