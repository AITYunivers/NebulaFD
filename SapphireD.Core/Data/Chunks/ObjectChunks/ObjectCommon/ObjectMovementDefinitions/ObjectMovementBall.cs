using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementBall : ObjectMovementDefinition
    {
        public short Speed;
        public short Bounce;
        public short Angles;
        public short Security;
        public short Decelerate;

        public ObjectMovementBall()
        {
            ChunkName = "ObjectMovementBall";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Bounce = reader.ReadShort();
            Angles = reader.ReadShort();
            Security = reader.ReadShort();
            Decelerate = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Speed = reader.ReadShort();
            Bounce = reader.ReadShort();
            Angles = reader.ReadShort();
            Security = reader.ReadShort();
            Decelerate = reader.ReadShort();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Speed);
            writer.WriteShort(Bounce);
            writer.WriteShort(Angles);
            writer.WriteShort(Security);
            writer.WriteShort(Decelerate);
        }
    }
}
