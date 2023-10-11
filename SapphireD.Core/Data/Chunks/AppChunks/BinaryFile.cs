using SapphireD.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireD.Core.Data.Chunks.AppChunks
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
            short fileNameLength = reader.ReadShort();
            FileName = reader.ReadYuniversal(fileNameLength);
            int fileDataLength = reader.ReadInt();
            FileData = reader.ReadBytes(fileDataLength);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
