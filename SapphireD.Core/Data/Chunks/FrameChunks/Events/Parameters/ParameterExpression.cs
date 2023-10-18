using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExpression : ParameterChunk
    {
        public short ObjectType;
        public short Num;
        public ExpressionChunk Expression = new();
        public ushort ObjectInfo;
        public short ObjectInfoList;

        public ParameterExpression()
        {
            ChunkName = "ParameterExpression";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();

            if (ObjectType == 0 && Num == 0)
                return;

            long endPosition = reader.Tell() + reader.ReadUShort() - 4;
            if (ObjectType == -1)
            {
                Expression = Num switch
                {
                    0 => new ExpressionInt(),
                    3 => new ExpressionString(),
                    23 => new ExpressionDouble(),
                    24 or 50 => new ExpressionCommon(),
                    _ => new ExpressionChunk()
                };
            }
            else if (ObjectType > 1 || ObjectType == -7)
            {
                ObjectInfo = reader.ReadUShort();
                ObjectInfoList = reader.ReadShort();

                if (Num == 16 || Num == 19)
                    Expression = new ExpressionShort();
            }

            Expression.ReadCCN(reader);
            reader.Seek(endPosition);
        }
    }
}
