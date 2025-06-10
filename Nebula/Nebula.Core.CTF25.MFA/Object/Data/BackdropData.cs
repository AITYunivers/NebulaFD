using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class BackdropData : IReadable, IWritable
    {
        public uint ObstacleType;
        public uint CollisionType;
        public uint ImageHandle;

        public void Read(ByteReader reader)
        {
            ObstacleType = reader.ReadUInt();
            CollisionType = reader.ReadUInt();
            ImageHandle = reader.ReadUInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(ObstacleType);
            writer.WriteUInt(CollisionType);
            writer.WriteUInt(ImageHandle);
        }
    }
}
