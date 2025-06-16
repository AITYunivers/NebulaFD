using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame.Instance
{
    internal class FrameInstanceBank : CommonChunk, ICollection<FrameInstanceItem>
    {
        private Collection<FrameInstanceItem> _instanceItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            int instanceCount = reader.ReadInt();
            this.Log($"Found {instanceCount} instance(s)", Logger.LogType.Debug);
            if (instanceCount < 0)
                throw new InvalidDataException("Invalid instance count. Expected greater than or equal to 0, got " + instanceCount);

            _instanceItems = [.. reader.ReadIReadables<FrameInstanceItem>(instanceCount)];
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(_instanceItems.Count);
            writer.WriteIWritables(_instanceItems);
        }

        public int Count => _instanceItems.Count;
        public bool IsReadOnly => false;
        public void Add(FrameInstanceItem item) => _instanceItems.Add(item);
        public bool Remove(FrameInstanceItem item) => _instanceItems.Remove(item);
        public void Clear() => _instanceItems.Clear();
        public bool Contains(FrameInstanceItem item) => _instanceItems.Contains(item);
        public void CopyTo(FrameInstanceItem[] array, int arrayIndex) => _instanceItems.CopyTo(array, arrayIndex);
        public IEnumerator<FrameInstanceItem> GetEnumerator() => _instanceItems.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
