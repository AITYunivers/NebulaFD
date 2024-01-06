using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionString : ExpressionChunk
    {
        public string Value = string.Empty;

        public ExpressionString()
        {
            ChunkName = "ExpressionString";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadYuniversal();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUnicode(Value, true);
        }
    }
}
