using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameMovementTimerChunk : CommonChunk
    {
        public int Value;

        public override void ReadChunkData(ByteReader reader)
        {
            Value = reader.ReadInt();

            this.Log("Movement Timer: " + Value, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(Value);
        }
    }
}
