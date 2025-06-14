using Nebula.Core.CTF25.MFA.Extension;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Collections;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data.Paragraph
{
    internal class ParagraphContainer : IReadable, IWritable, ICollection<ParagraphItem>
    {
        public uint FontHandle;
        public Color Color = Color.White;
        public uint Flags;
        public bool Relief;
        public ParagraphBank Paragraphs = [];

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

        public int Count => Paragraphs.Count;
        public bool IsReadOnly => false;
        public void Add(ParagraphItem item) => Paragraphs.Add(item);
        public void Clear() => Paragraphs.Clear();
        public bool Contains(ParagraphItem item) => Paragraphs.Contains(item);
        public void CopyTo(ParagraphItem[] array, int arrayIndex) => Paragraphs.CopyTo(array, arrayIndex);
        public IEnumerator<ParagraphItem> GetEnumerator() => Paragraphs.GetEnumerator();
        public bool Remove(ParagraphItem item) => Paragraphs.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ParagraphItem this[int index]
        {
            get => Paragraphs[index];
            set => Paragraphs[index] = value;
        }
    }
}
