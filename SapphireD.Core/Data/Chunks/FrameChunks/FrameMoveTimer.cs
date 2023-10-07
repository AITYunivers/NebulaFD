using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameMoveTimer : IntChunk
    {
        public FrameMoveTimer()
        {
            ChunkName = "FrameMoveTimer";
            ChunkID = 0x3347;
        }

        public override void ReadCCN(ByteReader reader)
        {
            base.ReadCCN(reader);

            Frame.curFrame.FrameMoveTimer = Value;
        }
    }
}
