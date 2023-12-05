using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementPlatform : ObjectMovementDefinition
    {
        public short Speed;
        public short Acceleration;
        public short Deceleration;
        public short JumpControl;
        public short Gravity;
        public short Jump;

        public ObjectMovementPlatform()
        {
            ChunkName = "ObjectMovementPlatform";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Acceleration = reader.ReadShort();
            Deceleration = reader.ReadShort();
            JumpControl = reader.ReadShort();
            Gravity = reader.ReadShort();
            Jump = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Acceleration = reader.ReadShort();
            Deceleration = reader.ReadShort();
            JumpControl = reader.ReadShort();
            Gravity = reader.ReadShort();
            Jump = reader.ReadShort();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Speed);
            writer.WriteShort(Acceleration);
            writer.WriteShort(Deceleration);
            writer.WriteShort(JumpControl);
            writer.WriteShort(Gravity);
            writer.WriteShort(Jump);
        }
    }
}
