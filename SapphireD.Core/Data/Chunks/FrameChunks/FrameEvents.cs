using SapphireD.Core.Data.Chunks.FrameChunks.Events;
using SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameEvents : Chunk
    {
        public BitDict OptionFlags = new BitDict( // Option Flags
            "BreakChild" // Break Child
        );

        public short MaxObjects;
        public short MaxObjectInfos;
        public short NumberOfPlayers;
        public short[] ConditionCount = new short[17];
        public Qualifier[] Qualifiers = new Qualifier[0];

        public int EventCount;
        public List<Event> Events = new();

        // For MFA
        public ushort Version;
        public ushort FrameType;
        public Comment[] Comments = new Comment[0];
        public EventGroup[] EventGroups = new EventGroup[0];
        public EventObject[] EventObjects = new EventObject[0];
        public int EditorData;
        public ushort ConditionWidth;
        public short ObjectHeight;
        public ushort[] ObjectTypes = new ushort[0];
        public ushort[] ObjectHandles = new ushort[0];
        public ushort[] ObjectFlags = new ushort[0];
        public string[] Folders = new string[0];
        public byte[] TimeListData = new byte[0];
        public uint EditorX;
        public uint EditorY;
        public uint EditorCaretType;
        public uint EditorCaretX;
        public uint EditorCaretY;
        public uint EditorLineY;
        public uint EditorLineType;
        public uint EventLineY;
        public uint EventLineType;

        public FrameEvents()
        {
            ChunkName = "FrameEvents";
            ChunkID = 0x333D;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (SapDCore.Fusion < 2.5f) return;
            while (true)
            {
                string identifier = reader.ReadAscii(4);

                if (identifier == "ER>>")
                {
                    MaxObjects = reader.ReadShort();
                    MaxObjectInfos = reader.ReadShort();
                    NumberOfPlayers = reader.ReadShort();

                    for (int i = 0; i < ConditionCount.Length; i++)
                        ConditionCount[i] = reader.ReadShort();

                    Qualifiers = new Qualifier[reader.ReadShort()];
                    for (int i = 0; i < Qualifiers.Length; i++)
                    {
                        Qualifiers[i] = new Qualifier();
                        Qualifiers[i].ReadCCN(reader);
                    }
                }
                else if (identifier == "ERes")
                    EventCount = reader.ReadInt();
                else if (identifier == "ERev")
                {
                    long endPosition = reader.Tell() + reader.ReadInt();
                    if (SapDCore.Android || SapDCore.HTML)
                        reader.Skip(4);
                    while (reader.Tell() < endPosition)
                    {
                        Event newEvent = new Event();
                        newEvent.ReadCCN(reader);
                        Events.Add(newEvent);
                    }
                }
                else if (identifier == "ERop")
                    OptionFlags.Value = reader.ReadUInt();
                else if (identifier == "<<ER")
                    break;
            }

            ((Frame)extraInfo[0]).FrameEvents = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endOffset = 0;
            if (extraInfo.Length == 0)
            {
                Version = reader.ReadUShort();
                FrameType = reader.ReadUShort();
            }
            else
            {
                endOffset = reader.ReadUInt();
                reader.Skip(endOffset);
                return;
                if (endOffset == 0) return;
                else endOffset += reader.Tell() - 4;
            }

            while (true)
            {
                string identifier = reader.ReadAscii(4);

                if (identifier == "Evts" || identifier == "STVE")
                {
                    long endPosition = reader.Tell() + reader.ReadInt();
                    while (reader.Tell() < endPosition)
                    {
                        Event newEvent = new Event();
                        newEvent.ReadMFA(reader);
                        Events.Add(newEvent);
                    }
                }
                else if (identifier == "Rems" || identifier == "SMER")
                {
                    Comments = new Comment[reader.ReadInt()];
                    for (int i = 0; i < Comments.Length; i++)
                    {
                        Comments[i] = new Comment();
                        Comments[i].ReadMFA(reader);
                    }
                }
                else if (identifier == "SPRG")
                {
                    EventGroups = new EventGroup[reader.ReadInt()];
                    reader.Skip(4); // Max Handle
                    for (int i = 0; i < EventGroups.Length; i++)
                    {
                        EventGroups[i] = new EventGroup();
                        EventGroups[i].ReadMFA(reader);
                    }
                }
                else if (identifier == "EvOb" || identifier == "SJBO")
                {
                    EventObjects = new EventObject[reader.ReadInt()];
                    for (int i = 0; i < EventObjects.Length; i++)
                    {
                        EventObjects[i] = new EventObject();
                        EventObjects[i].ReadMFA(reader);
                    }
                }
                else if (identifier == "EvCs")
                {
                    EditorData = reader.ReadInt();
                    ConditionWidth = reader.ReadUShort();
                    ObjectHeight = reader.ReadShort();
                    reader.Skip(12);
                }
                else if (identifier == "EvEd")
                {
                    short header = reader.ReadShort();
                    short objectCount = header == -1 ? reader.ReadShort() : header;

                    ObjectTypes = new ushort[objectCount];
                    ObjectHandles = new ushort[objectCount];
                    ObjectFlags = new ushort[objectCount];
                    
                    for (int i = 0; i < objectCount * 3; i++)
                    {
                        if (i < objectCount)
                            ObjectTypes[i] = reader.ReadUShort();
                        else if (i < objectCount * 2)
                            ObjectHandles[i % objectCount] = reader.ReadUShort();
                        else
                            ObjectFlags[i % objectCount] = reader.ReadUShort();
                    }

                    if (header == -1)
                    {
                        Folders = new string[reader.ReadUShort()];
                        for (int i = 0; i < Folders.Length; i++)
                            Folders[i] = reader.ReadAutoYuniversal();
                    }
                }
                else if (identifier == "EvTs")
                {
                    reader.Skip(2);
                    EditorX = reader.ReadUInt();
                    EditorY = reader.ReadUInt();
                    EditorCaretType = reader.ReadUInt();
                    EditorCaretX = reader.ReadUInt();
                    EditorCaretY = reader.ReadUInt();
                }
                else if (identifier == "EvLs")
                {
                    reader.Skip(2);
                    EditorLineY = reader.ReadUInt();
                    EditorLineType = reader.ReadUInt();
                    EventLineY = reader.ReadUInt();
                    EventLineType = reader.ReadUInt();
                }
                else if (identifier == "E2Ts" || identifier == "TYAL")
                    reader.Skip(reader.ReadInt());
                else if (identifier == "!DNE")
                    break;
            }

            if (extraInfo.Length > 0)
                reader.Seek(endOffset);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUShort(1030);
            writer.WriteUShort(0);

            if (Events.Count > 0)
            {
                writer.WriteAscii("Evts");
                ByteWriter evtsWriter = new ByteWriter(new MemoryStream());
                foreach (Event evt in Events)
                    evt.WriteMFA(evtsWriter);
                writer.WriteUInt((uint)evtsWriter.Tell());
                writer.WriteWriter(evtsWriter);
            }

            if (EventObjects.Length > 0)
            {
                writer.WriteAscii("EvOb");
                writer.WriteInt(EventObjects.Length);
                foreach (EventObject obj in EventObjects)
                    obj.WriteMFA(writer);
            }

            if (Comments.Length > 0)
            {
                writer.WriteAscii("Rems");
                writer.WriteInt(Comments.Length);
                foreach (Comment comment in Comments)
                    comment.WriteMFA(writer);
            }

            writer.WriteAscii("EvEd");
            {
                writer.WriteShort(-1);
                writer.WriteShort((short)ObjectTypes.Length);

                foreach (ushort type in ObjectTypes)
                    writer.WriteUShort(type);
                foreach (ushort handle in ObjectHandles)
                    writer.WriteUShort(handle);
                foreach (ushort flag in ObjectFlags)
                    writer.WriteUShort(flag);

                writer.WriteUShort((ushort)Folders.Length);
                foreach (string folder in Folders)
                    writer.WriteAutoYunicode(folder);
            }

            writer.WriteAscii("EvTs");
            {
                writer.WriteInt(10);
                writer.WriteBytes(new byte[18]);
            }

            writer.WriteAscii("EvLs");
            {
                writer.WriteInt(10);
                writer.WriteBytes(new byte[14]);
            }

            writer.WriteAscii("E2Ts");
            {
                writer.WriteInt(8);
                writer.WriteBytes(new byte[8]);
            }

            writer.WriteAscii("EvCs");
            {
                writer.WriteInt(EditorData);
                writer.WriteUShort(ConditionWidth);
                writer.WriteShort(ObjectHeight);
                writer.WriteBytes(new byte[12]);
            }

            writer.WriteAscii("!DNE");
        }
    }
}
