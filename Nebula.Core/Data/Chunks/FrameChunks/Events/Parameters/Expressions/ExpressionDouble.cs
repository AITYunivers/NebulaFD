using Nebula.Core.Memory;
using Nebula.Core.Utilities;

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
            Value = Math.Round(reader.ReadDouble(), 5);
            Value2 = (float)Math.Round(reader.ReadFloat(), 5);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteDouble(Value);
            writer.WriteFloat(Value2);
        }
    }
}
