using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Object.Data.Behaviour
{
    internal class BehaviourBank : IReadable, IWritable
    {
        private BehaviourItem[] _behaviourItems = [];

        public void Read(ByteReader reader)
        {
            int behaviourCount = reader.ReadInt();
            this.Log($"Found {behaviourCount} behaviour(s)", Logger.LogType.Debug);
            if (behaviourCount < 0)
                throw new InvalidDataException("Invalid behaviour count. Expected greater than or equal to 0, got " + behaviourCount);

            _behaviourItems = reader.ReadIReadables<BehaviourItem>(behaviourCount);

            for (int i = 0; i < behaviourCount; i++)
                this.Log($"Behaviour {i}: {_behaviourItems[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_behaviourItems.Length);
            writer.WriteIWritables(_behaviourItems);
        }

        public BehaviourItem this[int index]
        {
            get => _behaviourItems[index];
            set => _behaviourItems[index] = value;
        }
    }
}
