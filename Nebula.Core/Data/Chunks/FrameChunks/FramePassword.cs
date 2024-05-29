using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FramePassword : StringChunk
    {
        public FramePassword()
        {
            ChunkName = "FramePassword";
            ChunkID = 0x3336;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            ((Frame)extraInfo[0]).FramePassword = Value;
        }
    }
}
