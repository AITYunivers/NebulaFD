using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class SerialNumber : Chunk
    {
        public byte[]? Data;

        public SerialNumber()
        {
            ChunkName = "SerialNumber";
            ChunkID = 0x2237;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Data = reader.ReadBytes();

            SapDCore.PackageData.SerialNumber = this;
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
