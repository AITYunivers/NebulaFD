using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAString : MFAObjectLoader
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

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
            Flags.Value = reader.ReadUInt();
            reader.Skip(4);

            Paragraphs = new ObjectParagraph[reader.ReadUInt()];
            for (int i = 0; i < Paragraphs.Length; i++)
            {
                Paragraphs[i] = new ObjectParagraph();
                Paragraphs[i].ReadMFA(reader);
            }
        }
    }
}
