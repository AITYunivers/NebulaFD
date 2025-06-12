using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Font
{
    public class FontBank : IReadable, IWritable
    {
        private FontItem[] _fontItems = [];

        public void Read(ByteReader reader)
        {
            string bankHeader = reader.ReadAscii(4);
            if (bankHeader != "ATNF")
                throw new InvalidDataException("Invalid font bank header. Expected ATNF, got " + bankHeader);

            int fontCount = reader.ReadInt();
            this.Log($"Found {fontCount} font(s)", Logger.LogType.Debug);
            if (fontCount < 0)
                throw new InvalidDataException("Invalid font count. Expected greater than or equal to 0, got " + fontCount);

            _fontItems = reader.ReadIReadables<FontItem>(fontCount);

            foreach (FontItem fontItem in _fontItems)
                this.Log($"Font {fontItem.Handle}: {fontItem.Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAscii("ATNF");
            writer.WriteInt(_fontItems.Length);
            writer.WriteIWritables(_fontItems);
        }

        public FontItem this[int index]
        {
            get => _fontItems[index];
            set => _fontItems[index] = value;
        }
    }
}
