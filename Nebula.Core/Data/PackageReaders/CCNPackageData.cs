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
        public override void Read(ByteReader reader)
        {
            this.Log($"Running {NebulaCore.BuildDate} build.");
            if (NebulaCore.Fusion == 1.1f)
                return;

            Header = reader.ReadAscii(4);
            if ((Header == "PAMU" || Header == "PAME") && !Parameters.DontUseHeader)
                NebulaCore._yunicode = Header != "PAME";
            if (Header == "CRUF" && !Parameters.DontUseHeader)
                NebulaCore.Fusion = 3f;
            this.Log("Game Header: " + Header);

            RuntimeVersion = reader.ReadShort();
            RuntimeSubversion = reader.ReadShort();
            ProductVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            NebulaCore.Build = ProductBuild;
            if (RuntimeVersion != 769)
            {
                if (NebulaCore.Build < 280)
                    NebulaCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);
                this.Log("Fusion Build: " + ProductBuild + "(Fusion " + NebulaCore.Fusion + ")");
            }
            else
            {
                NebulaCore.Fusion = 1.5f;
                this.Log("Fusion 1.5");
            }



            Frames = new List<Frame>();
            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                this.Log($"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    NebulaCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    NebulaCore.Plus = true;

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader);
                if (!(NebulaCore.Fusion == 1.5f && newChunk.ChunkID >= 0x6666 && newChunk.ChunkID <= 0x6669))
                    newChunk.ChunkData = new byte[0];
            }
            reader.Seek(reader.Size());
        }
    }
}
