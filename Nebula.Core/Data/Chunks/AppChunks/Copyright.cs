using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class Copyright : StringChunk
    {
        public Copyright()
        {
            ChunkName = "Copyright";
            ChunkID = 0x223B;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            NebulaCore.PackageData.Copyright = Value;
        }
    }
}
