using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class QuickBackdropData : IReadable
    {
        public void Read(ByteReader reader)
        {
            uint obstacleType = reader.ReadUInt();
            uint collisionType = reader.ReadUInt();
            int width = reader.ReadInt();
            int height = reader.ReadInt();
            int shape = reader.ReadInt();
            int borderSize = reader.ReadInt();
            Color borderColor = reader.ReadColor();
            int fillType = reader.ReadInt();
            Color color1 = reader.ReadColor();
            Color color2 = reader.ReadColor();
            uint flags = reader.ReadUInt();
            uint imageHandle = reader.ReadUInt();
        }
    }
}
