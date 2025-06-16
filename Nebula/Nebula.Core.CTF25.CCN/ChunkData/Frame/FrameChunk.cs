using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.ChunkData.Frame.Instance;
using Nebula.Core.CTF25.CCN.ChunkData.Frame.Layer;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameChunk : CommonChunk, ICollection<IChunk>, IChunkReader
    {
        private readonly ICollection<IChunk> _chunks = [];

        public override void ReadChunkData(ByteReader reader)
        {
            while (reader.HasMemory(8))
            {
                ChunkDefinition chunkDefinition = new ChunkDefinition();
                chunkDefinition.Read(reader);

                this.Log($"Found Frame Chunk 0x{chunkDefinition.GetID():X} known as '{chunkDefinition.GetChunkType()}'");
                ReadChunk(chunkDefinition, reader);

                if (chunkDefinition.GetChunkType() == EChunks.LAST)
                    break;
            }
        }

        public void ReadChunk(ChunkDefinition chunkDefinition, ByteReader reader)
        {
            CommonChunk? chunk = chunkDefinition.GetChunkType() switch
            {
                EChunks.FRAME_HEADER => new FrameHeaderChunk(),
                EChunks.FRAME_NAME => new FrameNameChunk(),
                EChunks.FRAME_PALETTE => new FramePaletteChunk(),
                EChunks.FRAME_INSTANCES => new FrameInstanceBank(),
                EChunks.FRAME_TRANSITION_IN => new FrameTransitionInChunk(),
                EChunks.FRAME_TRANSITION_OUT => new FrameTransitionOutChunk(),
                EChunks.FRAME_LAYERS => new FrameLayerBank(),
                EChunks.FRAME_RECT => new FrameRectChunk(),
                EChunks.FRAME_LAYER_EFFECTS => CreateLayerEffectBank(),
                EChunks.FRAME_MOVEMENT_TIMER => new FrameMovementTimerChunk(),
                EChunks.FRAME_EFFECTS => new FrameEffectsChunk(),
                _ => null
            };

            if (chunk != null)
            {
                chunk.SetChunkDefinition(chunkDefinition);
                chunk.Read(reader);
                Add(chunk);
            }
        }

        private CommonChunk? CreateLayerEffectBank()
        {
            FrameLayerBank? layerBank = GetFirstChunk<FrameLayerBank>();
            if (layerBank == null)
                return null;

            return new FrameLayerEffectBank(layerBank.Count);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            throw new NotImplementedException();
        }

        public T[] GetChunks<T>() => [.. _chunks.Where(x => x is T).Select(x => (T)x)];
        public T? GetFirstChunk<T>() => GetChunks<T>().FirstOrDefault();
        public bool HasChunk<T>() => _chunks.Any(x => x is T);

        public int Count => _chunks.Count;
        public bool IsReadOnly => false;
        public void Add(IChunk item) => _chunks.Add(item);
        public void Clear() => _chunks.Clear();
        public bool Contains(IChunk item) => _chunks.Contains(item);
        public void CopyTo(IChunk[] array, int arrayIndex) => _chunks.CopyTo(array, arrayIndex);
        public bool Remove(IChunk item) => _chunks.Remove(item);
        public IEnumerator<IChunk> GetEnumerator() => _chunks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
