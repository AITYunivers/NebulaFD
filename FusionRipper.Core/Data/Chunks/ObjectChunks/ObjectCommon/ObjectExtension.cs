using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectExtension : Chunk
    {
        public int ExtensionVersion;
        public int ExtensionID;
        public int ExtensionPrivate;
        public byte[] ExtensionData = new byte[0];

        public ObjectExtension()
        {
            ChunkName = "ObjectExtension";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int DataSize = reader.ReadInt() - 20;
            reader.Skip(4);
            ExtensionVersion = reader.ReadInt();
            ExtensionID = reader.ReadInt();
            ExtensionPrivate = reader.ReadInt();
            ExtensionData = reader.ReadBytes(DataSize);
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
