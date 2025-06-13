using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Movement
{
    internal class MovementBank : IReadable, IWritable
    {
        private MovementItem[] _movementItems = [];

        public void Read(ByteReader reader)
        {
            int movementCount = reader.ReadInt();
            this.Log($"Found {movementCount} movement(s)", Logger.LogType.Debug);
            if (movementCount < 0)
                throw new InvalidDataException("Invalid movement count. Expected greater than or equal to 0, got " + movementCount);
            
            int[] offsets = [.. Enumerable.Range(0, movementCount).Select(i => i * 16)];
            _movementItems = reader.ReadIReadables<MovementItem, int>(movementCount, offsets);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_movementItems.Length);
            writer.WriteIWritables(_movementItems);
        }

        public MovementItem this[int index]
        {
            get => _movementItems[index];
            set => _movementItems[index] = value;
        }
    }
}
