using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class QuickBackdropData : IReadable, IWritable
    {
        public uint ObstacleType;
        public uint CollisionType;
        public int Width;
        public int Height;
        public uint ShapeHandle;
        public uint BorderSize;
        public Color BorderColor;
        public uint FillType;
        public Color Color1;
        public Color Color2;
        public uint Flags;
        public uint ImageHandle;

        public void Read(ByteReader reader)
        {
            ObstacleType = reader.ReadUInt();
            CollisionType = reader.ReadUInt();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            ShapeHandle = reader.ReadUInt();
            BorderSize = reader.ReadUInt();
            BorderColor = reader.ReadColor();
            FillType = reader.ReadUInt();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            Flags = reader.ReadUInt();
            ImageHandle = reader.ReadUInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(ObstacleType);
            writer.WriteUInt(CollisionType);
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteUInt(ShapeHandle);
            writer.WriteUInt(BorderSize);
            writer.WriteColor(BorderColor);
            writer.WriteUInt(FillType);
            writer.WriteColor(Color1);
            writer.WriteColor(Color2);
            writer.WriteUInt(Flags);
            writer.WriteUInt(ImageHandle);
        }
    }
}
