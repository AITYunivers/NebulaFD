using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class Condition : Chunk
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
        public short ObjectInfoList;
        public Parameter[] Parameters = new Parameter[0];
        public byte DefType;
        public short Identifier;

        public Condition()
        {
            ChunkName = "Condition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();
            Identifier = reader.ReadShort();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ReadCCN(reader, extraInfo);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter condWriter = new ByteWriter(new MemoryStream());
            condWriter.WriteShort(ObjectType);
            condWriter.WriteShort(Num);
            condWriter.WriteShort(ObjectInfo);
            condWriter.WriteShort(ObjectInfoList);
            condWriter.WriteByte((byte)EventFlags.Value);
            condWriter.WriteByte((byte)OtherFlags.Value);
            condWriter.WriteByte((byte)Parameters.Length);
            condWriter.WriteByte(DefType);
            condWriter.WriteShort(Identifier);

            foreach (Parameter parameter in Parameters)
                parameter.WriteMFA(condWriter);

            writer.WriteUShort((ushort)condWriter.Tell());
            writer.WriteWriter(condWriter);
            condWriter.Flush();
            condWriter.Close();
        }
    }
}
