using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class Extension : Chunk
    {
        public string Name = string.Empty;
        public string SubType = string.Empty;
        public int MagicNumber;
        public int VersionLs;
        public int VersionMs;
        public short Handle;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long start = reader.Tell();

            short size = Math.Abs(reader.ReadShort());
            Handle = reader.ReadShort();
            MagicNumber = reader.ReadInt();
            VersionLs = reader.ReadInt();
            VersionMs = reader.ReadInt();
            Name = reader.ReadYuniversal();
            int pos = Name.LastIndexOf('.');
            Name = Name.Substring(0, pos);
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

        }
    }
}
