using Nebula.Core.CTF25.MFA.Object.Data.Animation;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class ActiveData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            bool hasAnimations = reader.ReadBool();
            if (hasAnimations)
            {
                AnimationBank animationBank = new AnimationBank();
                animationBank.Read(reader);
            }
        }
    }
}
