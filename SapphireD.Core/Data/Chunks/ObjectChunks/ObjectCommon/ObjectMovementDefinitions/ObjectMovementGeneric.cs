using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementGeneric : ObjectMovementDefinition
    {
        public short Speed;
        public short Acceleration;
        public short Deceleration;
        public short BounceMultiplier;
        public int Direction;

        public ObjectMovementGeneric()
        {
            ChunkName = "ObjectMovementGeneric";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Acceleration = reader.ReadShort();
            Deceleration = reader.ReadShort();
            BounceMultiplier = reader.ReadShort();
            Direction = reader.ReadInt();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Acceleration = reader.ReadShort();
            Deceleration = reader.ReadShort();
            BounceMultiplier = reader.ReadShort();
            Direction = reader.ReadInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Speed);
            writer.WriteShort(Acceleration);
            writer.WriteShort(Deceleration);
            writer.WriteShort(BounceMultiplier);
            writer.WriteInt(Direction);
        }
    }
}
