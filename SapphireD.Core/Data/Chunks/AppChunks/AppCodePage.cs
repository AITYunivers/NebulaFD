using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class AppCodePage : IntChunk
    {
        public AppCodePage()
        {
            ChunkName = "AppCodePage";
            ChunkID = 0x2246;
        }

        public override void ReadCCN(ByteReader reader)
        {
            base.ReadCCN(reader);

            SapDCore.PackageData.AppCodePage = Value;
        }
    }
}
