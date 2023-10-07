using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameEffects : Chunk
    {
        public int InkEffect;
        public int InkEffectParams;

        public FrameEffects()
        {
            ChunkName = "FrameEffects";
            ChunkID = 0x3349;
        }

        public override void ReadCCN(ByteReader reader)
        {
            InkEffect = reader.ReadInt();
            InkEffectParams = reader.ReadInt();

            Frame.curFrame.FrameEffects = this;
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
