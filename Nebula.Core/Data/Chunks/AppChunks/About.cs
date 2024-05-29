using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class About : StringChunk
    {
        public About()
        {
            ChunkName = "About";
            ChunkID = 0x223A;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            NebulaCore.PackageData.About = Value;
        }
    }
}
