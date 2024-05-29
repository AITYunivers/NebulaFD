using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class TargetFilename : StringChunk
    {
        public TargetFilename()
        {
            ChunkName = "TargetFilename";
            ChunkID = 0x222F;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            NebulaCore.PackageData.TargetFilename = Value;
        }
    }
}
