using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class AppIcon : Chunk
    {
        public Bitmap Icon = new(1, 1);
        public List<Color> Palette = new();

        public AppIcon()
        {
            ChunkName = "AppIcon";
            ChunkID = 0x2235;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Seek(reader.ReadInt());
            Palette = new();
            for (int i = 0; i < 16 * 16; i++)
            {
                var b = reader.ReadByte();
                var g = reader.ReadByte();
                var r = reader.ReadByte();
                var a = reader.ReadByte();
                var newColor = Color.FromArgb(255, r, g, b);
                Palette.Add(newColor);
            }

            Icon = new Bitmap(16, 16);

            for (int h = 0; h < Icon.Height; h++)
                for (int w = 0; w < Icon.Width; w++)
                    Icon.SetPixel(w, Icon.Width - 1 - h, Palette[reader.ReadByte()]);

            var BitmapSize = Icon.Width * Icon.Height;
            for (int y = 0; y < Icon.Height; ++y)
                for (int x = 0; x < Icon.Width; x += 8)
                {
                    byte Mask = reader.ReadByte();
                    for (int i = 0; i < 8; ++i)
                        if ((1 & (Mask >> (7 - i))) != 0)
                        {
                            Color get = Icon.GetPixel(x + i, y);
                            Icon.SetPixel(x + i, y, Color.FromArgb(0, get.R, get.G, get.B));
                        }
                        else
                        {
                            Color get = Icon.GetPixel(x + i, y);
                            Icon.SetPixel(x + i, y, Color.FromArgb(255, get.R, get.G, get.B));
                        }
                }

            NebulaCore.PackageData.AppIcon = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
