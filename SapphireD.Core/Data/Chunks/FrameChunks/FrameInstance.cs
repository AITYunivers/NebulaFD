using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameInstance : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public uint Handle;
        public uint ObjectInfo;
        public int PositionX;
        public int PositionY;
        public uint ParentType;
        public uint ParentHandle;
        public uint Layer;
        public short InstanceValue;

        public FrameInstance()
        {
            ChunkName = "FrameInstance";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUShort();
            ObjectInfo = reader.ReadUShort();
            PositionX = reader.ReadInt();
            PositionY = reader.ReadInt();
            ParentType = reader.ReadUShort();
            ParentHandle = reader.ReadUShort();
            Layer = reader.ReadUShort();
            InstanceValue = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            PositionX = reader.ReadInt();
            PositionY = reader.ReadInt();
            Layer = reader.ReadUInt();
            Handle = reader.ReadUInt();
            Flags.Value = reader.ReadUShort();
            InstanceValue = reader.ReadShort();
            ParentType = reader.ReadUInt();
            ObjectInfo = reader.ReadUInt();
            ParentHandle = reader.ReadUInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
