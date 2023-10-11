using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectParagraphs : Chunk
    {
        public int Width;
        public int Height;
        public ObjectParagraph[] Paragraphs = new ObjectParagraph[0];

        public ObjectParagraphs()
        {
            ChunkName = "ObjectParagraphs";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();
            reader.Skip(4);
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            int Count = reader.ReadInt();

            int[] Offsets = new int[Count];
            for (int i = 0; i < Count; i++)
                Offsets[i] = reader.ReadInt();

            Paragraphs = new ObjectParagraph[Count];
            for (int i = 0; i < Count; i++)
            {
                reader.Seek(StartOffset + Offsets[i]);
                Paragraphs[i] = new ObjectParagraph();
                Paragraphs[i].ReadCCN(reader);
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
