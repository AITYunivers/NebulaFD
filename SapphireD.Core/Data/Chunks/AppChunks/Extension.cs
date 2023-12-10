using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class Extension : Chunk
    {
        public string Name = string.Empty;
        public string FileName = string.Empty;
        public string SubType = string.Empty;
        public int MagicNumber;
        public int VersionLs;
        public int VersionMs;
        public int Handle;

        public Extension()
        {
            ChunkName = "Extension";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long start = reader.Tell();

            short size = Math.Abs(reader.ReadShort());
            Handle = reader.ReadShort();
            MagicNumber = reader.ReadInt();
            VersionLs = reader.ReadInt();
            VersionMs = reader.ReadInt();
            FileName = reader.ReadYuniversal();
            Name = FileName[..FileName.LastIndexOf('.')];
            SubType = reader.ReadYuniversal();
            reader.Seek(start + size);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Handle);
            writer.WriteAutoYunicode(Name);
            writer.WriteAutoYunicode(FileName);
            writer.WriteInt(MagicNumber);
            writer.WriteAutoYunicode(SubType);
            writer.WriteInt(0); // Is Unicode? How set this??
        }
    }
}
