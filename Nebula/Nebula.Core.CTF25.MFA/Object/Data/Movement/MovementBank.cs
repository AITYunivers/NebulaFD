using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Object.Data.Movement
{
    internal class MovementBank : Collection<MovementItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int movementCount = reader.ReadInt();
            this.Log($"Found {movementCount} movement(s)", Logger.LogType.Debug);
            if (movementCount < 0)
                throw new InvalidDataException("Invalid movement count. Expected greater than or equal to 0, got " + movementCount);

            foreach (MovementItem movementItem in reader.ReadIReadables<MovementItem>(movementCount))
                Add(movementItem);

            for (int i = 0; i < movementCount; i++)
                this.Log($"Movement {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
