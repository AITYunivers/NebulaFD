using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunks.Objects
{
    internal class ObjectNameChunk : CommonChunk
    {
        public string Name = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Name = reader.ReadYuniversal();
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Name);
        }
    }
}
