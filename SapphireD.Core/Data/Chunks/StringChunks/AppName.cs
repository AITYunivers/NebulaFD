using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.StringChunks
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

            SapDCore.PackageData.AppName = Value;
        }
    }
}
