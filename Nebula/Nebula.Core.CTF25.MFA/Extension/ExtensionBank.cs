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

            _extensionItems = new ExtensionItem[extensionCount];
            for (int i = 0; i < extensionCount; i++)
            {
                ExtensionItem extensionItem = new ExtensionItem();
                extensionItem.Read(reader);
                _extensionItems[i] = extensionItem;

                this.Log($"Extension {extensionItem.Handle}: {extensionItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_extensionItems.Length);
            foreach (ExtensionItem extensionItem in _extensionItems)
                extensionItem.Write(writer);
        }

        public ExtensionItem this[int index]
        {
            get => _extensionItems[index];
            set => _extensionItems[index] = value;
        }
    }
}
