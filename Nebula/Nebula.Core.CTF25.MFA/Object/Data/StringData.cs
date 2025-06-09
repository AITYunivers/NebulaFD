using Nebula.Core.CTF25.MFA.Object.Data.Paragraph;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class StringData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            int width = reader.ReadInt();
            int height = reader.ReadInt();

            ParagraphContainer paragraphContainer = new ParagraphContainer();
            paragraphContainer.Read(reader);
        }
    }
}
