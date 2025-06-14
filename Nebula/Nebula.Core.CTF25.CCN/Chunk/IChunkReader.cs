using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunk
{
    public interface IChunkReader : ICollection<IChunk>
    {
        public void ReadChunk(ChunkDefinition chunkDefinition, ByteReader reader);
        public T[] GetChunks<T>();
        public T? GetFirstChunk<T>();
        public bool HasChunk<T>();
    }
}
