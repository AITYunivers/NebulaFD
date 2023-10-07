using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class Frame : Chunk
    {
        public static Frame curFrame;

        public FrameHeader FrameHeader;
        public string FrameName = string.Empty;
        public FrameLayers FrameLayers;
        public FrameRect FrameRect;
        public int FrameMoveTimer;
        public FrameEffects FrameEffects;

        public Frame()
        {
            ChunkName = "Frame";
            ChunkID = 0x3333;
        }

        public override void ReadCCN(ByteReader reader)
        {
            curFrame = this;
            string log = string.Empty;

            while (reader.HasMemory(8))
            {
                var newChunk = InitChunk(reader);
                log = $"Reading Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})";
                Logger.Log(this, log);

                ByteReader chunkReader = new ByteReader(ChunkData);
                newChunk.ReadCCN(chunkReader);
            }

            SapDCore.PackageData.Frames.Add(this);
            log = $"Frame '{FrameName}' found.";
            Logger.Log(this, log, color: ConsoleColor.Green);
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
