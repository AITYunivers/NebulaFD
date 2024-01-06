using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterTimer : ParameterChunk
    {
        public int Timer;
        public int Loops;
        public short Comparison;

        public ParameterTimer()
        {
            ChunkName = "ParameterTimer";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Timer = reader.ReadInt();
            Loops = reader.ReadInt();
            Comparison = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Timer);
            writer.WriteInt(Loops);
            writer.WriteShort(Comparison);
        }
    }
}
