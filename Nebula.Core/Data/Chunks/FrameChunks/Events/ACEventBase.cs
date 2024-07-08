using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public class ACEventBase : Chunk
    {
        public BitDict EventFlags = new BitDict( // Flags
            "Repeat",         // Repeat
            "Done",           // Done
            "Default",        // Default
            "DoneBeforeFade", // Done Before Fade In
            "NotDoneInStart", // Not Done In Start
            "Always",         // Always
            "Bad",            // Bad
            "BadObject"       // Bad Object
        );

        public BitDict OtherFlags = new BitDict( // Other Flags
            "Negated", "", "", "", "", // Not
            "NoInterdependence"        // No Object Interdependence
        );

        public short ObjectType;
        public short Num;
        public ushort ObjectInfo;
        public short ObjectInfoList;
        public Parameter[] Parameters = new Parameter[0];
        public byte DefType;

        public Event Parent = null;
        public bool DoAdd = true;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }
    }
}
