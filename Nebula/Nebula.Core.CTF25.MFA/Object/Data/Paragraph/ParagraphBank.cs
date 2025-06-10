using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Object.Data.Paragraph
{
    internal class ParagraphBank : IReadable, IWritable
    {
        private ParagraphItem[] _paragraphItems = [];

        public void Read(ByteReader reader)
        {
            int paragraphCount = reader.ReadInt();
            this.Log($"Found {paragraphCount} paragraphs(s)", Logger.LogType.Debug);
            if (paragraphCount < 0)
                throw new InvalidDataException("Invalid paragraph count. Expected greater than or equal to 0, got " + paragraphCount);
        
            _paragraphItems = new ParagraphItem[paragraphCount];
            for (int i = 0; i < paragraphCount; i++)
            {
                ParagraphItem paragraphItem = new ParagraphItem();
                paragraphItem.Read(reader);
                _paragraphItems[i] = paragraphItem;
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_paragraphItems.Length);
            foreach (ParagraphItem paragraphItem in _paragraphItems)
                paragraphItem.Write(writer);
        }

        public ParagraphItem this[int index]
        {
            get => _paragraphItems[index];
            set => _paragraphItems[index] = value;
        }
    }
}
