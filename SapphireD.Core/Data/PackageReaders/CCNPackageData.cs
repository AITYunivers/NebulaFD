using SapphireD.Core.Data.Chunks;
using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
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

            RuntimeVersion = reader.ReadShort();
            RuntimeSubversion = reader.ReadShort();
            ProductVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            SapDCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            if (SapDCore.Build < 280)
                SapDCore.Fusion = 2f + (ProductVersion == 1 ? 0.1f : 0);

            Frames = new Dictionary<int, Frame>();
            int frameHandle = 0;
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
                    //try
                    {
                        ByteReader chunkReader = new ByteReader(Chunks[chunkId].ChunkData!);
                        Chunks[chunkId].ReadCCN(chunkReader);
                    }
                    //catch {}
                    Chunks[chunkId].ChunkData = new byte[0];
                    ChunksLoaded++;
                });

                if (newChunk is Frame)
                    (newChunk as Frame).Handle = frameHandle++;

                // Chunks with read priority
                if (newChunk is AppName        || // For Encryption
                    newChunk is EditorFilename || // For Encryption
                    newChunk is Copyright      || // For Encryption
                    newChunk is ExtendedHeader || // For Image Bank
                    newChunk is ObjectHeaders)    // For 2.5+ Object Bank
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
