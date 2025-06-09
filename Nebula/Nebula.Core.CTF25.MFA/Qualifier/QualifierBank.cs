using Nebula.Core.CTF25.MFA.Sound;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Qualifier
{
    public class QualifierBank : IReadable
    {
        private QualifierItem[] _qualifierItems = [];

        public void Read(ByteReader reader)
        {
            int qualifierCount = reader.ReadInt();
            this.Log($"Found {qualifierCount} qualifier(s)", Logger.LogType.Debug);
            if (qualifierCount < 0)
                throw new InvalidDataException("Invalid qualifier count. Expected greater than or equal to 0, got " + qualifierCount);

            _qualifierItems = new QualifierItem[qualifierCount];
            for (int i = 0; i < qualifierCount; i++)
            {
                QualifierItem qualifierItem = new QualifierItem();
                qualifierItem.Read(reader);
                _qualifierItems[i] = qualifierItem;

                this.Log($"Qualifier {qualifierItem.Handle}: {qualifierItem.Name}", Logger.LogType.Debug);
            }
        }

        public QualifierItem this[int index]
        {
            get => _qualifierItems[index];
            set => _qualifierItems[index] = value;
        }
    }
}
