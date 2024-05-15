using Nebula.Core.Data.Chunks;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.FileReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace Nebula.Core.Data.PackageReaders
{
    public class CCNPackageData : PackageData
    {
        public override void Read()
        {
            Logger.Log(this, $"Running {NebulaCore.BuildDate} build.");
            if (NebulaCore.Fusion == 1.1f)
                return;

            Header = Reader.ReadAscii(4);
            NebulaCore._yunicode = Header != "PAME";
            if (Header == "CRUF")
                NebulaCore.Fusion = 3f;
            Logger.Log(this, "Game Header: " + Header);

            RuntimeVersion = Reader.ReadShort();
            RuntimeSubversion = Reader.ReadShort();
            ProductVersion = Reader.ReadInt();
            ProductBuild = Reader.ReadInt();
            NebulaCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            if (NebulaCore.Build < 280)
                NebulaCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);

            Frames = new Dictionary<int, Frame>();
            while (Reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(Reader);
                Logger.Log(this, $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    NebulaCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    NebulaCore.Plus = true;

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader);
                newChunk.ChunkData = new byte[0];
            }
            Reader.Seek(Reader.Size());
        }
    }
}
