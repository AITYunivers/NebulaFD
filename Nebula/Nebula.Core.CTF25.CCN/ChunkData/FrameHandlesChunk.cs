using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class FrameHandlesChunk : CommonChunk, ICollection<ushort>
    {
        private Collection<ushort> _frameHandles = [];

        public override void ReadChunkData(ByteReader reader)
        {
            while (reader.HasMemory(2))
                _frameHandles.Add(reader.ReadUShort());

            this.Log($"Found {Count} Frame Handles", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUShorts(_frameHandles);
        }

        public int Count => _frameHandles.Count;
        public bool IsReadOnly => false;
        public void Add(ushort item) => _frameHandles.Add(item);
        public void Clear() => _frameHandles.Clear();
        public bool Contains(ushort item) => _frameHandles.Contains(item);
        public void CopyTo(ushort[] array, int arrayIndex) => _frameHandles.CopyTo(array, arrayIndex);
        public bool Remove(ushort item) => (_frameHandles.Remove(item));
        public IEnumerator<ushort> GetEnumerator() => _frameHandles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
