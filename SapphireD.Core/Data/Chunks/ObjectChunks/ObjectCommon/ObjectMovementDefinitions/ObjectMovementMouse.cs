using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementMouse : ObjectMovementDefinition
    {
        public short Dx;
        public short Fx;
        public short Dy;
        public short Fy;
        public short Flags;

        public ObjectMovementMouse()
        {
            ChunkName = "ObjectMovementMouse";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            Dx = reader.ReadShort();
            Fx = reader.ReadShort();
            Dy = reader.ReadShort();
            Fy = reader.ReadShort();
            Flags = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Dx = reader.ReadShort();
            Fx = reader.ReadShort();
            Dy = reader.ReadShort();
            Fy = reader.ReadShort();
            Flags = reader.ReadShort();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
