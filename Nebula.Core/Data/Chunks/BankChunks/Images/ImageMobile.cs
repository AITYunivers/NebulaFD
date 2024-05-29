using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class ImageMobile : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUShort();
            GraphicMode = (byte)reader.ReadInt();
            Width = reader.ReadShort();
            Height = reader.ReadShort();
            HotspotX = reader.ReadShort();
            HotspotY = reader.ReadShort();
            ActionPointX = reader.ReadShort();
            ActionPointY = reader.ReadShort();
            Flags.Value = 16;

            var size = reader.ReadInt();
            if (reader.PeekByte() == 255)
                ImageData = reader.ReadBytes(size);
            else
                ImageData = Decompressor.DecompressBlock(reader, size);
            ImageBank.LoadedImageCount++;
        }
    }
}
