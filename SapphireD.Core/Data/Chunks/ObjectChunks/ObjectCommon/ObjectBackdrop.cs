using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectBackdrop : ObjectProperties
    {
        public int Size;
        public uint ObstacleType;
        public uint CollisionType;
        public int Width;
        public int Height;
        public int Image;

        public ObjectBackdrop()
        {
            ChunkName = "ObjectBackdrop";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Size = reader.ReadInt();
            ObstacleType = reader.ReadUShort();
            CollisionType = reader.ReadUShort();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Image = reader.ReadShort();

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
