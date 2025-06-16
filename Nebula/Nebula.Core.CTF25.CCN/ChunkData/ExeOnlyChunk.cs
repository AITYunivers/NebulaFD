using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class ExeOnlyChunk : CommonChunk
    {
        public bool Value;

        public override void ReadChunkData(ByteReader reader)
        {
            Value = reader.ReadBool();

            this.Log("Exe Only: " + Value, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteBool(Value);
        }
    }
}
