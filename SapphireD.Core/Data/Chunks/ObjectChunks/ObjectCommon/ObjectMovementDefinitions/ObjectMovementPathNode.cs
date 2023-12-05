using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementPathNode : ObjectMovementDefinition
    {
        public byte Speed;
        public byte Direction;
        public short Dx;
        public short Dy;
        public short Cos;
        public short Sin;
        public short Length;
        public short Pause;
        public string Name = string.Empty;

        public ObjectMovementPathNode()
        {
            ChunkName = "ObjectMovementPathNode";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Speed = reader.ReadByte();
            Direction = reader.ReadByte();
            Dx = reader.ReadShort();
            Dy = reader.ReadShort();
            Cos = reader.ReadShort();
            Sin = reader.ReadShort();
            Length = reader.ReadShort();
            Pause = reader.ReadShort();
            Name = reader.ReadYuniversal();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteByte(Speed);
            writer.WriteByte(Direction);
            writer.WriteShort(Dx);
            writer.WriteShort(Dy);
            writer.WriteShort(Cos);
            writer.WriteShort(Sin);
            writer.WriteShort(Length);
            writer.WriteShort(Pause);
            writer.WriteUnicode(Name, true);
        }
    }
}
