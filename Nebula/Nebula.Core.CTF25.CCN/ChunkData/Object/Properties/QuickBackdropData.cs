using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.ChunkData.Object.Properties;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunks.Objects.Properties
{
    internal class QuickBackdropData : CommonChunk
    {
        public ushort ObstacleType;
        public ushort CollisionType;
        public int Width;
        public int Height;
        public ShapeData Shape = new ShapeData();
        public ushort ImageHandle;

        public override void ReadChunkData(ByteReader reader)
        {
            reader.Skip(4); // Size
            ObstacleType = reader.ReadUShort();
            CollisionType = reader.ReadUShort();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Shape.Read(reader);
            ImageHandle = reader.ReadUShort();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
