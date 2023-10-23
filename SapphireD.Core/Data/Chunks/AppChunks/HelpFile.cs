using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
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

            SapDCore.PackageData.HelpFile = Value;
        }
    }
}
