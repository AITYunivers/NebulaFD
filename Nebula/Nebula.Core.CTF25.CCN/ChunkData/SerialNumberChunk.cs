using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class SerialNumberChunk : CommonChunk
    {
        public ushort Value;

        public override void ReadChunkData(ByteReader reader)
        {
            Value = reader.ReadUShort();

            this.Log("Serial Number: " + Value, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUInt(Value);
        }
    }
}
