using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.Chunks
{
    internal class AppIconChunk : CommonChunk
    {
        public Color[] Palette = [];
        public byte[] ImageData = [];

        public override void ReadChunkData(ByteReader reader)
        {
            reader.Skip(reader.ReadInt() - 4); // Unknown

            Palette = new Color[16 * 16];
            for (int i = 0; i < Palette.Length; i++)
                Palette[i] = reader.ReadColor();

            ImageData = reader.ReadBytes(16 * 16 * 2);

            this.Log("Yep, the app icon data does indeed exist.", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(4); // Unknown

            foreach (Color color in Palette)
                writer.WriteColor(color);

            writer.WriteBytes(ImageData);
        }
    }
}
