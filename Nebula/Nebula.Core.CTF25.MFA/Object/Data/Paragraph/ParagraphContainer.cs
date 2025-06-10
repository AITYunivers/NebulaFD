using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data.Paragraph
{
    internal class ParagraphContainer : IReadable, IWritable
    {
        public uint FontHandle;
        public Color Color = Color.White;
        public uint Flags;
        public bool Relief;
        public ParagraphBank Paragraphs = new ParagraphBank();

        public void Read(ByteReader reader)
        {
            FontHandle = reader.ReadUInt();
            Color = reader.ReadColor();
            Flags = reader.ReadUInt();
            Relief = reader.ReadBool4();
            Paragraphs.Read(reader);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(FontHandle);
            writer.WriteColor(Color);
            writer.WriteUInt(Flags);
            writer.WriteBool4(Relief);
            Paragraphs.Write(writer);
        }
    }
}
