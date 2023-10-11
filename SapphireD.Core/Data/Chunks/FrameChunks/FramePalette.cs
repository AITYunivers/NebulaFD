using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FramePalette : Chunk
    {
        public short PaletteVersion;
        public short PaletteEntries;
        public List<Color> Palette = new();

        public FramePalette()
        {
            ChunkName = "FramePalette";
            ChunkID = 0x3337;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            PaletteVersion = reader.ReadShort();
            PaletteEntries = reader.ReadShort();
            for (int i = 0; i < PaletteEntries; i++)
                Palette.Add(reader.ReadColor());

            ((Frame)extraInfo[0]).FramePalette = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
