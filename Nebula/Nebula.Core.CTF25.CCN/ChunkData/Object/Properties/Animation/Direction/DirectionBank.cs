using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Object.Data.Animation.Direction
{
    internal class DirectionBank : IReadable, IWritable
    {
        private DirectionItem[] _directionItems = [];

        public void Read(ByteReader reader)
        {
            _directionItems = reader.ReadIReadables<DirectionItem, ushort>(32);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteIWritablesWithOffsets<DirectionItem, ushort>(_directionItems);
        }

        public DirectionItem this[int index]
        {
            get => _directionItems[index];
            set => _directionItems[index] = value;
        }
    }
}
