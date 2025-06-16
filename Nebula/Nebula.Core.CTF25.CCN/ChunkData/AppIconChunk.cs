using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.ChunkData
{
    internal class AppIconChunk : CommonChunk
    {
        public Color[] Palette = [];
        public byte[] ImageData = [];

        public override void ReadChunkData(ByteReader reader)
        {
            reader.Skip(reader.ReadInt() - 4); // Unknown
            Palette = reader.ReadColors(16 * 16);
            ImageData = reader.ReadBytes(16 * 16 * 2);

            this.Log("Yep, the app icon data does indeed exist.", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(4); // Unknown
            writer.WriteColors(Palette);
            writer.WriteBytes(ImageData);
        }
    }
}
