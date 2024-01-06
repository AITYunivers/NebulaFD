using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks
{
    public class FrameMoveTimer : IntChunk
    {
        public FrameMoveTimer()
        {
            ChunkName = "FrameMoveTimer";
            ChunkID = 0x3347;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            ((Frame)extraInfo[0]).FrameMoveTimer = Value;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader);

            ((Frame)extraInfo[0]).FrameMoveTimer = Value;
        }
    }
}
