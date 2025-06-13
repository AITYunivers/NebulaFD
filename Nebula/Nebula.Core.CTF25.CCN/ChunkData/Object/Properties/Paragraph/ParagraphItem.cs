using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Paragraph
{
    internal class ParagraphItem : IReadable, IWritable
    {
        public ushort FontHandle;
        public ushort Flags;
        public Color Color;
        public string Value = string.Empty;

        public void Read(ByteReader reader)
        {
            FontHandle = reader.ReadUShort();
            Flags = reader.ReadUShort();
            Color = reader.ReadColor();
            Value = reader.ReadYuniversal();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUShort(FontHandle);
            writer.WriteUShort(Flags);
            writer.WriteColor(Color);
            writer.WriteYunicode(Value);
        }
    }
}
