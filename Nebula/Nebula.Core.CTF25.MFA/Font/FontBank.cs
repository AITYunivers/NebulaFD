using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Font
{
    public class FontBank : Collection<FontItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            string bankHeader = reader.ReadAscii(4);
            if (bankHeader != "ATNF")
                throw new InvalidDataException("Invalid font bank header. Expected ATNF, got " + bankHeader);

            int fontCount = reader.ReadInt();
            this.Log($"Found {fontCount} font(s)", Logger.LogType.Debug);
            if (fontCount < 0)
                throw new InvalidDataException("Invalid font count. Expected greater than or equal to 0, got " + fontCount);

            foreach (FontItem fontItem in reader.ReadIReadables<FontItem>(fontCount))
            {
                Add(fontItem);
                this.Log($"Font {fontItem.Handle}: {fontItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAscii("ATNF");
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
