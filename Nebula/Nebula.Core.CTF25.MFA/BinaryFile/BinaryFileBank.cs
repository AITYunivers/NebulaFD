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

            _binaryFileItems = new BinaryFileItem[binaryFileCount];
            for (int i = 0; i < binaryFileCount; i++)
            {
                BinaryFileItem binaryFileItem = new BinaryFileItem();
                binaryFileItem.Read(reader);
                _binaryFileItems[i] = binaryFileItem;

                this.Log($"Binary File {i}: {binaryFileItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_binaryFileItems.Length);
            foreach (BinaryFileItem binaryFileItem in _binaryFileItems)
                binaryFileItem.Write(writer);
        }

        public BinaryFileItem this[int index]
        {
            get => _binaryFileItems[index];
            set => _binaryFileItems[index] = value;
        }
    }
}
