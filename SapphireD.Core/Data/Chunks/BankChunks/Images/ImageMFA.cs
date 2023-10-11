using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class ImageMFA : Image
    {
        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();
            if (SapDCore.Build >= 284)
                Handle--;

            Checksum = reader.ReadInt();
            References = reader.ReadInt();
            var dataSize = reader.ReadInt32();
            var newReader = new ByteReader(reader.ReadBytes(dataSize + 20));
            Width = newReader.ReadInt16();
            Height = newReader.ReadInt16();
            GraphicMode = newReader.ReadByte();
            Flags.Value = newReader.ReadByte();
            newReader.ReadInt16();
            HotspotX = newReader.ReadInt16();
            HotspotX = newReader.ReadInt16();
            ActionPointX = newReader.ReadInt16();
            ActionPointY = newReader.ReadInt16();
            TransparentColor = newReader.ReadColor();
            if (Flags["LZX"])
            {
                var decompressedSize = newReader.ReadInt32();
                ImageData = Decompressor.DecompressBlock(newReader, (int)(newReader.Size() - newReader.Tell()));
            }
            else ImageData = newReader.ReadBytes(dataSize);

            ImageBank.LoadedImageCount++;
        }
    }
}
