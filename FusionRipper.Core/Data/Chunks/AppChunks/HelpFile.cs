using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
{
    public class HelpFile : StringChunk
    {
        public HelpFile()
        {
            ChunkName = "HelpFile";
            ChunkID = 0x2230;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            FRipCore.PackageData.HelpFile = Value;
        }
    }
}
