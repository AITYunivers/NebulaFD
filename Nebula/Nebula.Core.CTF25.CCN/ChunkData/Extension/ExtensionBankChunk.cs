using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Extensions
{
    internal class ExtensionBankChunk : CommonChunk, ICollection<ExtensionItem>
    {
        private Collection<ExtensionItem> _extensionItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            ushort extensionCount = reader.ReadUShort();
            this.Log($"Found {extensionCount} extension(s)", Logger.LogType.Debug);
            if (extensionCount < 0)
                throw new InvalidDataException("Invalid extension count. Expected greater than or equal to 0, got " + extensionCount);
            reader.Skip(2); // Max Handle?? Is this the case on Windows too?

            _extensionItems = [.. reader.ReadIReadables<ExtensionItem>(extensionCount)];

            foreach (ExtensionItem extensionItem in _extensionItems)
                this.Log($"Extension {extensionItem.Handle}: {extensionItem.FileName}", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUShort((ushort)Count);
            writer.Skip(2); // Max Handle?? Is this the case on Windows too?
            writer.WriteIWritables(_extensionItems);
        }

        public int Count => _extensionItems.Count;
        public bool IsReadOnly => false;
        public void Add(ExtensionItem item) => _extensionItems.Add(item);
        public void Clear() => _extensionItems.Clear();
        public bool Contains(ExtensionItem item) => _extensionItems.Contains(item);
        public void CopyTo(ExtensionItem[] array, int arrayIndex) => _extensionItems.CopyTo(array, arrayIndex);
        public bool Remove(ExtensionItem item) => _extensionItems.Remove(item);
        public IEnumerator<ExtensionItem> GetEnumerator() => _extensionItems.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ExtensionItem this[int index]
        {
            get => _extensionItems[index];
            set => _extensionItems[index] = value;
        }
    }
}
