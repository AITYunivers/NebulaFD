using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.BinaryFile
{
    public class BinaryFileBank : Collection<BinaryFileItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int binaryFileCount = reader.ReadInt();
            this.Log($"Found {binaryFileCount} binary file(s)", Logger.LogType.Debug);
            if (binaryFileCount < 0)
                throw new InvalidDataException("Invalid binary file count. Expected greater than or equal to 0, got " + binaryFileCount);

            foreach (BinaryFileItem binaryFileItem in reader.ReadIReadables<BinaryFileItem>(binaryFileCount))
                Add(binaryFileItem);

            for (int i = 0; i < binaryFileCount; i++)
                this.Log($"Binary File {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
