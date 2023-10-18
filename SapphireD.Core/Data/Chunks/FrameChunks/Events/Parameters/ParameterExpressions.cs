using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExpressions : ParameterChunk
    {
        public short Comparison;
        public List<ParameterExpression> Expressions = new();

        public ParameterExpressions()
        {
            ChunkName = "ParameterExpressions";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Comparison = reader.ReadShort();

            while (true)
            {
                ParameterExpression newExpression = new ParameterExpression();
                newExpression.ReadCCN(reader);

                if (newExpression.ObjectType == 0 && newExpression.Num == 0)
                    break;

                Expressions.Add(newExpression);
            }
        }
    }
}
