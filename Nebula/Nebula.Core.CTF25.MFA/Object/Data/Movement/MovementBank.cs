using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Object.Data.Movement
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
        
            _movementItems = reader.ReadIReadables<MovementItem>(movementCount);

            for (int i = 0; i < movementCount; i++)
                this.Log($"Movement {i}: {_movementItems[i].Name}", Logger.LogType.Debug);
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
