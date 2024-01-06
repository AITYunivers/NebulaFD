using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
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

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteDouble(Value);
            writer.WriteFloat(Value2);
        }
    }
}
