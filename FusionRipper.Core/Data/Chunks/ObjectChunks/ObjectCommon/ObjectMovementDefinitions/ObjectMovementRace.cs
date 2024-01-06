using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementRace : ObjectMovementDefinition
    {
        public short Speed;
        public short Acceleration;
        public short Deceleration;
        public short Rotation;
        public short BounceMultiplier;
        public short Angles;
        public short Reversable;

        public ObjectMovementRace()
        {
            ChunkName = "ObjectMovementRace";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Acceleration = reader.ReadShort();
            Deceleration = reader.ReadShort();
            Rotation = reader.ReadShort();
            BounceMultiplier = reader.ReadShort();
            Angles = reader.ReadShort();
            Reversable = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Acceleration = reader.ReadShort();
            Deceleration = reader.ReadShort();
            Rotation = reader.ReadShort();
            BounceMultiplier = reader.ReadShort();
            Angles = reader.ReadShort();
            Reversable = reader.ReadShort();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Speed);
            writer.WriteShort(Acceleration);
            writer.WriteShort(Deceleration);
            writer.WriteShort(Rotation);
            writer.WriteShort(BounceMultiplier);
            writer.WriteShort(Angles);
            writer.WriteShort(Reversable);
        }
    }
}
