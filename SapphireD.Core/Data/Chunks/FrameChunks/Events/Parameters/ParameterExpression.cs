using SapphireD.Core.Memory;
using System.Diagnostics;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExpression : ParameterChunk
    {
        public short ObjectType;
        public short Num;
        public ushort Size;
        public ExpressionChunk Expression = new();
        public ushort ObjectInfo;
        public short ObjectInfoList;

        public ParameterExpression()
        {
            ChunkName = "ParameterExpression";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long Debut = reader.Tell();
            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();

            if (ObjectType == 0 && Num == 0)
                return;

            Size = reader.ReadUShort();
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
            else if (ObjectType == 0)
                Expression = new ExpressionExtension();

            Debug.Assert(Size >= 6);
            Expression.ReadCCN(reader, Size - 6);
            reader.Seek(Debut + Size);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(ObjectType);
            writer.WriteShort(Num);

            if (ObjectType == 0 && Num == 0)
                return;

            ByteWriter expWriter = new ByteWriter(new MemoryStream());
            if (ObjectType > 1 || ObjectType == -7)
            {
                expWriter.WriteUShort(ObjectInfo);
                expWriter.WriteShort(ObjectInfoList);
            }

            Expression.WriteMFA(expWriter);

            writer.WriteUShort((ushort)(expWriter.Tell() + 6));
            Debug.Assert(expWriter.Tell() + 6 == Size);
            writer.WriteWriter(expWriter);

            expWriter.Flush();
            expWriter.Close();
        }
    }
}
