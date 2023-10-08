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

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            SapDCore.PackageData.MusicOffsets = this;
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
