using Nebula.Core.Data.Chunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace Nebula.Core.Data.PackageReaders
{
    public class ANMPackageData : PackageData
    {
        public override void Read(ByteReader reader)
        {
            this.Log($"Running build '{NebulaCore.GetCommitHash()}'");
            Header = reader.ReadAscii(4);
            this.Log("Header: " + Header);

            RuntimeVersion = reader.ReadShort();
            RuntimeSubversion = reader.ReadShort();
            ProductVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            NebulaCore.Build = 295;
            NebulaCore.Plus = true;
            this.Log("Fusion Build: " + ProductBuild);

            reader.Skip(4);
            int ChunkCount = reader.ReadInt();
            for (int i = 0; i < ChunkCount; i++)
            {
                var newChunk = Chunk.InitChunk(reader);
                this.Log($"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader);
            }
        }
    }
}
