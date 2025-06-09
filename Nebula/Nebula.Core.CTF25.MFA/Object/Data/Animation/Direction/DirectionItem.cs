using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation.Direction
{
    internal class DirectionItem : IReadable
    {
        public void Read(ByteReader reader)
        {
            uint handle = reader.ReadUInt();
            int minSpeed = reader.ReadInt();
            int maxSpeed = reader.ReadInt();
            int repeatCount = reader.ReadInt();
            int repeatFrom = reader.ReadInt();
            reader.Skip(reader.ReadInt() * 4); // Frame Image Handles
        }
    }
}
