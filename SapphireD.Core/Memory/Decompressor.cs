using Ionic.Zlib;
using Joveler.Compression.ZLib;

namespace SapphireD.Core.Memory
{
    public static class Decompressor
    {
        public static ByteWriter Compress(byte[] buffer)
        {
            var writer = new ByteWriter(new MemoryStream());
            var compressed = CompressBlock(buffer);
            writer.WriteInt32(buffer.Length);
            writer.WriteInt32(compressed.Length);
            writer.WriteBytes(compressed);
            return writer;
        }

        public static byte[] Decompress(ByteReader exeReader, out int decompressed)
        {
            var decompSize = exeReader.ReadInt32();
            var compSize = exeReader.ReadInt32();
            decompressed = decompSize;
            return DecompressBlock(exeReader, compSize);
        }

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