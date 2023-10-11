using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectDirection : Chunk
    {
        public byte MinimumSpeed;
        public byte MaximumSpeed;
        public short Repeat;
        public short RepeatFrame;
        public short[] Frames = new short[0];

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

            Frames = new short[reader.ReadShort()];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = reader.ReadShort();
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
