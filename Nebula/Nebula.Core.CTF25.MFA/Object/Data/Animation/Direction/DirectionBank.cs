using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation.Direction
{
    internal class DirectionBank : Collection<DirectionItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int directionCount = reader.ReadInt();
            this.Log($"Found {directionCount} direction(s)", Logger.LogType.Debug);
            if (directionCount < 0)
                throw new InvalidDataException("Invalid direction count. Expected greater than or equal to 0, got " + directionCount);

            foreach (DirectionItem directionItem in reader.ReadIReadables<DirectionItem>(directionCount))
                Add(directionItem);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
