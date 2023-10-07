using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class ImageBank : Chunk
    {
        public Dictionary<uint, Image> Images;

        public ImageBank()
        {
            ChunkName = "ImageBank";
            ChunkID = 0x6666;
        }

        public override void ReadCCN(ByteReader reader)
        {
            Images = new();
            var count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Image img = Image.NewImage();
                img.ReadCCN(reader);
                Images[img.Handle] = img;
            }
            SapDCore.PackageData.ImageBank = this;
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
