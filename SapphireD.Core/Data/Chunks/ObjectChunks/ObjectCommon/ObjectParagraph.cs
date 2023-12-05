using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectParagraph : Chunk
    {
        public BitDict ParagraphFlags = new BitDict( // Paragraph Flags
            "HoriCenter", // Alignment Horizontal: Center (Unused?)
            "HoriRight",  // Alignment Horizontal: Right (Unused?)
            "VertCenter", // Alignment Vertical: Center (Unused?)
            "VertBottom", // Alignment Vertical: Bottom (Unused?)
            "Correct",    // Correct Answer
            "Relief"      // Relief (Unused?)
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
            FontHandle = reader.ReadUShort();
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
