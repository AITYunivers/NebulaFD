using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectFormattedText : Chunk
    {
        public BitDict FTFlags = new BitDict( // Formatted Text Flags
            "", "AutoScrollbar" // Auto Vertical Scrollbar
        );

        public int Width;
        public int Height;
        public Color Color;
        public string Data = string.Empty;

        public ObjectFormattedText()
        {
            ChunkName = "ObjectFormattedText";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4); // Size
            reader.Skip(4); // Unknown
            FTFlags.Value = reader.ReadUInt();
            Color = reader.ReadColor();
            if (NebulaCore.Fusion > 1.5f)
            {
                Width = reader.ReadInt();
                Height = reader.ReadInt();
            }
            else
            {
                Width = reader.ReadUShort();
                Height = reader.ReadUShort();
            }
            reader.Skip(4); // Unknown
            Data = reader.ReadAscii(reader.ReadInt());
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
