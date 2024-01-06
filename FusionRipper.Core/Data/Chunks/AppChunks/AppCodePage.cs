using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
{
    public class AppCodePage : IntChunk
    {
        public AppCodePage()
        {
            ChunkName = "AppCodePage";
            ChunkID = 0x2246;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            FRipCore.PackageData.AppCodePage = Value;
        }
    }
}
