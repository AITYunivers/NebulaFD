using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAnimation : Chunk
    {
        public ObjectDirection[] Directions = new ObjectDirection[0];

        public ObjectAnimation()
        {
            ChunkName = "ObjectAnimation";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();

            short[] Offsets = new short[32];
            for (int i = 0; i < 32; i++)
                Offsets[i] = reader.ReadShort();

            Directions = new ObjectDirection[32];
            for (int i = 0; i < 32; i++)
            {
                Directions[i] = new ObjectDirection();
                if (Offsets[i] != 0)
                {
                    reader.Seek(StartOffset + Offsets[i]);
                    Directions[i].ReadCCN(reader);
                }
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
