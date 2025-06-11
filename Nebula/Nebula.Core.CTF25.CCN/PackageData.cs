using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.Chunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Reflection.PortableExecutable;

namespace Nebula.Core.CTF25.CCN
{
    public class PackageData : IPackageData
    {
        public virtual void Read(ByteReader reader)
        {
            SkipEXEHeader(reader);
            SkipPackData(reader);

            this.Log($"Running alpha build");
            string header = reader.ReadAscii(4);
            reader.SetUnicode(true);
            this.Log("Project Header: " + header);

            if (header != "PAMU")
                throw new InvalidDataException("Invalid project header. Expected PAMU, got " + header);

            ushort runtimeVersion = reader.ReadUShort();
            ushort runtimeSubversion = reader.ReadUShort();
            int productVersion = reader.ReadInt();
            int productBuild = reader.ReadInt();
            this.Log("Fusion Build: " + productBuild);

            while (reader.HasMemory(8))
            {
                ChunkDefinition chunkDefinition = new ChunkDefinition();
                chunkDefinition.Read(reader);

                this.Log($"Found Chunk 0x{chunkDefinition.GetID():X} known as '{chunkDefinition.GetChunkType()}'");
                ReadChunk(chunkDefinition, reader);
            }
        }

        public virtual void ReadChunk(ChunkDefinition chunkDefinition, ByteReader reader)
        {
            CommonChunk? chunk = chunkDefinition.GetChunkType() switch
            {
                EChunks.APP_HEADER => new AppHeaderChunk(),
                _ => null
            };

            if (chunk != null)
            {
                chunk.SetChunkDefinition(chunkDefinition);
                chunk.Read(reader);
            }
        }

        // Later this will be moved
        private void SkipEXEHeader(ByteReader reader)
        {
            reader.Seek(0);
            if (reader.PeekHeader()[..2] != "MZ")
                return;

            reader.Seek(60); // Find PE Header
            reader.Seek(reader.ReadUInt() + 6); // Skip to section header count
            reader.Skip(240 + (reader.ReadUShort() - 1) * 40 + 16); // Skip to the last section header
            reader.Seek(reader.ReadUInt() + reader.ReadUInt()); // Calculate the offset to the CCN header
        }

        // Later this will be removed
        private void SkipPackData(ByteReader reader)
        {
            string header = reader.ReadAscii(8);
            if (header != "wwwwI\u0087G\u0012") // wwwwI‡G
                return;

            reader.Skip(4); // Header Data Size
            reader.Skip(reader.ReadUInt() - 48); // Skip Packed Files
        }

        // Later this will always return false
        public virtual bool Check(ByteReader reader)
        {
            string header = reader.PeekHeader();
            if (header[..2] == "MZ") // Will seperate later
                return ValidifyEXE(reader);
            else if (header == "wwww") // Not descriptive enough, will remove later
                return ValidifyCCNWithPacked(reader);
            else if (header == "PAMU") // Not descriptive enough, will remove later
                return ValidifyCCN(reader);
            return false;
        }
        
        private bool ValidifyEXE(ByteReader reader)
        {
            SkipEXEHeader(reader);
            return ValidifyCCNWithPacked(reader);
        }

        private bool ValidifyCCNWithPacked(ByteReader reader)
        {
            string header = reader.PeekHeader(8);
            if (header == "wwwwI\u0087G\u0012") // wwwwI‡G
            {
                SkipPackData(reader);
                return ValidifyCCN(reader);
            }
            reader.Seek(0); // Reset Position
            return false;
        }

        private bool ValidifyCCN(ByteReader reader)
        {
            string header = reader.ReadAscii(4);
            uint runtimeVersion = reader.ReadUInt();
            reader.Skip(4); // productVersion
            uint productBuild = reader.ReadUInt();
            reader.Seek(0); // Reset Position
            return header == "PAMU" && runtimeVersion == 770 && productBuild >= 280;
        }
    }
}
