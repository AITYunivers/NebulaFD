using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class BasicCounterData : CommonObjectData
    {
        public uint PlayerHandle;
        public List<uint> ImageHandles = [];
        public bool UseText;
        public Color Color = Color.White;
        public uint FontHandle;
        public int Width;
        public int Height;

        public override void ReadUncommonData(ByteReader reader)
        {
            PlayerHandle = reader.ReadUInt();
            ImageHandles = [.. reader.ReadUInts(reader.ReadInt())];
            UseText = reader.ReadBool4();
            Color = reader.ReadColor();
            FontHandle = reader.ReadUInt();

            Width = reader.ReadInt();
            Height = reader.ReadInt();
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteUInt(PlayerHandle);
            writer.WriteInt(ImageHandles.Count);
            writer.WriteUInts(ImageHandles);
            writer.WriteBool4(UseText);
            writer.WriteColor(Color);
            writer.WriteUInt(FontHandle);

            writer.WriteInt(Width);
            writer.WriteInt(Height);
        }
    }
}
