using Nebula.Core.Memory;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class CounterData : CommonObjectData
    {
        public int DefaultValue;
        public int MinValue;
        public int MaxValue;
        public uint DisplayType;
        public uint FillType;
        public Color Color1 = Color.White;
        public Color Color2 = Color.White;
        public bool VerticalGradient;
        public uint BarDirection;
        public int Width;
        public int Height;
        public uint[] ImageHandles = [];
        public uint FontHandle;

        public override void ReadUncommonData(ByteReader reader)
        {
            DefaultValue = reader.ReadInt();
            MinValue = reader.ReadInt();
            MaxValue = reader.ReadInt();
            DisplayType = reader.ReadUInt();
            FillType = reader.ReadUInt();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            VerticalGradient = reader.ReadBool4();
            BarDirection = reader.ReadUInt();

            Width = reader.ReadInt();
            Height = reader.ReadInt();

            ImageHandles = new uint[reader.ReadInt()];
            for (int i = 0; i < ImageHandles.Length; i++)
                ImageHandles[i] = reader.ReadUInt();

            FontHandle = reader.ReadUInt();
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteInt(DefaultValue);
            writer.WriteInt(MinValue);
            writer.WriteInt(MaxValue);
            writer.WriteUInt(DisplayType);
            writer.WriteUInt(FillType);
            writer.WriteColor(Color1);
            writer.WriteColor(Color2);
            writer.WriteBool4(VerticalGradient);
            writer.WriteUInt(BarDirection);

            writer.WriteInt(Width);
            writer.WriteInt(Height);

            writer.WriteInt(ImageHandles.Length);
            foreach (uint imageHandle in ImageHandles)
                writer.WriteUInt(imageHandle);

            writer.WriteUInt(FontHandle);
        }
    }
}
