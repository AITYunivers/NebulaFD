using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
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

            NebulaCore.PackageData.ExeOnly = Value;
        }
    }
}
