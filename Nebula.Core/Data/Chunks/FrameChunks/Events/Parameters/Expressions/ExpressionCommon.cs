using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
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

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(0);
            writer.WriteInt(Value);
        }
    }
}
