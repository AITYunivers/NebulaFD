using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class ImageBank : Chunk
    {
        public int ImageCount = 0;
        public Dictionary<uint, Image> Images;

        public ImageBank()
        {
            ChunkName = "ImageBank";
            ChunkID = 0x6666;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            SapDCore.PackageData.ImageBank = this;

            Images = new();
            ImageCount = reader.ReadInt();
            for (int i = 0; i < ImageCount; i++)
            {
                Image img = Image.NewImage();
                img.ReadCCN(reader);
                Images[img.Handle] = img;
            }
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
