using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks
{
    internal class EditorFilenameChunk : CommonChunk
    {
        public string Filename = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Filename = reader.ReadYuniversal();

            this.Log("Editor Filename: " + Filename, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Filename);
        }
    }
}
