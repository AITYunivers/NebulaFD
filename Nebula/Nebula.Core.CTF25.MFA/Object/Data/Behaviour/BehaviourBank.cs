using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Object.Data.Behaviour
{
    internal class BehaviourBank : Collection<BehaviourItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int behaviourCount = reader.ReadInt();
            this.Log($"Found {behaviourCount} behaviour(s)", Logger.LogType.Debug);
            if (behaviourCount < 0)
                throw new InvalidDataException("Invalid behaviour count. Expected greater than or equal to 0, got " + behaviourCount);

            foreach (BehaviourItem behaviourItem in reader.ReadIReadables<BehaviourItem>(behaviourCount))
                Add(behaviourItem);

            for (int i = 0; i < behaviourCount; i++)
                this.Log($"Behaviour {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
