using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.BankChunks.Images
{
    public class ImageOffsets : Chunk
    {
        public ImageOffsets()
        {
            ChunkName = "ImageOffsets";
            ChunkID = 0x5555;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            FRipCore.PackageData.ImageOffsets = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
