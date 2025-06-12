using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.BinaryFile
{
    public class BinaryFileBank : IReadable, IWritable
    {
        private BinaryFileItem[] _binaryFileItems = [];

        public void Read(ByteReader reader)
        {
            int binaryFileCount = reader.ReadInt();
            this.Log($"Found {binaryFileCount} binary file(s)", Logger.LogType.Debug);
            if (binaryFileCount < 0)
                throw new InvalidDataException("Invalid binary file count. Expected greater than or equal to 0, got " + binaryFileCount);

            _binaryFileItems = reader.ReadIReadables<BinaryFileItem>(binaryFileCount);

            for (int i = 0; i < binaryFileCount; i++)
                this.Log($"Binary File {i}: {_binaryFileItems[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_binaryFileItems.Length);
            writer.WriteIWritables(_binaryFileItems);
        }

        public BinaryFileItem this[int index]
        {
            get => _binaryFileItems[index];
            set => _binaryFileItems[index] = value;
        }
    }
}
