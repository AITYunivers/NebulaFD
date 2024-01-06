using FusionRipper.Core.Memory;
using System.Drawing;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectParagraph : Chunk
    {
        public BitDict ParagraphFlags = new BitDict( // Paragraph Flags
            "MFACorrect", "", "", "",  // MFA Correct Answer
            "", "", "", "", "Correct", // CCN Correct Answer
            "Relief"                   // Relief
        );

        public ushort FontHandle;
        public string Value = string.Empty;
        public Color Color = Color.White;

        public ObjectParagraph()
        {
            ChunkName = "ObjectParagraph";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            FontHandle = (ushort)(reader.ReadUShort() + 1u);
            ParagraphFlags.Value = reader.ReadUShort();
            Color = reader.ReadColor();
            Value = reader.ReadYuniversal();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadAutoYuniversal();
            ParagraphFlags.Value = reader.ReadUInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(Value);
            writer.WriteUInt(ParagraphFlags.Value);
        }
    }
}
