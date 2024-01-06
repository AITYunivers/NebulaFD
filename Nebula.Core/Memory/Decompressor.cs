using Ionic.Zlib;
using Joveler.Compression.ZLib;

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
            int bytesread = TinyInflate.TinyInflateOLD.tinf_uncompress(buf, ref decompressed_size, data, (uint)data.Length);
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

        public static byte[] DecompressBlock(byte[] data)
        {
            return ZlibStream.UncompressBuffer(data);
        }

        public static byte[] DecompressBlock(ByteReader reader, int size)
        {
            return ZlibStream.UncompressBuffer(reader.ReadBytes(size));
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
            var compOpts = new ZLibCompressOptions();
            compOpts.Level = ZLibCompLevel.Default;
            var decompressedStream = new MemoryStream(data);
            var compressedStream = new MemoryStream();
            byte[] compressedData = null;
            var zs = new ZLibStream(compressedStream, compOpts);
            decompressedStream.CopyTo(zs);
            zs.Close();

            compressedData = compressedStream.ToArray();

            return compressedData;
        }
    }
}