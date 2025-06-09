using Nebula.Core.CTF25.MFA.Object.Data.Animation;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class ExtensionData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            bool hasAnimations = reader.ReadBool();
            if (hasAnimations)
            {
                AnimationBank animationBank = new AnimationBank();
                animationBank.Read(reader);
            }

            uint type = reader.ReadUInt();
            if (type == uint.MaxValue)
            {
                string name = reader.ReadAutoYuniversal();
                string fileName = reader.ReadAutoYuniversal();
                uint magic = reader.ReadUInt();
                string subType = reader.ReadAutoYuniversal();
            }

            reader.Skip(reader.ReadInt()); // Data
        }
    }
}
