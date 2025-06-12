using Nebula.Core.CTF25.CCN.Object.Data.Animation.Direction;
using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Object.Data.Animation
{
    internal class AnimationItem : IReadable, IWritable
    {
        public DirectionBank Directions = new DirectionBank();

        public void Read(ByteReader reader)
        {
            Directions.Read(reader);
        }

        public void Write(ByteWriter writer)
        {
            Directions.Write(writer);
        }
    }
}
