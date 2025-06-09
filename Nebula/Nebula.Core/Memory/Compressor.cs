using System.IO.Compression;

namespace Nebula.Core.Memory
{
    public static class Compressor
    {
        public static ByteWriter CompressZlib(byte[] buffer)
        {
            var writer = new ByteWriter(new MemoryStream());
            var compressed = CompressZlibRaw(buffer);
            writer.WriteInt(buffer.Length);
            writer.WriteInt(compressed.Length);
            writer.WriteBytes(compressed);
            return writer;
        }

        public static byte[] CompressZlibRaw(byte[] data)
        {
            using MemoryStream inputStream = new MemoryStream(data);
            using ZLibStream deflateStream = new ZLibStream(inputStream, CompressionMode.Compress, true);
            using MemoryStream outputStream = new MemoryStream();
            deflateStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}
