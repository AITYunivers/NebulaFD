using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class ImageFlash : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUShort();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            HotspotX = reader.ReadInt16();
            HotspotY = reader.ReadInt16();
            ActionPointX = reader.ReadInt16();
            ActionPointY = reader.ReadInt16();

            var size = reader.ReadInt32();
            var compressedBuffer = reader.ReadBytes(size);
            GraphicMode = 9;
            ImageData = Decompressor.DecompressBlock(compressedBuffer);
        }
    }
}
