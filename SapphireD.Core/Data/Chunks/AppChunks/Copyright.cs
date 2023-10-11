using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
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

            SapDCore.PackageData.Copyright = Value;
        }
    }
}
