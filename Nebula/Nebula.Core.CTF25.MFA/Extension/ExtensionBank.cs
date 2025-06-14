using Nebula.Core.CTF25.MFA.Extension;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Extension
{
    internal class ExtensionBank : Collection<ExtensionItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int extensionCount = reader.ReadInt();
            this.Log($"Found {extensionCount} extension(s)", Logger.LogType.Debug);
            if (extensionCount < 0)
                throw new InvalidDataException("Invalid extension count. Expected greater than or equal to 0, got " + extensionCount);

            foreach (ExtensionItem extensionItem in reader.ReadIReadables<ExtensionItem>(extensionCount))
            {
                Add(extensionItem);
                this.Log($"Extension {extensionItem.Handle}: {extensionItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
