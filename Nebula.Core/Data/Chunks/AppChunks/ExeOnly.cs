using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class ExeOnly : BoolChunk
    {
        public ExeOnly()
        {
            ChunkName = "ExeOnly";
            ChunkID = 0x2224;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);
            NebulaCore.PackageData.ExeOnly = Value;
        }
    }
}
