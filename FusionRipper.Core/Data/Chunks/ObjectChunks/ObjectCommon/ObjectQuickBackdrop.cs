using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectQuickBackdrop : ObjectInfoProperties
    {
        public int Size;
        public uint ObstacleType;
        public uint CollisionType;
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
            ObstacleType = reader.ReadUShort();
            CollisionType = reader.ReadUShort();
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
