using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectDirection : Chunk
    {
        public int Index;
        public int MinimumSpeed;
        public int MaximumSpeed;
        public int Repeat;
        public int RepeatFrame;
        public int[] Frames = new int[0];

        public ObjectDirection()
        {
            ChunkName = "ObjectDirection";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            MinimumSpeed = reader.ReadByte();
            MaximumSpeed = reader.ReadByte();
            Repeat = reader.ReadShort();
            RepeatFrame = reader.ReadShort();

            Frames = new int[reader.ReadShort()];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Index = reader.ReadInt();
            MinimumSpeed = reader.ReadInt();
            MaximumSpeed = reader.ReadInt();
            Repeat = reader.ReadInt();
            RepeatFrame = reader.ReadInt();

            Frames = new int[reader.ReadInt()];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = reader.ReadInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
