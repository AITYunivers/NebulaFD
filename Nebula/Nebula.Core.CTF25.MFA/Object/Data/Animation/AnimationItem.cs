using Nebula.Core.CTF25.MFA.Object.Data.Animation.Direction;
using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation
{
    internal class AnimationItem : IReadable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();

            DirectionBank directionBank = new DirectionBank();
            directionBank.Read(reader);
        }
    }
}
