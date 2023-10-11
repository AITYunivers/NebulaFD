using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class Frame : Chunk
    {
        public FrameHeader FrameHeader = new();       // 0x3334
        public string FrameName = string.Empty;       // 0x3335
        public string FramePassword = string.Empty;   // 0x3336
        public FramePalette FramePalette = new();     // 0x3337
        public FrameInstances FrameInstances = new(); // 0x3338
        public FrameLayers FrameLayers = new();       // 0x3341
        public FrameRect FrameRect = new();           // 0x3342
        public short FrameSeed;                       // 0x3344
        public int FrameMoveTimer;                    // 0x3347
        public FrameEffects FrameEffects = new();     // 0x3349

        public Frame()
        {
            ChunkName = "Frame";
            ChunkID = 0x3333;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            string log = string.Empty;

            while (reader.HasMemory(8))
            {
                var newChunk = InitChunk(reader);
                log = $"Reading Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})";
                Logger.Log(this, log);

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader, this);
                newChunk.ChunkData = null;
            }

            SapDCore.PackageData.Frames.Add(this);
            log = $"Frame '{FrameName}' found.";
            Logger.Log(this, log, color: ConsoleColor.Green);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
