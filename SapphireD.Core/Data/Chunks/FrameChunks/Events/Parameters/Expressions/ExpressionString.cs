using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionString : ExpressionChunk
    {
        public string Value;

        public ExpressionString()
        {
            ChunkName = "ExpressionString";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadYuniversal();
        }
    }
}
