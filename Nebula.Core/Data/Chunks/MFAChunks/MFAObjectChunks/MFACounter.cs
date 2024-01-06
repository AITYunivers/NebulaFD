using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFACounter : MFAObjectLoader
    {
        public int Value;
        public int Minimum;
        public int Maximum;
        public uint DisplayType;
        /* Display Type Values:
         * 0 - Hidden
         * 1 - Numbers
         * 2 - Vertical Bar
         * 3 - Horizontal Bar
         * 4 - Animation
         * 5 - Text
         */
        public int FillType;
        /* Fill Type Values:
         * 1 - Solid Color
         * 2 - Gradient
         */
        public Color Color1;
        public Color Color2;
        public bool VerticalGradient;
        public int BarDirection;
        /* Bar Direction Values:
         * 0 - Down/Left
         * 1 - Up/Right
         */
        public int Width;
        public int Height;
        public uint[] Images = new uint[0];
        public uint Font;

        public MFACounter()
        {
            ChunkName = "MFACounter";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Value = reader.ReadInt();
            Minimum = reader.ReadInt();
            Maximum = reader.ReadInt();
            DisplayType = reader.ReadUInt();
            FillType = reader.ReadInt();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            VerticalGradient = reader.ReadInt() != 0;
            BarDirection = reader.ReadInt();
            Width = reader.ReadInt();
            Height = reader.ReadInt();

            Images = new uint[reader.ReadUInt()];
            for (int i = 0; i < Images.Length; i++)
                Images[i] = reader.ReadUInt();

            Font = reader.ReadUInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteInt(Value);
            writer.WriteInt(Minimum);
            writer.WriteInt(Maximum);
            writer.WriteUInt(DisplayType);
            writer.WriteInt(FillType);
            writer.WriteColor(Color1);
            writer.WriteColor(Color2);
            writer.WriteInt(VerticalGradient ? 1 : 0);
            writer.WriteInt(BarDirection);
            writer.WriteInt(Width);
            writer.WriteInt(Height);

            writer.WriteInt(Images.Length);
            foreach (uint image in Images)
                writer.WriteUInt(image);

            writer.WriteUInt(Font);
        }
    }
}
