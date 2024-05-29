using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class AppName : StringChunk
    {
        public AppName()
        {
            ChunkName = "AppName";
            ChunkID = 0x2224;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            NebulaCore.PackageData.AppName = Value;
        }
    }
}
