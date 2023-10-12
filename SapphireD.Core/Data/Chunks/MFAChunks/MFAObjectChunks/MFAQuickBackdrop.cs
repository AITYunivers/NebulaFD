using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAQuickBackdrop : MFAObjectLoader
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public uint ObstacleType;
        public uint CollisionType;
        public int Width;
        public int Height;
        public int Shape;
        public int BorderSize;
        public Color BorderColor;
        public int FillType;
        public Color Color1;
        public Color Color2;
        public int Image;

        public MFAQuickBackdrop()
        {
            ChunkName = "MFAQuickBackdrop";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ObstacleType = reader.ReadUInt();
            CollisionType = reader.ReadUInt();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Shape = reader.ReadInt();
            BorderSize = reader.ReadInt();
            BorderColor = reader.ReadColor();
            FillType = reader.ReadInt();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            Flags.Value = reader.ReadUInt();
            Image = reader.ReadInt();
        }
    }
}
