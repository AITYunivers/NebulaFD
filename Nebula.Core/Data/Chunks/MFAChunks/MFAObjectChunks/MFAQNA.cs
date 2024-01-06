using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAQNA : MFAObjectLoader
    {
        public int Width;
        public int Height;

        public int QuestionFont;
        public Color QuestionColor;
        public bool QuestionRelief;
        public ObjectParagraph QuestionParagraph = new();

        public int AnswerFont;
        public Color AnswerColor;
        public bool AnswerRelief;
        public ObjectParagraph[] AnswerParagraphs = new ObjectParagraph[0];

        public MFAQNA()
        {
            ChunkName = "MFAQNA";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Width = reader.ReadInt();
            Height = reader.ReadInt();

            QuestionFont = reader.ReadInt();
            QuestionColor = reader.ReadColor();
            reader.Skip(4); // 37
            QuestionRelief = reader.ReadInt() == 1;
            reader.Skip(4); // 1
            QuestionParagraph = new ObjectParagraph();
            QuestionParagraph.ReadMFA(reader);

            AnswerFont = reader.ReadInt();
            AnswerColor = reader.ReadColor();
            reader.Skip(4); // 37
            AnswerRelief = reader.ReadInt() == 1;
            AnswerParagraphs = new ObjectParagraph[reader.ReadInt()];
            for (int i = 0; i < AnswerParagraphs.Length; i++)
            {
                AnswerParagraphs[i] = new ObjectParagraph();
                AnswerParagraphs[i].ReadMFA(reader);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteInt(Width);
            writer.WriteInt(Height);

            writer.WriteInt(QuestionFont);
            writer.WriteColor(QuestionColor);
            writer.WriteInt(37);
            writer.WriteInt(QuestionRelief ? 1 : 0);
            writer.WriteInt(1);
            QuestionParagraph.WriteMFA(writer);

            writer.WriteInt(AnswerFont);
            writer.WriteColor(AnswerColor);
            writer.WriteInt(37);
            writer.WriteInt(AnswerRelief ? 1 : 0);
            writer.WriteInt(AnswerParagraphs.Length);
            foreach (ObjectParagraph AnswerParagraph in AnswerParagraphs)
                AnswerParagraph.WriteMFA(writer);
        }
    }
}
