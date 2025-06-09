using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Paragraph
{
    internal class ParagraphItem : IReadable
    {
        public void Read(ByteReader reader)
        {
            string value = reader.ReadAutoYuniversal();
            uint flags = reader.ReadUInt();
        }
    }
}
