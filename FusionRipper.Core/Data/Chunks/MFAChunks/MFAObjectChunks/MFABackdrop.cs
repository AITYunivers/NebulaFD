using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFABackdrop : MFAObjectLoader
    {
        public uint ObstacleType;
        public uint CollisionType;
        public uint Image;

        public MFABackdrop()
        {
            ChunkName = "MFABackdrop";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ObstacleType = reader.ReadUInt();
            CollisionType = reader.ReadUInt();
            Image = reader.ReadUInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUInt(ObstacleType);
            writer.WriteUInt(CollisionType);
            writer.WriteUInt(Image);
        }
    }
}
