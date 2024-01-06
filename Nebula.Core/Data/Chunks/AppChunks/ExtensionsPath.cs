using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
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

            NebulaCore.PackageData.ExtensionsPath = Value;
        }
    }
}
