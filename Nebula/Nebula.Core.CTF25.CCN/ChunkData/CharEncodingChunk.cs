using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class CharEncodingChunk : CommonChunk
    {
        public uint InputEncoding; // Make an enum pls
        public uint OutputEncoding; // Make an enum pls

        public override void ReadChunkData(ByteReader reader)
        {
            InputEncoding = reader.ReadUInt();
            OutputEncoding = reader.ReadUInt();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUInt(InputEncoding);
            writer.WriteUInt(OutputEncoding);
        }
    }
}
