using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Object
{
    internal class ObjectBank : Collection<ObjectItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int objectCount = reader.ReadInt();
            this.Log($"Found {objectCount} object(s)", Logger.LogType.Debug);
            if (objectCount < 0)
                throw new InvalidDataException("Invalid object count. Expected greater than or equal to 0, got " + objectCount);

            foreach (ObjectItem objectItem in reader.ReadIReadables<ObjectItem>(objectCount))
                Add(objectItem);

            for (int i = 0; i < objectCount; i++)
                this.Log($"Object {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
