using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Objects.Properties
{
    internal class BackdropData : CommonChunk
    {
        public ushort ObstacleType;
        public ushort CollisionType;
        public int Width;
        public int Height;
        public ushort ImageHandle;

        public override void ReadChunkData(ByteReader reader)
        {
            reader.Skip(4); // Size
            ObstacleType = reader.ReadUShort();
            CollisionType = reader.ReadUShort();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            ImageHandle = reader.ReadUShort();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
