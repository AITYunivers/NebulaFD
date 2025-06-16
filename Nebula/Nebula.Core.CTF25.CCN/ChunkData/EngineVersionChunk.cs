using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class EngineVersionChunk : CommonChunk
    {
        public uint EngineVersion;
        public uint EngineSubversion;
        public uint Flags;

        public override void ReadChunkData(ByteReader reader)
        {
            EngineVersion = reader.ReadUInt();
            EngineSubversion = reader.ReadUInt();
            Flags = reader.ReadUInt();

            this.Log($"Engine Version: {EngineVersion}.{EngineSubversion}", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUInt(EngineVersion);
            writer.WriteUInt(EngineSubversion);
            writer.WriteUInt(Flags);
        }
    }
}
