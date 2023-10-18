using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionCommon : ExpressionChunk
    {
        public int Value;

        public ExpressionCommon()
        {
            ChunkName = "ExpressionCommon";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4);
            Value = reader.ReadInt();
        }
    }
}
