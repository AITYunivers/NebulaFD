using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class ImageFlash : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUShort();
            Width = reader.ReadShort();
            Height = reader.ReadShort();
            HotspotX = reader.ReadShort();
            HotspotY = reader.ReadShort();
            ActionPointX = reader.ReadShort();
            ActionPointY = reader.ReadShort();

            var size = reader.ReadInt();
            var compressedBuffer = reader.ReadBytes(size);
            GraphicMode = 9;
            ImageData = Decompressor.DecompressBlock(compressedBuffer);
            ImageBank.LoadedImageCount++;
        }
    }
}
