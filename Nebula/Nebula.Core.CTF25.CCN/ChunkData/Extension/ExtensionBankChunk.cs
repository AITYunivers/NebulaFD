using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks.Extensions
{
    internal class ExtensionBankChunk : CommonChunk
    {
        private ExtensionItem[] _extensionItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            ushort extensionCount = reader.ReadUShort();
            this.Log($"Found {extensionCount} extension(s)", Logger.LogType.Debug);
            if (extensionCount < 0)
                throw new InvalidDataException("Invalid extension count. Expected greater than or equal to 0, got " + extensionCount);
            reader.Skip(2); // Max Handle?? Is this the case on Windows too?

            _extensionItems = reader.ReadIReadables<ExtensionItem>(extensionCount);

            foreach (ExtensionItem extensionItem in _extensionItems)
                this.Log($"Extension {extensionItem.Handle}: {extensionItem.FileName}", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUShort((ushort)_extensionItems.Length);
            writer.Skip(2); // Max Handle?? Is this the case on Windows too?
            writer.WriteIWritables(_extensionItems);
        }

        public ExtensionItem this[int index]
        {
            get => _extensionItems[index];
            set => _extensionItems[index] = value;
        }
    }
}
