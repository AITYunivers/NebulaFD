using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAString : MFAObjectLoader
    {
        public BitDict StringFlags = new BitDict( // String Flags
            "HoriCenter",               // Alignment Horizontal: Center
            "HoriRight",                // Alignment Horizontal: Right
            "VertCenter",               // Alignment Vertical: Center
            "VertBottom",               // Alignment Vertical: Bottom
            "", "", "", "", "", "", "", //
            "", "", "", "", "", "",     //
            "RightToLeft"               // Right-to-left reading
        );

        public int Width;
        public int Height;
        public int Font;
        public Color Color;

        public ObjectParagraph[] Paragraphs = new ObjectParagraph[0];

        public MFAString()
        {
            ChunkName = "MFAString";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Font = reader.ReadInt();
            Color = reader.ReadColor();
            StringFlags.Value = reader.ReadUInt();
            reader.Skip(4);

            Paragraphs = new ObjectParagraph[reader.ReadUInt()];
            for (int i = 0; i < Paragraphs.Length; i++)
            {
                Paragraphs[i] = new ObjectParagraph();
                Paragraphs[i].ReadMFA(reader);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteInt(Font);
            writer.WriteColor(Color);
            writer.WriteUInt(StringFlags.Value);
            writer.WriteInt(0);

            writer.WriteInt(Paragraphs.Length);
            foreach (ObjectParagraph paragraph in Paragraphs)
                paragraph.WriteMFA(writer);
        }
    }
}
