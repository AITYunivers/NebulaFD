using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunks.Extensions
{
    internal class ExtensionItem : IReadable, IWritable
    {
        public ushort Handle;
        public uint Magic;
        public int VersionLs;
        public int VersionMs;
        public string FileName = string.Empty;
        public string SubType = string.Empty;

        public void Read(ByteReader reader)
        {
            short size = reader.ReadShort();
            Handle = reader.ReadUShort();
            Magic = reader.ReadUInt();
            VersionLs = reader.ReadInt();
            VersionMs = reader.ReadInt();
            FileName = reader.ReadYuniversal();
            SubType = reader.ReadYuniversal();
        }

        public void Write(ByteWriter writer)
        {
            using ByteWriter dataWriter = new ByteWriter();
            dataWriter.WriteUShort(Handle);
            dataWriter.WriteUInt(Magic);
            dataWriter.WriteInt(VersionLs);
            dataWriter.WriteInt(VersionMs);
            dataWriter.WriteYunicode(FileName);
            dataWriter.WriteYunicode(SubType);

            writer.WriteShort((short)(dataWriter.Tell() + 2)); // Maybe inverted??
            writer.WriteWriter(dataWriter);
        }
    }
}
