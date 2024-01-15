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
        public List<Task> ChunkReaders = new();
        public List<Chunk> Chunks = new();

        public int ChunksLoaded;

        public override void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {NebulaCore.BuildDate} build.");
            if (NebulaCore.Fusion == 1.1f)
                return;

            Header = reader.ReadAscii(4);
            NebulaCore._unicode = Header != "PAME";
            if (Header == "CRUF")
                NebulaCore.Fusion = 3f;
            Logger.Log(this, "Game Header: " + Header);

            RuntimeVersion = reader.ReadShort();
            RuntimeSubversion = reader.ReadShort();
            ProductVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            NebulaCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            if (NebulaCore.Build < 280)
                NebulaCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);

            Frames = new List<Frame>();
            int frameHandle = 0;
            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                Logger.Log(this, $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                if (newChunk.ChunkID == 32494)
                    NebulaCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    NebulaCore.Plus = true;

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
                else if (newChunk is Frame && NebulaCore.CurrentReader is OpenFileReader)
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
                ProgressTask? chunkReading = ctx.AddTask($"[{NebulaCore.ColorRules[4]}]Reading chunks[/]", false);
                ProgressTask? imageReading = null;

                while (!chunkReading.IsFinished)
                {
                    if (NebulaCore.Fusion == 1.1f)
                        return;

                    if (NebulaCore.PackageData != null && Chunks.Count > 0)
                    {
                        if (!chunkReading.IsStarted)
                            chunkReading.StartTask();

                        chunkReading.Value = ChunksLoaded;
                        chunkReading.MaxValue = Chunks.Count * 2;

                        if (NebulaCore.PackageData.ImageBank.ImageCount > 0)
                        {
                            if (imageReading == null)
                                imageReading = ctx.AddTask($"[{NebulaCore.ColorRules[4]}]Reading images[/]", true);

                            imageReading.Value = Data.Chunks.BankChunks.Images.ImageBank.LoadedImageCount;
                            imageReading.MaxValue = NebulaCore.PackageData.ImageBank.ImageCount;
                        }
                    }
                }
            });
        }
    }
}
