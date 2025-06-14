using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.Chunks.Objects
{
    internal class ObjectBankChunk : CommonChunk, ICollection<ObjectItem>
    {
        private Collection<ObjectItem> _objectItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            int objectCount = reader.ReadInt();
            this.Log($"Found {objectCount} objects(s)", Logger.LogType.Debug);
            if (objectCount < 0)
                throw new InvalidDataException("Invalid object count. Expected greater than or equal to 0, got " + objectCount);

            _objectItems = [.. reader.ReadIReadables<ObjectItem>(objectCount)];
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(_objectItems.Count);
            writer.WriteIWritables(_objectItems);
        }

        public int Count => _objectItems.Count;
        public bool IsReadOnly => false;

        public void Add(ObjectItem item) => _objectItems.Add(item);
        public void Clear() => _objectItems.Clear();
        public bool Contains(ObjectItem item) => _objectItems.Contains(item);
        public void CopyTo(ObjectItem[] array, int arrayIndex) => _objectItems.CopyTo(array, arrayIndex);
        public bool Remove(ObjectItem item) => _objectItems.Remove(item);
        public IEnumerator<ObjectItem> GetEnumerator() => _objectItems.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ObjectItem this[int index]
        {
            get => _objectItems[index];
            set => _objectItems[index] = value;
        }
    }
}
