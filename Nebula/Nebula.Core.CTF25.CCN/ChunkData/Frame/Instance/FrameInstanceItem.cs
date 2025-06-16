using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame.Instance
{
    internal class FrameInstanceItem : IReadable, IWritable
    {
        public ushort Handle;
        public ushort ObjectInfo;
        public int PositionX;
        public int PositionY;
        public ushort ParentType; // Is this right?
        public short InstanceValue;
        public ushort Layer;
        public ushort ParentHandle; // Is this right?

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUShort();
            ObjectInfo = reader.ReadUShort();
            PositionX = reader.ReadInt();
            PositionY = reader.ReadInt();
            ParentType = reader.ReadUShort();
            InstanceValue = reader.ReadShort();
            Layer = reader.ReadUShort();
            ParentHandle = reader.ReadUShort();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUShort(Handle);
            writer.WriteUShort(ObjectInfo);
            writer.WriteInt(PositionX);
            writer.WriteInt(PositionY);
            writer.WriteUShort(ParentType);
            writer.WriteShort(InstanceValue);
            writer.WriteUShort(Layer);
            writer.WriteUShort(ParentHandle);
        }
    }
}
