using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
{
    public class ExeOnly : BoolChunk
    {
        public ExeOnly()
        {
            ChunkName = "AppName";
            ChunkID = 0x2224;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadByte() != 0;

            FRipCore.PackageData.ExeOnly = Value;
        }
    }
}
