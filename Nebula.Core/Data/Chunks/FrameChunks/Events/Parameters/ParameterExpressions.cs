using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExpressions : ParameterChunk
    {
        public short Comparison;
        public List<ParameterExpression> Expressions = new();

        public FrameEvents? FrameEvents;

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
                
                newExpression.Parent = Parent;
                Expressions.Add(newExpression);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Comparison);

            foreach (ParameterExpression expression in Expressions)
                expression.WriteMFA(writer);

            writer.WriteInt(0); // Final Expression
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (ParameterExpression expression in Expressions)
                str += expression.ToString();
            return str.Trim();
        }
    }
}
