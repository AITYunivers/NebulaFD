using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks
{
    internal class AppNameChunk : CommonChunk
    {
        public string Name = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Name = reader.ReadYuniversal();

            this.Log("App Name: " + Name, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Name);
        }
    }
}
