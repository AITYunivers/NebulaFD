using Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Paragraph;
using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class StringData : IReadable, IWritable
    {
        public int Width;
        public int Height;
        public ParagraphBank Paragraphs = new ParagraphBank();

        public void Read(ByteReader reader)
        {
            reader.Skip(4); // Size?
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Paragraphs.Read(reader);
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
