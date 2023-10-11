using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
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

            SapDCore.PackageData.TargetFilename = Value;
        }
    }
}
