using FusionRipper.Core.Data.Chunks.FrameChunks;
using FusionRipper.Core.FileReaders;
using FusionRipper.Core.Memory;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

#pragma warning disable CA1416 // Validate platform compatibility
namespace FusionRipper.Core.Data.Chunks.BankChunks.Images
{
    public class ImageOpen : Image
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

            int MosaicHandle = FrameMosaicTable.Handles[(int)Handle];
            if (MosaicHandle != 0)
            {
                Bitmap Mosaic = new Bitmap(Bitmap.FromFile("Temp\\M" + MosaicHandle + ".png"));
                short posX = FrameMosaicTable.PosXs[(int)Handle];
                short posY = FrameMosaicTable.PosYs[(int)Handle];
                Rectangle MosaicBounds = new Rectangle(posX,
                                                       posY,
                                                       Math.Min(Width, Mosaic.Width - posX),
                                                       Math.Min(Height, Mosaic.Height - posY));
                Mosaic = Mosaic.Clone(MosaicBounds, Mosaic.PixelFormat);

                if (posX + Width > Mosaic.Width || posY + Height > Mosaic.Height)
                {
                    Bitmap newMosaic = new Bitmap(Width, Height);
                    var graph = Graphics.FromImage(newMosaic);
                    graph.DrawImage(Mosaic, 0, 0, Mosaic.Width, Mosaic.Height);
                    graph.Dispose();
                    Mosaic = newMosaic;
                }

                FromBitmap(Mosaic);
                Mosaic.Dispose();
            }
            else
            {
                Bitmap Image = new Bitmap(Bitmap.FromFile("Temp\\I" + Handle + ".png"));
                FromBitmap(Image);
                Image.Dispose();
            }

            Flags.Value = 16;
            ImageBank.LoadedImageCount++;
        }
    }
}
