using FusionRipper.Core.Data.Chunks;
using FusionRipper.Core.Data.Chunks.AppChunks;
using FusionRipper.Core.Data.Chunks.FrameChunks;
using FusionRipper.Core.Data.Chunks.ObjectChunks;
using FusionRipper.Core.FileReaders;
using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;
using Spectre.Console;

namespace FusionRipper.Core.Data.PackageReaders
{
    public class CCNPackageData : PackageData
    {
        public List<Task> ChunkReaders = new();
        public List<Chunk> Chunks = new();

        public int ChunksLoaded;

        public override void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {FRipCore.BuildDate} build.");
            if (FRipCore.Fusion == 1.1f)
                return;

            Header = reader.ReadAscii(4);
            if (Header.StartsWith("PAM"))
                FRipCore._unicode = Header != "PAME";
            if (Header == "CRUF")
                FRipCore.Fusion = 3f;
            Logger.Log(this, "Game Header: " + Header);

            RuntimeVersion = reader.ReadShort();
            RuntimeSubversion = reader.ReadShort();
            ProductVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            FRipCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            if (FRipCore.Build < 280)
                FRipCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);

            Frames = new List<Frame>();
            int frameHandle = 0;
            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                Logger.Log(this, $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    FRipCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    FRipCore.Plus = true;

                Chunks.Add(newChunk);
                int chunkId = Chunks.Count - 1;

                Task chunkReader = new Task(() =>
                {
                    //try
                    {
                        ByteReader chunkReader = new ByteReader(Chunks[chunkId].ChunkData!);
                        Chunks[chunkId].ReadCCN(chunkReader);
                    }
                    //catch {}
                    Chunks[chunkId].ChunkData = new byte[0];
                    ChunksLoaded++;
                });

                // Chunks with read priority
                if (newChunk is AppName        || // For Encryption
                    newChunk is EditorFilename || // For Encryption
                    newChunk is Copyright      || // For Encryption
                    newChunk is FrameHandles   || // For Frame Handles
                    newChunk is ExtendedHeader || // For Image Bank
                    newChunk is ObjectHeaders)    // For 2.5+ Object Bank
                    chunkReader.RunSynchronously();
                else if (newChunk is Frame && FRipCore.CurrentReader is OpenFileReader)
                    chunkReader.RunSynchronously();
                else
                    ChunkReaders.Add(chunkReader);

                ChunksLoaded++;
            }

            foreach (Task chunkReader in ChunkReaders)
                chunkReader.RunSynchronously(); //chunkReader.Start();

            foreach (Task chunkReader in ChunkReaders)
                chunkReader.Wait();
        }

        public override void CliUpdate()
        {
            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? chunkReading = ctx.AddTask($"[{FRipCore.ColorRules[4]}]Reading chunks[/]", false);
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
                                imageReading = ctx.AddTask($"[{FRipCore.ColorRules[4]}]Reading images[/]", true);

                            imageReading.Value = Data.Chunks.BankChunks.Images.ImageBank.LoadedImageCount;
                            imageReading.MaxValue = FRipCore.PackageData.ImageBank.ImageCount;
                        }
                    }
                }
            });
        }
    }
}
