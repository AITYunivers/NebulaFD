using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectShape : Chunk
    {
        public BitDict LineFlags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict GradientFlags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int BorderSize;
        public Color BorderColor = Color.White;
        public int ShapeType;
        public int FillType;
        public Color Color1 = Color.White;
        public Color Color2 = Color.White;
        public int Image;

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
                LineFlags.Value = reader.ReadUShort();
            else if (FillType == 1)
                Color1 = reader.ReadColor();
            else if (FillType == 2)
            {
                Color1 = reader.ReadColor();
                Color2 = reader.ReadColor();
                GradientFlags.Value = reader.ReadUShort();
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
