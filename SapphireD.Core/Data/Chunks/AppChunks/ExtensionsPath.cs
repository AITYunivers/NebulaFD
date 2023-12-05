using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
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

            SapDCore.PackageData.ExtensionsPath = Value;
        }
    }
}
