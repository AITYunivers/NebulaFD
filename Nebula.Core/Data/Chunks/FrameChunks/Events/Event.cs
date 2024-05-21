using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
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

        public List<Condition> Conditions = new List<Condition>();
        public List<Action> Actions = new List<Action>();

        public int Restricted;
        public int RestrictCpt;
        public short Identifier;
        public short Undo;

        public FrameEvents Parent = null;

        public Event()
        {
            ChunkName = "Event";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadShort());

            byte cndCnt = reader.ReadByte();
            byte actCnt = reader.ReadByte();
            EventFlags.Value = reader.ReadUShort();
            Parent = (FrameEvents)extraInfo[0];

            if (NebulaCore.Build >= 284 && !(NebulaCore.MFA || NebulaCore.Android && NebulaCore.Build == 287))
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

            for (int i = 0; i < cndCnt; i++)
            {
                Condition cnd = new Condition();
                cnd.ReadCCN(reader, Conditions, this);
                if (cnd.DoAdd)
                {
                    Conditions.Add(cnd);
                    this.Log($"[COND] Type: {cnd.ObjectType}, Num: {cnd.Num}, Params: {cnd.Parameters.Length}");
                }
            }

            for (int i = 0; i < actCnt; i++)
            {
                Action act = new Action();
                act.ReadCCN(reader, Actions, this);
                if (act.DoAdd)
                {
                    Actions.Add(act);
                    this.Log($"[ACT] Type: {act.ObjectType}, Num: {act.Num}, Params: {act.Parameters.Length}");
                }
            }

            reader.Seek(endPosition);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadShort());

            byte cndCnt = reader.ReadByte();
            byte actCnt = reader.ReadByte();
            EventFlags.Value = reader.ReadUShort();
            Restricted = reader.ReadShort();
            RestrictCpt = reader.ReadShort();
            Identifier = reader.ReadShort();
            Undo = reader.ReadShort();

            for (int i = 0; i < cndCnt; i++)
            {
                Condition cnd = new Condition();
                cnd.ReadMFA(reader, Conditions, this);
                Conditions.Add(cnd);
                this.Log($"[COND] Type: {cnd.ObjectType}, Num: {cnd.Num}, Params: {cnd.Parameters.Length}");
            }

            for (int i = 0; i < actCnt; i++)
            {
                Action act = new Action();
                act.ReadMFA(reader, Actions, this);
                Actions.Add(act);
                this.Log($"[ACT] Type: {act.ObjectType}, Num: {act.Num}, Params: {act.Parameters.Length}");
            }

            reader.Seek(endPosition);
            Parent = (FrameEvents)extraInfo[0];
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter evtWriter = new ByteWriter(new MemoryStream());
            evtWriter.WriteByte((byte)Conditions.Count);
            evtWriter.WriteByte((byte)Actions.Count);
            evtWriter.WriteUShort((ushort)EventFlags.Value);
            evtWriter.WriteShort((short)Restricted);
            evtWriter.WriteShort((short)RestrictCpt);
            evtWriter.WriteShort(Identifier);
            evtWriter.WriteShort(Undo);

            foreach (Condition cond in Conditions)
                cond.WriteMFA(evtWriter);

            foreach (Action act in Actions)
                act.WriteMFA(evtWriter);

            writer.WriteShort((short)((evtWriter.Tell() + 2) * -1));
            writer.WriteWriter(evtWriter);
            evtWriter.Flush();
            evtWriter.Close();
        }
    }
}
