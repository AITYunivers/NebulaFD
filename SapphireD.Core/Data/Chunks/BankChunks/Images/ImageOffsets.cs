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

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            SapDCore.PackageData.ImageOffsets = this;
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
