using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame.Layer
{
    internal class FrameLayerEffectBank(int layerCount) : CommonChunk, ICollection<FrameLayerEffectItem>
    {
        private Collection<FrameLayerEffectItem> _layerEffectItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            _layerEffectItems = [.. reader.ReadIReadables<FrameLayerEffectItem>(layerCount)];

            this.Log($"Read {layerCount} Layer Effects", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            throw new NotImplementedException();
        }

        public int Count => _layerEffectItems.Count;
        public bool IsReadOnly => false;
        public void Add(FrameLayerEffectItem item) => _layerEffectItems.Add(item);
        public bool Remove(FrameLayerEffectItem item) => _layerEffectItems.Remove(item);
        public void Clear() => _layerEffectItems.Clear();
        public bool Contains(FrameLayerEffectItem item) => _layerEffectItems.Contains(item);
        public void CopyTo(FrameLayerEffectItem[] array, int arrayIndex) => _layerEffectItems.CopyTo(array, arrayIndex);
        public IEnumerator<FrameLayerEffectItem> GetEnumerator() => _layerEffectItems.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
