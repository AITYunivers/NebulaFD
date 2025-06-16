using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.ChunkData.Common;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameTransitionInChunk : CommonChunk
    {
        public CommonTransitionData Data = new CommonTransitionData();

        public override void ReadChunkData(ByteReader reader)
        {
            Data.Read(reader);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            Data.Write(writer);
        }
    }
}
