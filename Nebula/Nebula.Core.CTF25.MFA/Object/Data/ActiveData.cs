using Nebula.Core.CTF25.MFA.Object.Data.Animation;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class ActiveData : CommonObjectData
    {
        public AnimationBank? Animations;

        public override void ReadUncommonData(ByteReader reader)
        {
            bool hasAnimations = reader.ReadBool();
            if (hasAnimations)
                (Animations = []).Read(reader);
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteBool(Animations != null);
            Animations?.Write(writer);
        }
    }
}
