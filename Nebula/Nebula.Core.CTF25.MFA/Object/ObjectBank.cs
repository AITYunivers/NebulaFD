using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Object
{
    internal class ObjectBank : IReadable, IWritable
    {
        private ObjectItem[] _objectItems = [];

        public void Read(ByteReader reader)
        {
            int objectCount = reader.ReadInt();
            this.Log($"Found {objectCount} object(s)", Logger.LogType.Debug);
            if (objectCount < 0)
                throw new InvalidDataException("Invalid object count. Expected greater than or equal to 0, got " + objectCount);
            
            _objectItems = new ObjectItem[objectCount];
            for (int i = 0; i < objectCount; i++)
            {
                ObjectItem objectItem = new ObjectItem();
                objectItem.Read(reader);
                _objectItems[i] = objectItem;
                this.Log($"Object {i}: {objectItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_objectItems.Length);
            foreach (ObjectItem objectItem in _objectItems)
                objectItem.Write(writer);
        }

        public ObjectItem this[int index]
        {
            get => _objectItems[index];
            set => _objectItems[index] = value;
        }
    }
}
