using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
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
            int dataSize;
            if (NebulaCore.Fusion == 1.5f)
            {
                dataSize = reader.ReadInt() - 12;
                reader.Skip(2);
                ExtensionVersion = reader.ReadShort();
                ExtensionID = reader.ReadShort();
                ExtensionPrivate = reader.ReadShort();
                ExtensionData = reader.ReadBytes(dataSize);
            }
            else
            {
                dataSize = reader.ReadInt() - 20;
                reader.Skip(4);
                ExtensionVersion = reader.ReadInt();
                ExtensionID = reader.ReadInt();
                ExtensionPrivate = reader.ReadInt();
                ExtensionData = reader.ReadBytes(dataSize);
            }
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
