using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class Event : Chunk
    {
        public BitDict EventFlags = new BitDict( // Event Flags
            "Once",         // Once
            "NotAlways",    // Not Always
            "Repeat",       // Repeat
            "NoMore",       // No More
            "Shuffle",      // Shuffle
            "EditorMark",   // Editor Mark
            "HasChildren",  // Has Children
            "Break",        // Break
            "BreakPoint",   // Break Point
            "AlwaysClean",  // Always Clean
            "HasOr",        // Or In Group
            "HasStop",      // Stop In Group
            "HasOrLogical", // Or Logical
            "Grouped",      // Grouped
            "Inactive",     // Inactive
            "Has Parent"    // Has Parent
        );

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
            EventFlags.Value = reader.ReadUShort();

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
            EventFlags.Value = reader.ReadUShort();
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
            ByteWriter evtWriter = new ByteWriter(new MemoryStream());
            evtWriter.WriteByte((byte)Conditions.Length);
            evtWriter.WriteByte((byte)Actions.Length);
            evtWriter.WriteUShort((ushort)EventFlags.Value);
            evtWriter.WriteShort((short)Restricted);
            evtWriter.WriteShort((short)RestrictCpt);
            evtWriter.WriteShort(Identifier);
            evtWriter.WriteShort(Undo);

            foreach (Condition cond in Conditions)
                cond.WriteMFA(evtWriter);

            foreach (Action act in Actions)
                act.WriteMFA(evtWriter);

            writer.WriteShort((short)(evtWriter.Tell() * -1));
            writer.WriteWriter(evtWriter);
            evtWriter.Flush();
            evtWriter.Close();
        }
    }
}
