using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class BinaryFile : Chunk
    {
        public string FileName = string.Empty;
        public byte[] FileData = new byte[0];

        public BinaryFile()
        {
            ChunkName = "BinaryFile";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            short fileNameLength = reader.ReadShort();
            FileName = reader.ReadYuniversal(fileNameLength);
            int fileDataLength = reader.ReadInt();
            FileData = reader.ReadBytes(fileDataLength);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            FileName = reader.ReadAutoYuniversal();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(FileName);
        }
    }
}
