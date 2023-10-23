using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class EventObject : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int Handle;
        public ushort ObjectType;
        public ushort ItemType;
        public string Name = string.Empty;
        public string TypeName = string.Empty;
        public uint ItemHandle;
        public uint InstanceHandle;
        public string Code = string.Empty;
        public byte[] IconBuffer = new byte[0];
        public ushort SystemQualifier;

        public EventObject()
        {
            ChunkName = "EventObject";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadInt();
            ObjectType = reader.ReadUShort();
            ItemType = reader.ReadUShort();
            Name = reader.ReadAutoYuniversal();
            TypeName = reader.ReadAutoYuniversal();
            Flags.Value = reader.ReadUShort();

            switch (ObjectType)
            {
                case 1:
                    ItemHandle = reader.ReadUInt();
                    InstanceHandle = reader.ReadUInt();
                    break;
                case 2:
                    Code = reader.ReadAscii(4);
                    if (Code == "OIC2")
                        IconBuffer = reader.ReadBytes(reader.ReadInt());
                    break;
                case 3:
                    SystemQualifier = reader.ReadUShort();
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
