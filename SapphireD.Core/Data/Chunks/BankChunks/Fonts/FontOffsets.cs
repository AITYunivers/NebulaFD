using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Fonts
{
    public class FontOffsets : Chunk
    {
        public FontOffsets()
        {
            ChunkName = "FontOffsets";
            ChunkID = 0x5556;
        }

        public override void ReadCCN(ByteReader reader)
        {
            SapDCore.PackageData.FontOffsets = this;
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
