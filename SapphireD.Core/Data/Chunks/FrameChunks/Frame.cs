using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class Frame : Chunk
    {
        public FrameHeader FrameHeader = new();               // 0x3334
        public string FrameName = string.Empty;               // 0x3335
        public string FramePassword = string.Empty;           // 0x3336
        public FramePalette FramePalette = new();             // 0x3337
        public FrameInstances FrameInstances = new();         // 0x3338
        public FrameTransitionIn FrameTransitionIn = new();   // 0x333B
        public FrameTransitionOut FrameTransitionOut = new(); // 0x333C
        public FrameEvents FrameEvents = new();               // 0x333D
        public FrameLayers FrameLayers = new();               // 0x3341
        public FrameRect FrameRect = new();                   // 0x3342
        public int FrameSeed;                                 // 0x3344
        public int FrameMoveTimer;                            // 0x3347
        public FrameEffects FrameEffects = new();             // 0x3349

        public MFAFrameInfo MFAFrameInfo = new();

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
            MFAFrameInfo.Handle = reader.ReadInt();
            FrameName = reader.ReadAutoYuniversal();
            FrameHeader.ReadMFA(reader);

            // Max Objects
            reader.Skip(4);

            FramePassword = reader.ReadAutoYuniversal();
            reader.Skip(reader.ReadInt()); // ?

            MFAFrameInfo.EditorX = reader.ReadInt();
            MFAFrameInfo.EditorY = reader.ReadInt();

            FramePalette.ReadMFA(reader);

            MFAFrameInfo.Stamp = reader.ReadInt();
            MFAFrameInfo.EditorLayer = reader.ReadInt();

            FrameLayers.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                FrameTransitionIn.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                FrameTransitionOut.ReadMFA(reader);

            MFAFrameInfo.Objects = new MFAObjectInfo[reader.ReadInt()];
            for (int i = 0; i < MFAFrameInfo.Objects.Length; i++)
            {
                MFAFrameInfo.Objects[i] = new MFAObjectInfo();
                MFAFrameInfo.Objects[i].ReadMFA(reader);
            }

            MFAFrameInfo.Folders.ReadMFA(reader);
            FrameInstances.ReadMFA(reader);
            FrameEvents.ReadMFA(reader);

            while (true)
            {
                Chunk newChunk = InitMFAChunk(reader, false);
                Logger.Log(this, $"Reading MFA Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadMFA(chunkReader, this);
                newChunk.ChunkData = new byte[0];
                if (newChunk is Last)
                    break;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
