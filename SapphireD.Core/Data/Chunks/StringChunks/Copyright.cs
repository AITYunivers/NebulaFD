using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.StringChunks
{
    public class Copyright : StringChunk
    {
        public Copyright()
        {
            ChunkName = "Copyright";
            ChunkID = 0x223B;
        }

        public override void ReadCCN(ByteReader reader)
        {
            base.ReadCCN(reader);

            SapDCore.PackageData.Copyright = Value;
        }
    }
}
