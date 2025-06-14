using Nebula.Core.CTF25.MFA.Object.Data.Paragraph;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class QuestionData : CommonObjectData
    {
        public int Width;
        public int Height;
        public ParagraphContainer Questions = [];
        public ParagraphContainer Answers = [];

        public override void ReadUncommonData(ByteReader reader)
        {
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Questions.Read(reader);
            Answers.Read(reader);
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            Questions.Write(writer);
            Answers.Write(writer);
        }
    }
}
