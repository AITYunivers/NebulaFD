using Nebula.Core.CTF25.MFA.Object.Data.Paragraph;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class QuestionData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            int width = reader.ReadInt();
            int height = reader.ReadInt();

            ParagraphContainer questionsContainer = new ParagraphContainer();
            questionsContainer.Read(reader);

            ParagraphContainer answersContainer = new ParagraphContainer();
            answersContainer.Read(reader);
        }
    }
}
