using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameHeaderChunk : CommonChunk
    {
        public int Width;
        public int Height;
        public Color Background;
        public uint Flags;

        public override void ReadChunkData(ByteReader reader)
        {
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Background = reader.ReadColor();
            Flags = reader.ReadUInt();

            this.Log($"Frame Size: {Width}x{Height}", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteColor(Background);
            writer.WriteUInt(Flags);
        }
    }
}
