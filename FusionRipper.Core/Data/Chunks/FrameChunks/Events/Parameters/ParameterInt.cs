using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterInt : ParameterChunk
    {
        public int Value;

        public ParameterInt()
        {
            ChunkName = "ParameterInt";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Value);
        }
    }
}
