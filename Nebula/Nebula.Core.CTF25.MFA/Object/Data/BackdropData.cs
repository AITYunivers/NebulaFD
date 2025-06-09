using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class BackdropData : IReadable
    {
        public void Read(ByteReader reader)
        {
            uint obstacleType = reader.ReadUInt();
            uint collisionType = reader.ReadUInt();
            uint imageHandle = reader.ReadUInt();
        }
    }
}
