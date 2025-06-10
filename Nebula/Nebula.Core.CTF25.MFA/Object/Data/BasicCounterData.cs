using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class BasicCounterData : CommonObjectData
    {
        public uint PlayerHandle;
        public uint[] ImageHandles = [];
        public bool UseText;
        public Color Color = Color.White;
        public uint FontHandle;
        public int Width;
        public int Height;

        public override void ReadUncommonData(ByteReader reader)
        {
            PlayerHandle = reader.ReadUInt();

            ImageHandles = new uint[reader.ReadInt()];
            for (int i = 0; i < ImageHandles.Length; i++)
                ImageHandles[i] = reader.ReadUInt();

            UseText = reader.ReadBool4();
            Color = reader.ReadColor();
            FontHandle = reader.ReadUInt();

            Width = reader.ReadInt();
            Height = reader.ReadInt();
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteUInt(PlayerHandle);

            writer.WriteInt(ImageHandles.Length);
            foreach (uint imageHandle in ImageHandles)
                writer.WriteUInt(imageHandle);

            writer.WriteBool4(UseText);
            writer.WriteColor(Color);
            writer.WriteUInt(FontHandle);

            writer.WriteInt(Width);
            writer.WriteInt(Height);
        }
    }
}
