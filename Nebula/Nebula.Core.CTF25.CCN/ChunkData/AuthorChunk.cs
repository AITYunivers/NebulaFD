using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks
{
    internal class AuthorChunk : CommonChunk
    {
        public string Author = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Author = reader.ReadYuniversal();

            this.Log("Author: " + Author, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Author);
        }
    }
}
