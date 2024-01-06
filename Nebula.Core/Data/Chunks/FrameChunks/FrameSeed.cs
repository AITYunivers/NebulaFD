using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameSeed : IntChunk
    {
        public FrameSeed()
        {
            ChunkName = "FrameSeed";
            ChunkID = 0x3344;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadShort();

            ((Frame)extraInfo[0]).FrameSeed = Value;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader);

            ((Frame)extraInfo[0]).FrameSeed = Value;
        }
    }
}
