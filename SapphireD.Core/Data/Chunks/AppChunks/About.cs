using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
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

            SapDCore.PackageData.About = Value;
        }
    }
}
