using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class TransitionFilenameChunk : CommonChunk
    {
        public ushort Handle;
        public string Filename = string.Empty;

        public override void ReadChunkData(ByteReader reader)
        {
            Handle = reader.ReadUShort();
            Filename = reader.ReadYuniversal();

            this.Log($"Transition Filename: {Filename} (Handle: {Handle})", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteYunicode(Filename);
        }
    }
}
