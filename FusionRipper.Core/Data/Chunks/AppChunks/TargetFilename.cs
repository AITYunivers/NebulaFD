using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
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

            FRipCore.PackageData.TargetFilename = Value;
        }
    }
}
