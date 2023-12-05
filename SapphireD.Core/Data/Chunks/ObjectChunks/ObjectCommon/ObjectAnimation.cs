using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAnimation : Chunk
    {
        public string Name = string.Empty;
        public List<ObjectDirection> Directions = new List<ObjectDirection>();

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

            Directions = new List<ObjectDirection>();
            for (int i = 0; i < 32; i++)
                if (Offsets[i] != 0)
                {
                    reader.Seek(StartOffset + Offsets[i]);
                    ObjectDirection dir = new ObjectDirection();
                    dir.ReadCCN(reader);
                    Directions.Add(dir);
                }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Name = reader.ReadAutoYuniversal();
            Directions = new List<ObjectDirection>();
            int Count = reader.ReadInt();
            for (int i = 0; i < Count; i++)
            {
                ObjectDirection dir = new ObjectDirection();
                dir.ReadMFA(reader);
                Directions.Add(dir);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Directions.Count);
            foreach (ObjectDirection direction in Directions)
                direction.WriteMFA(writer);
        }
    }
}
