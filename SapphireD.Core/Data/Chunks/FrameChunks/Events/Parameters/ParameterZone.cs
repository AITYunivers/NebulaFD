using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterZone : ParameterChunk
    {
        public short X1;
        public short Y1;
        public short X2;
        public short Y2;

        public ParameterZone()
        {
            ChunkName = "ParameterZone";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            X1 = reader.ReadShort();
            Y1 = reader.ReadShort();
            X2 = reader.ReadShort();
            Y2 = reader.ReadShort();
        }
    }
}
