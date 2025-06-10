using Nebula.Core.CTF25.MFA.Object.Data.Paragraph;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class StringData : CommonObjectData
    {
        public int Width;
        public int Height;
        public ParagraphContainer Paragraphs = new ParagraphContainer();

        public override void ReadUncommonData(ByteReader reader)
        {
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Paragraphs.Read(reader);
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            Paragraphs.Write(writer);
        }
    }
}
