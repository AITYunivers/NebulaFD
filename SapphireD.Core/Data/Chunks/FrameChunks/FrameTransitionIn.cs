using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameTransitionIn : TransitionChunk
    {
        public FrameTransitionIn()
        {
            ChunkName = "FrameTransitionIn";
            ChunkID = 0x333B;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            ((Frame)extraInfo[0]).FrameTransitionIn = this;
        }
    }
}
