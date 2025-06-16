using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.Objects;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Objects
{
    public class ObjectHeaderChunk : CommonChunk
    {
        public ushort Handle;
        public EObjectTypes ObjectType;
        public uint Flags;
        public ushort ShaderHandle;
        public ushort ShaderFlags;
        public uint ShaderSimpleParameter;

        public override void ReadChunkData(ByteReader reader)
        {
            Handle = reader.ReadUShort();
            ObjectType = (EObjectTypes)reader.ReadUShort();
            Flags = reader.ReadUInt();
            ShaderHandle = reader.ReadUShort();
            ShaderFlags = reader.ReadUShort();
            ShaderSimpleParameter = reader.ReadUInt();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUShort(Handle);
            writer.WriteUShort((ushort)ObjectType);
            writer.WriteUInt(Flags);
            writer.WriteUShort(ShaderHandle);
            writer.WriteUShort(ShaderFlags);
            writer.WriteUInt(ShaderSimpleParameter);
        }
    }
}
