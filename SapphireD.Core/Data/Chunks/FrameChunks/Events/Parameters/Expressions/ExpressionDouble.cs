using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionDouble : ExpressionChunk
    {
        public double Value;
        public float Value2;

        public ExpressionDouble()
        {
            ChunkName = "ExpressionDouble";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadDouble();
            Value2 = reader.ReadFloat();
        }
    }
}
