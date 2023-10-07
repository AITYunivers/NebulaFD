using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks
{
    public class MusicOffsets : Chunk
    {
        public MusicOffsets()
        {
            ChunkName = "MusicOffsets";
            ChunkID = 0x5558;
        }

        public override void ReadCCN(ByteReader reader)
        {
            SapDCore.PackageData.MusicOffsets = this;
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
