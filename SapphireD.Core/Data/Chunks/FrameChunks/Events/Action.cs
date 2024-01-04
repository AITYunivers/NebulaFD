using SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using System;
using System.Diagnostics;

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

        public bool DoAdd = true;

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
            Fix((List<Action>)extraInfo[0]);
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

        private void Fix(List<Action> evntList)
        {
            switch (ObjectType)
            {
                case -1:
                    switch (Num)
                    {
                        case 43:
                            Num = 0;
                            break;
                    }
                    break;
                case 2:
                    switch (Num)
                    {
                        case 2: // Set X
                            if (Parameters.Length > 1)
                            {
                                Action act = new Action();
                                act.ObjectType = ObjectType;
                                act.Num = Num;
                                act.ObjectInfo = ObjectInfo;
                                act.ObjectInfoList = ObjectInfoList;
                                act.EventFlags = EventFlags;
                                act.OtherFlags = OtherFlags;
                                act.Parameters = new Parameter[1];
                                act.Parameters[0] = Parameters[0];
                                act.DefType = DefType;
                                evntList.Add(act);

                                Num = 3;
                                Parameter param = Parameters[1];
                                Parameters = new Parameter[1];
                                Parameters[0] = param;
                            }
                            break;
                        case 13: // Set Movement
                            {
                                ParameterShort param = (ParameterShort)Parameters[0].Data;
                                ObjectCommon oC = (ObjectCommon)SapDCore.PackageData.FrameItems.Items[ObjectInfo].Properties;
                                string name = oC.ObjectMovements.Movements[param.Value].Name;
                                param.Extra = string.IsNullOrEmpty(name) ? "Movement #" + param.Value : name;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}
