using Nebula.Core.CTF25.MFA.Common;
using Nebula.Core.CTF25.MFA.Object.Data.Behaviour;
using Nebula.Core.CTF25.MFA.Object.Data.Movement;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal abstract class CommonObjectData : IReadable, IWritable
    {
        public ulong CommonFlags;
        public Color Background = Color.White;
        public short[] Qualifiers = [-1, -1, -1, -1, -1, -1, -1, -1];
        public CommonValue[] AlterableValues = [];
        public CommonValue[] AlterableStrings = [];
        public MovementBank Movements = new MovementBank();
        public BehaviourBank Behaviours = new BehaviourBank();
        public Transition? TransitionIn;
        public Transition? TransitionOut;

        public void Read(ByteReader reader)
        {
            CommonFlags = reader.ReadULong();
            Background = reader.ReadColor();

            for (int i = 0; i < 8; i++)
                Qualifiers[i] = reader.ReadShort();
            reader.Skip(2); // Extra Qualifier?

            AlterableValues = new CommonValue[reader.ReadInt()];
            for (int i = 0; i < AlterableValues.Length; i++)
            {
                CommonValue alterableValue = new CommonValue();
                alterableValue.Read(reader);
                AlterableValues[i] = alterableValue;
            }

            AlterableStrings = new CommonValue[reader.ReadInt()];
            for (int i = 0; i < AlterableStrings.Length; i++)
            {
                CommonValue alterableString = new CommonValue();
                alterableString.Read(reader);
                AlterableStrings[i] = alterableString;
            }

            Movements.Read(reader);
            Behaviours.Read(reader);

            bool hasTransitionIn = reader.ReadBool();
            if (hasTransitionIn)
            {
                TransitionIn = new Transition();
                TransitionIn.Read(reader);
            }

            bool hasTransitionOut = reader.ReadBool();
            if (hasTransitionOut)
            {
                TransitionOut = new Transition();
                TransitionOut.Read(reader);
            }

            ReadUncommonData(reader);
        }

        public abstract void ReadUncommonData(ByteReader reader);

        public void Write(ByteWriter writer)
        {
            writer.WriteULong(Flags);
            writer.WriteColor(Background);

            for (int i = 0; i < 8; i++)
                writer.WriteShort(Qualifiers[i]);
            writer.WriteShort(-1); // Extra Qualifier?

            writer.WriteInt(AlterableValues.Length);
            foreach (CommonValue alterableValue in AlterableValues)
                alterableValue.Write(writer);

            writer.WriteInt(AlterableStrings.Length);
            foreach (CommonValue alterableString in AlterableStrings)
                alterableString.Write(writer);

            Movements.Write(writer);
            Behaviours.Write(writer);

            writer.WriteBool(TransitionIn != null);
            TransitionIn?.Write(writer);

            writer.WriteBool(TransitionOut != null);
            TransitionOut?.Write(writer);

            WriteUncommonData(writer);
        }

        public abstract void WriteUncommonData(ByteWriter writer);
    }
}
