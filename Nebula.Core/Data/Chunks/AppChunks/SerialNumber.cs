using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class SerialNumber : IntChunk
    {
        public SerialNumber()
        {
            ChunkName = "SerialNumber";
            ChunkID = 0x2237;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            NebulaCore.PackageData.SerialNumber = Value;
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
