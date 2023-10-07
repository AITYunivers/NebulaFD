using SapphireD.Core.Data.Chunks;
using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Fonts;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.BankChunks.Sounds;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data
{
    public class PackageData
    {
        public string Header = string.Empty;
        public short RuntimeVersion;
        public short RuntimeSubversion;
        public int ProductVersion;
        public int ProductBuild;

        public string AppName = string.Empty;
        public string EditorFilename = string.Empty;
        public string TargetFilename = string.Empty;
        public string Copyright = string.Empty;
        public AppHeader AppHeader;
        public AppHeader2 AppHeader2;
        public int AppCodePage;
        public Extensions Extensions;
        public FrameItems FrameItems;
        public short[] FrameHandles;
        public List<Frame> Frames;

        public ImageOffsets ImageOffsets;
        public ImageBank ImageBank;
        public FontOffsets FontOffsets;
        public FontBank FontBank;
        public SoundOffsets SoundOffsets;
        public SoundBank SoundBank;
        public MusicOffsets MusicOffsets;

        public void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {SapDCore.BuildDate} build.");
            Header = reader.ReadAscii(4);
            SapDCore.Unicode = Header != "PAME";
            Logger.Log(this, "Game Header: " + Header);

            RuntimeVersion = (short)reader.ReadUInt16();
            RuntimeSubversion = (short)reader.ReadUInt16();
            ProductVersion = reader.ReadInt32();
            ProductBuild = reader.ReadInt32();
            SapDCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            Frames = new List<Frame>();
            while (reader.HasMemory(8))
            {
                var newChunk = Chunk.InitChunk(reader);
                string log = $"Reading Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})";
                Logger.Log(this, log);

                if (newChunk.ChunkID == 32494)
                    SapDCore.Seeded = true;
                if (newChunk.ChunkID == 8787)
                    SapDCore.Plus = true;

                ByteReader chunkReader = new ByteReader(Chunk.ChunkData);
                newChunk.ReadCCN(chunkReader);
            }
        }
    }
}
