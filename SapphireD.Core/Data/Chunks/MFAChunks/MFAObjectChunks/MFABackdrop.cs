using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFABackdrop : MFAObjectLoader
    {
        public uint ObstacleType;
        public uint CollisionType;
        public int Handle;

        public MFABackdrop()
        {
            ChunkName = "MFABackdrop";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ObstacleType = reader.ReadUInt();
            CollisionType = reader.ReadUInt();
            Handle = reader.ReadInt();
        }
    }
}
