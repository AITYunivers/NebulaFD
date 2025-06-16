using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame.Layer
{
    internal class FrameLayerEffectItem : IReadable, IWritable
    {
        public uint ShaderType;
        public uint ShaderSimpleParameter;
        public uint ShaderHandle;

        public void Read(ByteReader reader)
        {
            ShaderType = reader.ReadUInt();
            ShaderSimpleParameter = reader.ReadUInt();
            ShaderHandle = reader.ReadUInt();
            reader.Skip(4); // Shader Parameter Count
            reader.Skip(4); // Shader Parameter Offset
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
