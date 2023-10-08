using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class Image25 : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();
            if (SapDCore.Build >= 284)
                Handle--;

            var decompressedSize = reader.ReadInt32();
            var compSize = reader.ReadInt32();
            var compressedBuffer = reader.ReadBytes(compSize);
            var decompressedReader = new ByteReader(Decompressor.DecompressBlock(compressedBuffer));
            Checksum = decompressedReader.ReadInt();
            References = decompressedReader.ReadInt();
            var dataSize = decompressedReader.ReadInt32();
            Width = decompressedReader.ReadInt16();
            Height = decompressedReader.ReadInt16();
            GraphicMode = decompressedReader.ReadByte();
            Flags.Value = decompressedReader.ReadByte();
            decompressedReader.ReadInt16();
            HotspotX = decompressedReader.ReadInt16();
            HotspotX = decompressedReader.ReadInt16();
            ActionPointX = decompressedReader.ReadInt16();
            ActionPointY = decompressedReader.ReadInt16();
            TransparentColor = decompressedReader.ReadColor();
            if (Flags["LZX"])
            {
                decompressedSize = decompressedReader.ReadInt32();
                ImageData = Decompressor.DecompressBlock(decompressedReader,
                    (int)(decompressedReader.Size() - decompressedReader.Tell()));
            }
            else
                ImageData = decompressedReader.ReadBytes(dataSize);
        }
    }
}
