using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionInt : ExpressionChunk
    {
        public int Value;

        public ExpressionInt()
        {
            ChunkName = "ExpressionInt";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadInt();
        }
    }
}
