using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
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
            // First two bytes in MMF 1.5 is a size
            FontHandle = reader.ReadUShort();
            if (!NebulaCore.Android && NebulaCore.Fusion >= 2.5)
                FontHandle++;

            if (NebulaCore.Fusion == 1.5f)
                FontHandle = reader.ReadUShort();
            else
                ParagraphFlags.Value = reader.ReadUShort();

            Color = reader.ReadColor();

            if (NebulaCore.Fusion == 1.5)
                reader.Skip(2);

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
