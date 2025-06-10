using Nebula.Core.CTF25.MFA.Object.Data.Animation.Direction;
using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation
{
    internal class AnimationItem : IReadable, IWritable
    {
        public string Name = string.Empty;
        public DirectionBank Directions = new DirectionBank();

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            Directions.Read(reader);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
            Directions.Write(writer);
        }
    }
}
