using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class Event : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public Condition[] Conditions = new Condition[0];
        public Action[] Actions = new Action[0];

        public int Restricted;
        public int RestrictCpt;
        public short Identifier;
        public short Undo;

        public Event()
        {
            ChunkName = "Event";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + (reader.ReadShort() * -1);

            Conditions = new Condition[reader.ReadByte()];
            Actions = new Action[reader.ReadByte()];
            Flags.Value = reader.ReadUShort();

            if (SapDCore.Build >= 284 && !(SapDCore.MFA || SapDCore.Android && SapDCore.Build == 287))
            {
                reader.Skip(2);
                Restricted = reader.ReadInt();
                RestrictCpt = reader.ReadInt();
            }
            else
            {
                Restricted = reader.ReadShort();
                RestrictCpt = reader.ReadShort();
                Identifier = reader.ReadShort();
                Undo = reader.ReadShort();
            }

            for (int i = 0; i < Conditions.Length; i++)
            {
                Conditions[i] = new Condition();
                Conditions[i].ReadCCN(reader);
            }

            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i] = new Action();
                Actions[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + (reader.ReadShort() * -1);

            Conditions = new Condition[reader.ReadByte()];
            Actions = new Action[reader.ReadByte()];
            Flags.Value = reader.ReadUShort();
            Restricted = reader.ReadShort();
            RestrictCpt = reader.ReadShort();
            Identifier = reader.ReadShort();
            Undo = reader.ReadShort();

            for (int i = 0; i < Conditions.Length; i++)
            {
                Conditions[i] = new Condition();
                Conditions[i].ReadMFA(reader);
            }

            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i] = new Action();
                Actions[i].ReadMFA(reader);
            }

            reader.Seek(endPosition);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
