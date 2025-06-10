using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Frame.Instance
{
    internal class InstanceBank : IReadable, IWritable
    {
        private InstanceItem[] _instanceItems = [];

        public void Read(ByteReader reader)
        {
            int instanceCount = reader.ReadInt();
            this.Log($"Found {instanceCount} instance(s)", Logger.LogType.Debug);
            if (instanceCount < 0)
                throw new InvalidDataException("Invalid instance count. Expected greater than or equal to 0, got " + instanceCount);

            _instanceItems = new InstanceItem[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                InstanceItem instanceItem = new InstanceItem();
                instanceItem.Read(reader);
                _instanceItems[i] = instanceItem;
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_instanceItems.Length);
            foreach (InstanceItem instanceItem in _instanceItems)
                instanceItem.Write(writer);
        }

        public InstanceItem this[int index]
        {
            get => _instanceItems[index];
            set => _instanceItems[index] = value;
        }
    }
}
