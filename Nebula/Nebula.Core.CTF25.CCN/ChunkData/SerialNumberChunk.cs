using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class SerialNumberChunk : CommonChunk
    {
        public uint Value;

        public override void ReadChunkData(ByteReader reader)
        {
            Value = reader.ReadUInt();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUInt(Value);
        }
    }
}
