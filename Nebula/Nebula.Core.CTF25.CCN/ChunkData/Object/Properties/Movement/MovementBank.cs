using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Movement
{
    internal class MovementBank : Collection<MovementItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int movementCount = reader.ReadInt();
            this.Log($"Found {movementCount} movement(s)", Logger.LogType.Debug);
            if (movementCount < 0)
                throw new InvalidDataException("Invalid movement count. Expected greater than or equal to 0, got " + movementCount);
            
            int[] offsets = [.. Enumerable.Range(0, movementCount).Select(i => i * 16)];
            foreach (MovementItem movementItem in reader.ReadIReadables<MovementItem, int>(movementCount, offsets))
                Add(movementItem);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
