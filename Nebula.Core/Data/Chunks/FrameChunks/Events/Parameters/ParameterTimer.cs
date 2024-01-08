using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
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

        public override string ToString()
        {
            TimeSpan tS = TimeSpan.FromMilliseconds(Timer);
            string output = string.Empty;
            if (tS.Hours > 0)
                output += tS.Hours.ToString("D2") + ":";
            if (tS.Minutes > 0)
                output += tS.Minutes.ToString("D2") + "'";
            output += tS.Seconds.ToString("D2") + "''";
            output += "-" + tS.Milliseconds.ToString("D2").Substring(0, 2);
            return output;
        }
    }
}
