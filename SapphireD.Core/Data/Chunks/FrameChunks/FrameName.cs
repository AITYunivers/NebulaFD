using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameName : StringChunk
    {
        public FrameName()
        {
            ChunkName = "FrameName";
            ChunkID = 0x3335;
        }

        public override void ReadCCN(ByteReader reader)
        {
            base.ReadCCN(reader);

            Frame.curFrame.FrameName = Value;
        }
    }
}
