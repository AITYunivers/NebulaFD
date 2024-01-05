using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAFormattedText : MFAObjectLoader
    {
        public BitDict FTFlags = new BitDict( // Formatted Text Flags
            "", "AutoScrollbar" // Auto Vertical Scrollbar
        );

        public int Width;
        public int Height;
        public Color Color;
        public string Data = string.Empty;

        public MFAFormattedText()
        {
            ChunkName = "MFAFormattedText";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Width = reader.ReadInt();
            Height = reader.ReadInt();
            FTFlags.Value = reader.ReadUInt();
            Color = reader.ReadColor();
            Data = reader.ReadAscii(reader.ReadInt());
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteUInt(FTFlags.Value);
            writer.WriteColor(Color);
            writer.WriteInt(Data.Length);
            writer.WriteAscii(Data);
        }
    }
}
