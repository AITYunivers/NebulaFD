using FusionRipper.Core.Memory;
using System.Diagnostics;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
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
            //Debug.Assert(expWriter.Tell() + 6 == Size);
            writer.WriteWriter(expWriter);

            expWriter.Flush();
            expWriter.Close();
        }

        public override string ToString()
        {
            switch (ObjectType)
            {
                default: return "";
                case -4:
                    switch (Num)
                    {
                        default: return "";
                        case 0:
                            return "timer";
                    }
                case -1:
                    switch (Num)
                    {
                        default: return "";
                        case -3:
                            return ", ";
                        case -2:
                            return ")";
                        case -1:
                            return "(";
                        case 0:
                            return ((ExpressionInt)Expression).Value.ToString();
                        case 1:
                            return "Random(";
                        case 3:
                            return '"' + ((ExpressionString)Expression).Value + '"';
                        case 4:
                            return "Str$(";
                        case 5:
                            return "Val(";
                        case 6:
                            return "Appdrive$";
                        case 7:
                            return "Appdir$";
                        case 8:
                            return "Apppath$";
                        case 9:
                            return "Appname$";
                        case 10:
                            return "Sin(";
                        case 11:
                            return "Cos(";
                        case 12:
                            return "Tan(";
                        case 13:
                            return "Sqr(";
                        case 14:
                            return "Log(";
                        case 15:
                            return "Ln(";
                        case 16:
                            return "Hex$(";
                        case 17:
                            return "Bin$(";
                        case 18:
                            return "Exp(";
                        case 19:
                            return "Left$(";
                        case 20:
                            return "Right$(";
                        case 21:
                            return "Mid$(";
                        case 22:
                            return "Len(";
                        case 23:
                            return ((ExpressionDouble)Expression).Value.ToString();
                        case 28:
                            return "Int(";
                        case 29:
                            return "Abs(";
                        case 30:
                            return "Ceil(";
                        case 31:
                            return "Floor(";
                        case 32:
                            return "ACos(";
                        case 33:
                            return "ASin(";
                        case 34:
                            return "ATan(";
                        case 35:
                            return "NOT(";
                        case 36:
                            return "NDropped";
                        case 37:
                            return "Dropped$(";
                    }
                case 0:
                    switch (Num)
                    {
                        default: return "";
                        case 2:
                            return " + ";
                        case 4:
                            return " - ";
                        case 6:
                            return " * ";
                        case 8:
                            return " / ";
                        case 10:
                            return " mod ";
                        case 12:
                            return " pow ";
                        case 14:
                            return " and ";
                        case 16:
                            return " or ";
                        case 18:
                            return " xor ";
                    }
            }
        }
    }
}
