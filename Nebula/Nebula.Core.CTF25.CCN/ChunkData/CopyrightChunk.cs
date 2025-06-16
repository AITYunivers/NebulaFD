using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class CopyrightChunk : CommonChunk
    {
        public string Copyright = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Copyright = reader.ReadYuniversal();

            this.Log("Copyright: " + Copyright, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Copyright);
        }
    }
}
