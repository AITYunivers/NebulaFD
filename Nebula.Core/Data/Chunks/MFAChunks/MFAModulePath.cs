using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAModulePath : Chunk
    {
        public string ModulePath = "Modules";

        public MFAModulePath()
        {
            ChunkName = "MFAModulePath";
            ChunkID = 0x00F5;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ((PackageData)extraInfo[0]).ModulesDir = ModulePath = reader.ReadAutoYuniversal();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            {
                chunkWriter.WriteAutoYunicode(ModulePath);
            }

            writer.WriteByte((byte)ChunkID);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
