using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Animation.Direction
{
    internal class DirectionBank : Collection<DirectionItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            foreach (DirectionItem directionItem in reader.ReadIReadables<DirectionItem, ushort>(32, reader.Tell()))
                Add(directionItem);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteIWritablesWithOffsets<DirectionItem, ushort>(this);
        }
    }
}
