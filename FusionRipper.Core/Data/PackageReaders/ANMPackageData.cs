using FusionRipper.Core.Data.Chunks;
using FusionRipper.Core.Data.Chunks.FrameChunks;
using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;
using Spectre.Console;

namespace FusionRipper.Core.Data.PackageReaders
{
    public class ANMPackageData : PackageData
    {
        public List<Task> ChunkReaders = new();
        public List<Chunk> Chunks = new();

        public int ChunksLoaded;

        public override void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {FRipCore.BuildDate} build.");
            Header = reader.ReadAscii(4);
            Logger.Log(this, "Header: " + Header);

            RuntimeVersion = reader.ReadInt16();
            RuntimeSubversion = reader.ReadInt16();
            ProductVersion = reader.ReadInt32();
            ProductBuild = reader.ReadInt32();
            FRipCore.Build = 295;
            FRipCore.Plus = true;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            reader.Skip(4);
            int ChunkCount = reader.ReadInt32();
            for (int i = 0; i < ChunkCount; i++)
            {
                var newChunk = Chunk.InitChunk(reader);
                Logger.Log(this, $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                Chunks.Add(newChunk);
                int chunkId = Chunks.Count - 1;

                Task chunkReader = new Task(() =>
                {
                    //try
                    {
                        ByteReader chunkReader = new ByteReader(Chunks[chunkId].ChunkData!);
                        Chunks[chunkId].ReadCCN(chunkReader);
                    }
                    //catch { }
                    Chunks[chunkId].ChunkData = null;
                    ChunksLoaded++;
                });

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
                    if (FRipCore.Fusion == 1.1f)
                        return;

                    if (FRipCore.PackageData != null && Chunks.Count > 0)
                    {
                        if (!chunkReading.IsStarted)
                            chunkReading.StartTask();

                        chunkReading.Value = ChunksLoaded;
                        chunkReading.MaxValue = Chunks.Count * 2;

                        if (FRipCore.PackageData.ImageBank.ImageCount > 0)
                        {
                            if (imageReading == null)
                                imageReading = ctx.AddTask("[DeepSkyBlue3]Reading images[/]", true);

                            imageReading.Value = Data.Chunks.BankChunks.Images.ImageBank.LoadedImageCount;
                            imageReading.MaxValue = FRipCore.PackageData.ImageBank.ImageCount;
                        }
                    }
                }
            });
        }
    }
}
