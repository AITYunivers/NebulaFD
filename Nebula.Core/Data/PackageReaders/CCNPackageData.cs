using Nebula.Core.Data.Chunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.PackageReaders
{
    public class CCNPackageData : PackageData
    {
        public override void Read(ByteReader reader)
        {
            this.Log($"Running build '{NebulaCore.GetCommitHash()}'");
            if (NebulaCore.Fusion == 1.1f)
                return;

            Header = reader.ReadAscii(4);
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
                this.Log("Fusion Build: " + ProductBuild + " (Fusion " + NebulaCore.Fusion + ")");
            }
            else
            {
                NebulaCore.Fusion = 1.5f;
                this.Log("Fusion 1.5");
            }

            if (Parameters.ForceUnicode)
                NebulaCore._yunicode = true;
            else if (!Parameters.IgnoreHeader)
                NebulaCore._yunicode = Header != "PAME";

            Frames = new List<Frame>();
            bool HasExtendedHeader = false;

            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                this.Log($"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    NebulaCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    NebulaCore.Plus = true;
                if (newChunk.ChunkID == 0x2245)
                    HasExtendedHeader = true;

                if (newChunk.ChunkID == 0x3333 && !HasExtendedHeader)
                {
                    // We've parsed all the chunks we could before the Frame chunk
                    // but could not find the ExtendedHeader, so we have no platform info
                    // Default to a Windows build.
                    this.Log("No ExtendedHeader chunk found, defaulting to a Windows build.");
                    NebulaCore.Windows = true;
                }

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader);
                if (!(NebulaCore.Fusion == 1.5f && newChunk.ChunkID >= 0x6666 && newChunk.ChunkID <= 0x6669))
                    newChunk.ChunkData = new byte[0];
            }
            reader.Seek(reader.Size());
        }
    }
}
