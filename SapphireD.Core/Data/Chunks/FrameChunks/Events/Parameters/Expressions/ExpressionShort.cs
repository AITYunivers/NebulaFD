using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionShort : ExpressionChunk
    {
        public short Value;

        public ExpressionShort()
        {
            ChunkName = "ExpressionShort";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadShort();
        }
    }
}
