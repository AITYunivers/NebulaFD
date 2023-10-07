using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class ImageOffsets : Chunk
    {
        public ImageOffsets()
        {
            ChunkName = "ImageOffsets";
            ChunkID = 0x5555;
        }

        public override void ReadCCN(ByteReader reader)
        {
            SapDCore.PackageData.ImageOffsets = this;
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
