using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation.Direction
{
    internal class DirectionBank : IReadable, IWritable
    {
        private DirectionItem[] _directionItems = [];

        public void Read(ByteReader reader)
        {
            int directionCount = reader.ReadInt();
            this.Log($"Found {directionCount} direction(s)", Logger.LogType.Debug);
            if (directionCount < 0)
                throw new InvalidDataException("Invalid direction count. Expected greater than or equal to 0, got " + directionCount);

            _directionItems = new DirectionItem[directionCount];
            for (int i = 0; i < directionCount; i++)
            {
                DirectionItem directionItem = new DirectionItem();
                directionItem.Read(reader);
                _directionItems[i] = directionItem;
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_directionItems.Length);
            foreach (DirectionItem directionItem in _directionItems)
                directionItem.Write(writer);
        }

        public DirectionItem this[int index]
        {
            get => _directionItems[index];
            set => _directionItems[index] = value;
        }
    }
}
