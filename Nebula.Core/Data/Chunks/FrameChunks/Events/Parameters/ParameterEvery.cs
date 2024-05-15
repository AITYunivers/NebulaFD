using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterEvery : ParameterChunk
    {
        public int Delay;
        public int Compteur;

        public ParameterEvery()
        {
            ChunkName = "ParameterEvery";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Delay = reader.ReadInt();
            Compteur = reader.ReadInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Delay);
            writer.WriteInt(Compteur);
        }

        public override string ToString()
        {
            return "Every " + Delay + ", " + Compteur;
        }
    }
}
