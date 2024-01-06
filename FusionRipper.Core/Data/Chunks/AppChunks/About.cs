using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
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

            FRipCore.PackageData.About = Value;
        }
    }
}
