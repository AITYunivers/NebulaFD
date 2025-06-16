using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameRectChunk : CommonChunk
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public override void ReadChunkData(ByteReader reader)
        {
            Left = reader.ReadInt();
            Top = reader.ReadInt();
            Right = reader.ReadInt();
            Bottom = reader.ReadInt();

            this.Log($"Frame Rect: {Left}, {Top}, {Right}, {Bottom}", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(Left);
            writer.WriteInt(Top);
            writer.WriteInt(Right);
            writer.WriteInt(Bottom);
        }
    }
}
