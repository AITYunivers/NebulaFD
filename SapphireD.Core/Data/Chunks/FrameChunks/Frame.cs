using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class Frame : Chunk
    {
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
