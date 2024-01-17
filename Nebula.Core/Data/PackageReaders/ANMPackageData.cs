using Nebula.Core.Data.Chunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace Nebula.Core.Data.PackageReaders
{
    public class ANMPackageData : PackageData
    {
        public override void Read()
        {
            Logger.Log(this, $"Running {NebulaCore.BuildDate} build.");
            Header = Reader.ReadAscii(4);
            Logger.Log(this, "Header: " + Header);

            RuntimeVersion = Reader.ReadInt16();
            RuntimeSubversion = Reader.ReadInt16();
            ProductVersion = Reader.ReadInt32();
            ProductBuild = Reader.ReadInt32();
            NebulaCore.Build = 295;
            NebulaCore.Plus = true;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            Reader.Skip(4);
            int ChunkCount = Reader.ReadInt32();
            for (int i = 0; i < ChunkCount; i++)
            {
                var newChunk = Chunk.InitChunk(Reader);
                Logger.Log(this, $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader);
            }
        }
    }
}
