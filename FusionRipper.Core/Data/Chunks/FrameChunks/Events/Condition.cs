using FusionRipper.Core.Memory;
using System.Diagnostics;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events
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

        public bool DoAdd = true;

        public Condition()
        {
            ChunkName = "Condition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadUShort());

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
            Fix((List<Condition>)extraInfo[0]);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadUShort());

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

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter condWriter = new ByteWriter(new MemoryStream());
            condWriter.WriteShort(ObjectType);
            condWriter.WriteShort(Num);
            short oI = ObjectInfo;
            if (oI >> 8 == -128)
            {
                byte qual = (byte)(oI & 0xFF);
                oI = (short)((qual | ((128 - ObjectType) << 8)) & 0xFFFF);
            }
            condWriter.WriteShort(oI);
            condWriter.WriteShort(ObjectInfoList);
            condWriter.WriteByte((byte)EventFlags.Value);
            condWriter.WriteByte((byte)OtherFlags.Value);
            condWriter.WriteByte((byte)Parameters.Length);
            condWriter.WriteByte(DefType);
            condWriter.WriteShort(Identifier);

            foreach (Parameter parameter in Parameters)
                parameter.WriteMFA(condWriter);

            writer.WriteUShort((ushort)(condWriter.Tell() + 2));
            writer.WriteWriter(condWriter);
            condWriter.Flush();
            condWriter.Close();
        }

        private void Fix(List<Condition> evntList)
        {
            switch (ObjectType)
            {
                case -1:
                    switch (Num)
                    {
                        case -25:
                            Num = -24;
                            break;
                        case -28:
                        case -31:
                            Num = -8;
                            break;
                        case -43:
                            DoAdd = false;
                            break;
                    }
                    break;
                case >= 0:
                    switch (Num)
                    {
                        case -25:
                            Num = -27;
                            DoAdd = false; // TODO
                            break;
                        case -42:
                            Num = -27;
                            break;
                    }
                    break;
            }
        }
    }
}
