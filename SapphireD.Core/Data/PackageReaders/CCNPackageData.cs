using SapphireD.Core.Data.Chunks;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using Spectre.Console;

namespace SapphireD.Core.Data.PackageReaders
{
    public class CCNPackageData : PackageData
    {
        public List<Task> ChunkReaders = new();
        public List<Chunk> Chunks = new();

        public int ChunksLoaded;

        public override void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {SapDCore.BuildDate} build.");
            if (SapDCore.Fusion == 1.1f)
                return;

            Header = reader.ReadAscii(4);
            SapDCore._unicode = Header != "PAME";
            Logger.Log(this, "Game Header: " + Header);

            RuntimeVersion = (short)reader.ReadUInt16();
            RuntimeSubversion = (short)reader.ReadUInt16();
            ProductVersion = reader.ReadInt32();
            ProductBuild = reader.ReadInt32();
            SapDCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            if (SapDCore.Build < 280)
                SapDCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);

            Frames = new List<Frame>();
            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                Logger.Log(this, $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    SapDCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    SapDCore.Plus = true;

                Chunks.Add(newChunk);
                int chunkId = Chunks.Count - 1;

                Task chunkReader = new Task(() =>
                {
                    try
                    {
                        ByteReader chunkReader = new ByteReader(Chunks[chunkId].ChunkData!);
                        Chunks[chunkId].ReadCCN(chunkReader);
                    }
                    catch {}
                    Chunks[chunkId].ChunkData = new byte[0];
                    ChunksLoaded++;
                });

                if (newChunk.ChunkID == 0x2224 ||
                    newChunk.ChunkID == 0x223B ||
                    newChunk.ChunkID == 0x222E)
                    chunkReader.RunSynchronously();
                else
                    ChunkReaders.Add(chunkReader);

                ChunksLoaded++;
            }
            foreach (Task chunkReader in ChunkReaders)
                chunkReader.Start();

            foreach (Task chunkReader in ChunkReaders)
                chunkReader.Wait();
        }

        public override void CliUpdate()
        {
            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? chunkReading = ctx.AddTask("[DeepSkyBlue3]Reading chunks[/]", false);
                ProgressTask? imageReading = null;

                while (!chunkReading.IsFinished)
                {
                    if (SapDCore.Fusion == 1.1f)
                        return;

                    if (SapDCore.PackageData != null && Chunks.Count > 0)
                    {
                        if (!chunkReading.IsStarted)
                            chunkReading.StartTask();

                        chunkReading.Value = ChunksLoaded;
                        chunkReading.MaxValue = Chunks.Count * 2;

                        if (SapDCore.PackageData.ImageBank.ImageCount > 0)
                        {
                            if (imageReading == null)
                                imageReading = ctx.AddTask("[DeepSkyBlue3]Reading images[/]", true);

                            imageReading.Value = Data.Chunks.BankChunks.Images.ImageBank.LoadedImageCount;
                            imageReading.MaxValue = SapDCore.PackageData.ImageBank.ImageCount;
                        }
                    }
                }
            });
        }
    }
}
