using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Paragraph
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
        
            _paragraphItems = reader.ReadIReadables<ParagraphItem, int>(paragraphCount);

            for (int i = 0; i < paragraphCount; i++)
                this.Log($"Paragraph {i}: {_paragraphItems[i].Value}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_paragraphItems.Length);
            writer.WriteIWritables(_paragraphItems);
        }

        public ParagraphItem this[int index]
        {
            get => _paragraphItems[index];
            set => _paragraphItems[index] = value;
        }
    }
}
