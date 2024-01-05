using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class ImageMobile : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUShort();
            GraphicMode = (byte)reader.ReadInt();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            HotspotX = reader.ReadInt16();
            HotspotY = reader.ReadInt16();
            ActionPointX = reader.ReadInt16();
            ActionPointY = reader.ReadInt16();
            Flags.Value = 16;

            var size = reader.ReadInt32();
            if (reader.PeekByte() == 255)
                ImageData = reader.ReadBytes(size);
            else
                ImageData = Decompressor.DecompressBlock(reader, size);
            ImageBank.LoadedImageCount++;
        }
    }
}
