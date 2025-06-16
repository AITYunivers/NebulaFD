using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FrameNameChunk : CommonChunk
    {
        public string Value = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Value = reader.ReadYuniversal();

            this.Log("Frame Name: " + Value, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Value);
        }
    }
}
