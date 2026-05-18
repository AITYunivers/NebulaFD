using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionDouble : ExpressionChunk
    {
        public double Value;
        public float Value2;
        public double Value3;

        public ExpressionDouble()
        {
            ChunkName = "ExpressionDouble";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (NebulaCore.Windows)
            {
                Value = reader.ReadDouble();
                Value2 = reader.ReadFloat();
            }
            else
            {
                reader.Skip(8); // skipping 8 bytes for getting real value
                Value2 = reader.ReadFloat(); // fully reads float expression
                Value = Math.Round((double)Value2, 5); // rounding the value for preventing a lot of floating-point numbers
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteDouble(Value);
            writer.WriteFloat(Value2);
        }
    }
}
