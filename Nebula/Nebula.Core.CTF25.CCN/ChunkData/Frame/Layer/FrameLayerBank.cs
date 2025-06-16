using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame.Layer
{
    internal class FrameLayerBank : CommonChunk, ICollection<FrameLayerItem>
    {
        private Collection<FrameLayerItem> _layerItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            int layerCount = reader.ReadInt();
            this.Log($"Found {layerCount} layer(s)", Logger.LogType.Debug);
            if (layerCount < 0)
                throw new InvalidDataException("Invalid layer count. Expected greater than or equal to 0, got " + layerCount);

            _layerItems = [.. reader.ReadIReadables<FrameLayerItem>(layerCount)];
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(_layerItems.Count);
            writer.WriteIWritables(_layerItems);
        }

        public int Count => _layerItems.Count;
        public bool IsReadOnly => false;
        public void Add(FrameLayerItem item) => _layerItems.Add(item);
        public bool Remove(FrameLayerItem item) => _layerItems.Remove(item);
        public void Clear() => _layerItems.Clear();
        public bool Contains(FrameLayerItem item) => _layerItems.Contains(item);
        public void CopyTo(FrameLayerItem[] array, int arrayIndex) => _layerItems.CopyTo(array, arrayIndex);
        public IEnumerator<FrameLayerItem> GetEnumerator() => _layerItems.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
