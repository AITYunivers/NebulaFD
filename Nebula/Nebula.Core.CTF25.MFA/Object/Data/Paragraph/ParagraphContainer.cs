using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data.Paragraph
{
    internal class ParagraphContainer : IReadable
    {
        public void Read(ByteReader reader)
        {
            uint fontHandle = reader.ReadUInt();
            Color color = reader.ReadColor();
            uint flags = reader.ReadUInt();
            bool relief = reader.ReadBool4();

            ParagraphBank paragraphBank = new ParagraphBank();
            paragraphBank.Read(reader);
        }
    }
}
