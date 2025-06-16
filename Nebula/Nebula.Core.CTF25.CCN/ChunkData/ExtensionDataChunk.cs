using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class ExtensionDataChunk : CommonChunk
    {
        public byte[] Data = [];

        public override void ReadChunkData(ByteReader reader)
        {
            Data = reader.ReadBytes(reader.ReadInt());

            this.Log("Extension Data Size: " + Data.Length, Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteBytes(Data, true);
        }
    }
}
