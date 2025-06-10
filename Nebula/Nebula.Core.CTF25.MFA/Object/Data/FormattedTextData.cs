using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class FormattedTextData : CommonObjectData
    {
        public int Width;
        public int Height;
        public uint Flags;
        public Color Color;
        public byte[] Data = [];

        public override void ReadUncommonData(ByteReader reader)
        {
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Flags = reader.ReadUInt();
            Color = reader.ReadColor();
            Data = reader.ReadBytes(reader.ReadInt());
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteUInt(Flags);
            writer.WriteColor(Color);
            writer.WriteBytes(Data, true);
        }
    }
}
