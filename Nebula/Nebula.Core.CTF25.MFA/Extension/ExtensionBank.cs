using Nebula.Core.CTF25.MFA.Extension;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Extension
{
    internal class ExtensionBank : IReadable, IWritable
    {
        private ExtensionItem[] _extensionItems = [];

        public void Read(ByteReader reader)
        {
            int extensionCount = reader.ReadInt();
            this.Log($"Found {extensionCount} extension(s)", Logger.LogType.Debug);
            if (extensionCount < 0)
                throw new InvalidDataException("Invalid extension count. Expected greater than or equal to 0, got " + extensionCount);

            _extensionItems = reader.ReadIReadables<ExtensionItem>(extensionCount);

            foreach (ExtensionItem extensionItem in _extensionItems)
                this.Log($"Extension {extensionItem.Handle}: {extensionItem.Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_extensionItems.Length);
            writer.WriteIWritables(_extensionItems);
        }

        public ExtensionItem this[int index]
        {
            get => _extensionItems[index];
            set => _extensionItems[index] = value;
        }
    }
}
