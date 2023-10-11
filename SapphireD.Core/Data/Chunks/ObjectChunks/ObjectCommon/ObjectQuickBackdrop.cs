using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectQuickBackdrop : ObjectProperties
    {
        public int Size;
        public short ObstacleType;
        public short CollisionType;
        public int Width;
        public int Height;
        public ObjectShape Shape = new();

        public ObjectQuickBackdrop()
        {
            ChunkName = "ObjectQuickBackdrop";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Size = reader.ReadInt();
            ObstacleType = reader.ReadShort();
            CollisionType = reader.ReadShort();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Shape.ReadCCN(reader);

            ((ObjectInfo)extraInfo[0]).Properties = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
