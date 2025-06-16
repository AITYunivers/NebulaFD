using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class ExeOnlyChunk : CommonChunk
    {
        public bool Value;

        public override void ReadChunkData(ByteReader reader)
        {
            Value = reader.ReadBool();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteBool(Value);
        }
    }
}
