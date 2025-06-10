using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Paragraph
{
    internal class ParagraphItem : IReadable, IWritable
    {
        public string Value = string.Empty;
        public uint Flags = 0;

        public void Read(ByteReader reader)
        {
            Value = reader.ReadAutoYuniversal();
            Flags = reader.ReadUInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Value);
            writer.WriteUInt(Flags);
        }
    }
}
