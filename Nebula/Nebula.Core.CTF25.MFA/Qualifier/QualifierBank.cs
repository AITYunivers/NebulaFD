using Nebula.Core.CTF25.MFA.Sound;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Qualifier
{
    public class QualifierBank : Collection<QualifierItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int qualifierCount = reader.ReadInt();
            this.Log($"Found {qualifierCount} qualifier(s)", Logger.LogType.Debug);
            if (qualifierCount < 0)
                throw new InvalidDataException("Invalid qualifier count. Expected greater than or equal to 0, got " + qualifierCount);

            foreach (QualifierItem qualifierItem in reader.ReadIReadables<QualifierItem>(qualifierCount))
            {
                Add(qualifierItem);
                this.Log($"Qualifier {qualifierItem.Handle}: {qualifierItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
