using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class ObjectShape : IReadable, IWritable
    {
        public short BorderSize;
        public Color BorderColor;
        public ushort ShapeType;
        public ushort FillType;
        public ushort LineFlags;
        public Color Color1;
        public Color Color2;
        public ushort GradientFlags;

        public void Read(ByteReader reader)
        {
            BorderSize = reader.ReadShort();
            BorderColor = reader.ReadColor();
            ShapeType = reader.ReadUShort();
            FillType = reader.ReadUShort();

            if (ShapeType == 1) // Line
                LineFlags = reader.ReadUShort();
            else if (FillType == 1) // Solid
                Color1 = reader.ReadColor();
            else if (FillType == 2) // Gradient
            {
                Color1 = reader.ReadColor();
                Color2 = reader.ReadColor();
                GradientFlags = reader.ReadUShort();
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteShort(BorderSize);
            writer.WriteColor(BorderColor);
            writer.WriteUShort(ShapeType);
            writer.WriteUShort(FillType);

            if (ShapeType == 1) // Line
                writer.WriteUShort(FillType);
            else if (FillType == 1) // Solid
                writer.WriteColor(Color1);
            else if (FillType == 2) // Gradient
            {
                writer.WriteColor(Color1);
                writer.WriteColor(Color2);
                writer.WriteUShort(GradientFlags);
            }
        }
    }
}
