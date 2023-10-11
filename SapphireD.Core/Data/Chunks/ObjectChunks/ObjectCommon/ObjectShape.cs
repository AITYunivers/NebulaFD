using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectShape : Chunk
    {
        public short BorderSize;
        public Color BorderColor = Color.White;
        public short ShapeType;
        public short FillType;
        public short LineFlags;
        public Color Color1 = Color.White;
        public Color Color2 = Color.White;
        public short GradientFlags;
        public short Image;

        public ObjectShape()
        {
            ChunkName = "ObjectShape";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            BorderSize = reader.ReadShort();
            BorderColor = reader.ReadColor();
            ShapeType = reader.ReadShort();
            FillType = reader.ReadShort();

            if (ShapeType == 1)
                LineFlags = reader.ReadShort();
            else if (FillType == 1)
                Color1 = reader.ReadColor();
            else if (FillType == 2)
            {
                Color1 = reader.ReadColor();
                Color2 = reader.ReadColor();
                GradientFlags = reader.ReadShort();
            }

            Image = reader.ReadShort();
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
