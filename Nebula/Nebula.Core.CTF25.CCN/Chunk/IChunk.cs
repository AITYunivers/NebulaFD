using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunk
{
    public interface IChunk
    {
        public void ReadChunkData(ByteReader reader);
        public void WriteChunkData(ByteWriter writer);
        public void SetChunkDefinition(ChunkDefinition chunkDefinition);
        public ChunkDefinition GetChunkDefinition();
    }
}
