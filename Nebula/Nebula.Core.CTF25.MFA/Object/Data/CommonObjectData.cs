using Nebula.Core.CTF25.MFA.Common;
using Nebula.Core.CTF25.MFA.Object.Data.Behaviour;
using Nebula.Core.CTF25.MFA.Object.Data.Movement;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal abstract class CommonObjectData : IReadable
    {
        public void Read(ByteReader reader)
        {
            uint flags = reader.ReadUInt();
            uint flags2 = reader.ReadUInt();
            Color background = reader.ReadColor();
            reader.Skip(18); // Qualifiers

            int alterableValueCount = reader.ReadInt();
            for (int i = 0; i < alterableValueCount; i++)
            {
                CommonValue alterableValue = new CommonValue();
                alterableValue.Read(reader);
            }

            int alterableStringCount = reader.ReadInt();
            for (int i = 0; i < alterableStringCount; i++)
            {
                CommonValue alterableString = new CommonValue();
                alterableString.Read(reader);
            }

            MovementBank movementBank = new MovementBank();
            movementBank.Read(reader);

            BehaviourBank behaviourBank = new BehaviourBank();
            behaviourBank.Read(reader);

            bool hasTransitionIn = reader.ReadBool();
            if (hasTransitionIn)
            {
                Transition transitionIn = new Transition();
                transitionIn.Read(reader);
            }

            bool hasTransitionOut = reader.ReadBool();
            if (hasTransitionOut)
            {
                Transition transitionOut = new Transition();
                transitionOut.Read(reader);
            }

            ReadUncommonData(reader);
        }

        public abstract void ReadUncommonData(ByteReader reader);
    }
}
