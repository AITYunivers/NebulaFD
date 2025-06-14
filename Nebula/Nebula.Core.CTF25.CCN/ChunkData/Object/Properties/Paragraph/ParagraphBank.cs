using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Paragraph
{
    internal class ParagraphBank : Collection<ParagraphItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int paragraphCount = reader.ReadInt();
            this.Log($"Found {paragraphCount} paragraphs(s)", Logger.LogType.Debug);
            if (paragraphCount < 0)
                throw new InvalidDataException("Invalid paragraph count. Expected greater than or equal to 0, got " + paragraphCount);

            foreach (ParagraphItem paragraphItem in reader.ReadIReadables<ParagraphItem, int>(paragraphCount))
                Add(paragraphItem);

            for (int i = 0; i < paragraphCount; i++)
                this.Log($"Paragraph {i}: {this[i].Value}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
