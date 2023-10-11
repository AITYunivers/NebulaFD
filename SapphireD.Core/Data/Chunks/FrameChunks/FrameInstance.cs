using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameInstance : Chunk
    {
        public ushort Handle;
        public ushort ObjectInfo;
        public int PositionX;
        public int PositionY;
        public short ParentType;
        public short ParentHandle;
        public short Layer;
        public short InstanceValue;

        public FrameInstance()
        {
            ChunkName = "FrameInstance";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUShort();
            ObjectInfo = reader.ReadUShort();
            PositionX = reader.ReadInt();
            PositionY = reader.ReadInt();
            ParentType = reader.ReadShort();
            ParentHandle = reader.ReadShort();
            Layer = reader.ReadShort();
            InstanceValue = reader.ReadShort();
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
