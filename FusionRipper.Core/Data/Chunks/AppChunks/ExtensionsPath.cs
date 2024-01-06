using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
{
    public class ExtensionsPath : StringChunk
    {
        public ExtensionsPath()
        {
            ChunkName = "ExtensionsPath";
            ChunkID = 0x2227;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            FRipCore.PackageData.ExtensionsPath = Value;
        }
    }
}
