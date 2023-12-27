using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class Action : Chunk
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
        public short ObjectInfo;
        public ushort ObjectInfoList;
        public Parameter[] Parameters = new Parameter[0];
        public byte DefType;

        public Action()
        {
            ChunkName = "Action";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectInfoList = reader.ReadUShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
            Fix();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectInfoList = reader.ReadUShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter actWriter = new ByteWriter(new MemoryStream());
            actWriter.WriteShort(ObjectType);
            actWriter.WriteShort(Num);
            actWriter.WriteShort(ObjectInfo);
            actWriter.WriteUShort(ObjectInfoList);
            actWriter.WriteByte((byte)EventFlags.Value);
            actWriter.WriteByte((byte)OtherFlags.Value);
            actWriter.WriteByte((byte)Parameters.Length);
            actWriter.WriteByte(DefType);

            foreach (Parameter parameter in Parameters)
                parameter.WriteMFA(actWriter);

            writer.WriteUShort((ushort)(actWriter.Tell() + 2));
            writer.WriteWriter(actWriter);
            actWriter.Flush();
            actWriter.Close();
        }

        public void Fix()
        {

        }
    }
}
