using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameTransitionOut : TransitionChunk
    {
        public FrameTransitionOut()
        {
            ChunkName = "FrameTransitionOut";
            ChunkID = 0x333C;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            ((Frame)extraInfo[0]).FrameTransitionOut = this;
        }
    }
}
