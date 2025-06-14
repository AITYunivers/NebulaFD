using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Frame.Instance
{
    internal class InstanceBank : Collection<InstanceItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int instanceCount = reader.ReadInt();
            this.Log($"Found {instanceCount} instance(s)", Logger.LogType.Debug);
            if (instanceCount < 0)
                throw new InvalidDataException("Invalid instance count. Expected greater than or equal to 0, got " + instanceCount);

            foreach (InstanceItem instanceItem in reader.ReadIReadables<InstanceItem>(instanceCount))
                instanceItem.Read(reader);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
