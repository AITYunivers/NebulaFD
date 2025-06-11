using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.Chunks
{
    internal class AppHeaderChunk : CommonChunk
    {
        public ulong Flags;
        public int WindowWidth;
        public int WindowHeight;
        public int DefaultScore;
        public int DefaultLives;
        public ushort[] ControlTypes = [4, 4, 4, 4];
        public ushort[][] ControlKeys =
        [
            [38, 40, 37, 39, 16, 17, 32, 13],
            [38, 40, 37, 39, 16, 17, 32, 13],
            [38, 40, 37, 39, 16, 17, 32, 13],
            [38, 40, 37, 39, 16, 17, 32, 13]
        ];
        public Color BorderColor;
        public int FrameCount;
        public int FrameRate;
        public uint WindowMenuHandle;

        public override void ReadChunkData(ByteReader reader)
        {
            int dataSize = reader.ReadInt();
            Flags = reader.ReadULong();
            WindowWidth = reader.ReadUShort();
            WindowHeight = reader.ReadUShort();
            DefaultScore = (reader.ReadInt() + 1) * -1;
            DefaultLives = (reader.ReadInt() + 1) * -1;

            for (int i = 0; i < 4; i++)
                ControlTypes[i] = reader.ReadUShort();

            for (int i = 0; i < 4; i++)
                for (int ii = 0; ii < 8; ii++)
                    ControlKeys[i][ii] = reader.ReadUShort();

            BorderColor = reader.ReadColor();
            FrameCount = reader.ReadInt();
            FrameRate = reader.ReadInt();
            WindowMenuHandle = reader.ReadUInt();

            this.Log($"({WindowWidth}x{WindowHeight}) FPS: {FrameRate} | Frames: {FrameCount}", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            using ByteWriter headerWriter = new ByteWriter(new MemoryStream());
            headerWriter.WriteULong(Flags);
            headerWriter.WriteUShort((ushort)WindowWidth);
            headerWriter.WriteUShort((ushort)WindowHeight);
            headerWriter.WriteInt((DefaultScore + 1) * -1);
            headerWriter.WriteInt((DefaultLives + 1) * -1);

            for (int i = 0; i < 4; i++)
                headerWriter.WriteUShort(ControlTypes[i]);
            for (int i = 0; i < 4; i++)
                for (int ii = 0; ii < 8; ii++)
                    headerWriter.WriteUShort(ControlKeys[i][ii]);

            headerWriter.WriteColor(BorderColor);
            headerWriter.WriteInt(FrameCount);
            headerWriter.WriteInt(FrameRate);
            headerWriter.WriteUInt(WindowMenuHandle);

            writer.WriteUInt((uint)headerWriter.Tell() + 4);
            writer.WriteWriter(headerWriter);
        }
    }
}
