using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameSeed : ShortChunk
    {
        public FrameSeed()
        {
            ChunkName = "FrameSeed";
            ChunkID = 0x3344;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            ((Frame)extraInfo[0]).FrameSeed = Value;
        }
    }
}
