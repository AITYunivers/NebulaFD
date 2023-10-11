using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class Author : StringChunk
    {
        public Author()
        {
            ChunkName = "Author";
            ChunkID = 0x2225;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            SapDCore.PackageData.Author = Value;
        }
    }
}
