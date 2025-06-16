using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameEffectsChunk : CommonChunk
    {
        public uint ShaderType;
        public uint ShaderSimpleParameter;
        public uint ShaderHandle;

        public override void ReadChunkData(ByteReader reader)
        {
            ShaderType = reader.ReadUInt();
            ShaderSimpleParameter = reader.ReadUInt();
            ShaderHandle = reader.ReadUInt();
            int shaderParameterCount = reader.ReadInt();
            reader.Skip(shaderParameterCount * 4); // Shader Parameters

            this.Log($"Found Frame Effect with {shaderParameterCount} parameters", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
