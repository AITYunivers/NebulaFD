using SapphireD.Core.Memory;
using System.Diagnostics;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFACounter : MFAObjectLoader
    {
        public BitDict CounterFlags = new BitDict( // Counter Flags
            
        );

        public int Value;
        public int Minimum;
        public int Maximum;
        public uint DisplayType;
        public Color Color1;
        public Color Color2;
        public uint Gradient;
        public int ColorType;
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
            CounterFlags.Value = reader.ReadUInt();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            Gradient = reader.ReadUInt();
            ColorType = reader.ReadInt();
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
            writer.WriteUInt(CounterFlags.Value);
            writer.WriteColor(Color1);
            writer.WriteColor(Color2);
            writer.WriteUInt(Gradient);
            writer.WriteInt(ColorType);
            writer.WriteInt(Width);
            writer.WriteInt(Height);

            writer.WriteInt(Images.Length);
            foreach (uint image in Images)
                writer.WriteUInt(image);

            writer.WriteUInt(Font);
        }
    }
}
