using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Sounds
{
    public class SoundOffsets : Chunk
    {
        public SoundOffsets()
        {
            ChunkName = "SoundOffsets";
            ChunkID = 0x5557;
        }

        public override void ReadCCN(ByteReader reader)
        {
            SapDCore.PackageData.SoundOffsets = this;
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
