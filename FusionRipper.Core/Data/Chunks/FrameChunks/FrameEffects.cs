using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks
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

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            InkEffect = reader.ReadInt();
            InkEffectParams = reader.ReadInt();

            ((Frame)extraInfo[0]).FrameEffects = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
