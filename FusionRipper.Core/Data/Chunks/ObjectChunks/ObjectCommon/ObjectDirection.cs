using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectDirection : Chunk
    {
        public int Index;
        public int MinimumSpeed;
        public int MaximumSpeed;
        public int Repeat;
        public int RepeatFrame;
        public uint[] Frames = new uint[0];

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

            Frames = new uint[reader.ReadShort()];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = reader.ReadUShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Index = reader.ReadInt();
            MinimumSpeed = reader.ReadInt();
            MaximumSpeed = reader.ReadInt();
            Repeat = reader.ReadInt();
            RepeatFrame = reader.ReadInt();

            Frames = new uint[reader.ReadInt()];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = reader.ReadUInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Index);
            writer.WriteInt(MinimumSpeed);
            writer.WriteInt(MaximumSpeed);
            writer.WriteInt(Repeat);
            writer.WriteInt(RepeatFrame);

            writer.WriteInt(Frames.Length);
            foreach (uint frame in Frames)
                writer.WriteUInt(frame);
        }
    }
}
