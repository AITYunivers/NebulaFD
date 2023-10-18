using SapphireD.Core.Data.Chunks.FrameChunks.Events;
using SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameEvents : Chunk
    {
        public BitDict OptionFlags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

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
            Version = reader.ReadUShort();
            FrameType = reader.ReadUShort();
            while (true)
            {
                string identifier = reader.ReadAscii(4);

                if (identifier == "Evts")
                {
                    long endPosition = reader.Tell() + reader.ReadInt();
                    while (reader.Tell() < endPosition)
                    {
                        Event newEvent = new Event();
                        newEvent.ReadMFA(reader);
                        Events.Add(newEvent);
                    }
                }
                else if (identifier == "Rems")
                {
                    Comments = new Comment[reader.ReadInt()];
                    for (int i = 0; i < Comments.Length; i++)
                    {
                        Comments[i] = new Comment();
                        Comments[i].ReadMFA(reader);
                    }
                }
                else if (identifier == "EvOb")
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
                else if (identifier == "EvEd")
                {
                    reader.Skip(2);
                    TimeListData = reader.ReadBytes(reader.ReadUShort() * 6);
                    reader.Skip(2);
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
                else if (identifier == "E2Ts")
                    reader.Skip(reader.ReadInt());
                else if (identifier == "!DNE")
                    break;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
