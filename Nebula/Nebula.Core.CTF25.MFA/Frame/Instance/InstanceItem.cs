using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Frame.Instance
{
    internal class InstanceItem : IReadable, IWritable
    {
        public int X;
        public int Y;
        public uint LayerHandle;
        public uint Handle;
        public ushort Flags;
        public short InstanceValue;
        public uint ParentType; // Is this right?
        public uint ObjectHandle;
        public uint ParentHandle; // Is this right?

        public void Read(ByteReader reader)
        {
            X = reader.ReadInt();
            Y = reader.ReadInt();
            LayerHandle = reader.ReadUInt();
            Handle = reader.ReadUInt();
            Flags = reader.ReadUShort();
            InstanceValue = reader.ReadShort();
            ParentType = reader.ReadUInt();
            ObjectHandle = reader.ReadUInt(); 
            ParentHandle = reader.ReadUInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(X);
            writer.WriteInt(Y);
            writer.WriteUInt(LayerHandle);
            writer.WriteUInt(Handle);
            writer.WriteUShort(Flags);
            writer.WriteShort(InstanceValue);
            writer.WriteUInt(ParentType);
            writer.WriteUInt(ObjectHandle);
            writer.WriteUInt(ParentHandle);
        }
    }
}
