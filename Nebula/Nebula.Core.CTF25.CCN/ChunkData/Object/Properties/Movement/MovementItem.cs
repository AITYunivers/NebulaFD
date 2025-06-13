using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Movement
{
    internal class MovementItem : IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            // TODO
            reader.Skip(16); // Header Data Size
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
