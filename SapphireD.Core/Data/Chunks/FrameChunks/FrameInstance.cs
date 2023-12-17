using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameInstance : Chunk
    {
        public BitDict InstanceFlags = new BitDict( // Instance Flags
            "", "", "Locked" // Locked
        );

        public uint Handle;
        public uint ObjectInfo;
        public int PositionX;
        public int PositionY;
        public uint ParentType;
        public uint ParentHandle;
        public uint Layer;
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
            if (SapDCore.Fusion < 3)
                ParentType = reader.ReadUShort();
            ParentHandle = reader.ReadUShort();
            Layer = reader.ReadUShort();
            if (SapDCore.Fusion < 3)
                InstanceValue = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            PositionX = reader.ReadInt();
            PositionY = reader.ReadInt();
            Layer = reader.ReadUInt();
            Handle = reader.ReadUInt();
            InstanceFlags.Value = reader.ReadUShort();
            InstanceValue = reader.ReadShort();
            ParentType = reader.ReadUInt();
            ObjectInfo = reader.ReadUInt();
            ParentHandle = reader.ReadUInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(PositionX);
            writer.WriteInt(PositionY);
            writer.WriteUInt(Layer);
            writer.WriteUInt(Handle);
            writer.WriteUShort((ushort)InstanceFlags.Value);
            writer.WriteShort(InstanceValue);
            writer.WriteUInt(ParentType);
            writer.WriteUInt(ObjectInfo);
            writer.WriteUInt(ParentHandle);
        }
    }
}
