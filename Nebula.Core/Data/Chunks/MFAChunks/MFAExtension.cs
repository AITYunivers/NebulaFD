using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAExtension : Chunk
    {
        public int Handle;
        public string Name = string.Empty;
        public string FileName = string.Empty;
        public int Magic;
        public string SubType = string.Empty;

        public MFAExtension()
        {
            ChunkName = "MFAExtension";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadInt();
            Name = reader.ReadAutoYuniversal();
            FileName = reader.ReadAutoYuniversal();
            Magic = reader.ReadInt();
            SubType = reader.ReadAutoYuniversal();
            reader.Skip(4);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
