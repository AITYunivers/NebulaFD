using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class ExtensionData : Chunk
    {
        public int DataSize;
        public byte[] Data = new byte[0];

        public ExtensionData()
        {
            ChunkName = "ExtensionData";
            ChunkID = 0x222C;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            DataSize = reader.ReadInt();
            Data = reader.ReadBytes(DataSize);

            SapDCore.PackageData.ExtensionData = this;
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
