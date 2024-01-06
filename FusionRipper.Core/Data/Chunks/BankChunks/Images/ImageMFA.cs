using FusionRipper.Core.Memory;
using System.Diagnostics;

namespace FusionRipper.Core.Data.Chunks.BankChunks.Images
{
    public class ImageMFA : Image
    {
        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();
            if (FRipCore.Build < 284)
                Handle++;

            Checksum = reader.ReadInt();
            References = reader.ReadInt();
            var dataSize = reader.ReadInt();
            var newReader = new ByteReader(reader.ReadBytes(dataSize + 20));
            Width = newReader.ReadShort();
            Height = newReader.ReadShort();
            GraphicMode = newReader.ReadByte();
            Flags.Value = newReader.ReadByte();
            newReader.ReadShort();
            HotspotX = newReader.ReadShort();
            HotspotY = newReader.ReadShort();
            ActionPointX = newReader.ReadShort();
            ActionPointY = newReader.ReadShort();
            TransparentColor = newReader.ReadColor();
            if (Flags["LZX"])
            {
                var decompressedSize = newReader.ReadInt();
                ImageData = Decompressor.DecompressBlock(newReader, (int)(newReader.Size() - newReader.Tell()));
            }
            else ImageData = newReader.ReadBytes(dataSize);

            ImageBank.LoadedImageCount++;
        }
    }
}
