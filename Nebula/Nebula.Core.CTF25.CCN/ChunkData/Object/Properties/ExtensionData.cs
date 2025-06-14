using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class ExtensionData : IReadable, IWritable
    {
        public uint Version;
        public uint ID;
        public uint Private;
        public byte[] Data = [];

        public void Read(ByteReader reader)
        {
            int size = reader.ReadInt();
            reader.Skip(4);
            Version = reader.ReadUInt();
            ID = reader.ReadUInt();
            Private = reader.ReadUInt();
            Data = reader.ReadBytes(size - 20);
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
