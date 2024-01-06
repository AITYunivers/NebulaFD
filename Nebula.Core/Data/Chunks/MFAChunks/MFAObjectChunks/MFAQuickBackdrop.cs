using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAQuickBackdrop : MFAObjectLoader
    {
        public BitDict QuickBkdFlags = new BitDict( // Quick Backdrop Flags
            "VerticalGradient",  // Vertical Gradient
            "IntegralDimensions" // Integral Dimensions
        );

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
        public uint Image;

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
            QuickBkdFlags.Value = reader.ReadUInt();
            Image = reader.ReadUInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUInt(ObstacleType);
            writer.WriteUInt(CollisionType);
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteInt(Shape);
            writer.WriteInt(BorderSize);
            writer.WriteColor(BorderColor);
            writer.WriteInt(FillType);
            writer.WriteColor(Color1);
            writer.WriteColor(Color2);
            writer.WriteUInt(QuickBkdFlags.Value);
            writer.WriteUInt(Image);
        }
    }
}
