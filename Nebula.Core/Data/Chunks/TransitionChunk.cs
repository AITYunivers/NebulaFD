using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class TransitionChunk : Chunk
    {
        public string ModuleName = string.Empty;
        public int Module;
        public string ID = string.Empty;
        public int Duration;
        public bool UseColor;
        public Color Color = Color.Black;

        public string FileName = string.Empty;
        public byte[] ParameterData = new byte[0];

        public TransitionChunk()
        {
            ChunkName = "TransitionChunk";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();

            Module = reader.ReadInt();
            ID = reader.ReadAscii(4);
            Duration = reader.ReadInt();
            UseColor = reader.ReadInt() != 0;
            Color = reader.ReadColor();

            int NameOffset = reader.ReadInt();
            int DataOffset = reader.ReadInt();
            int DataSize = reader.ReadInt();

            reader.Seek(StartOffset + NameOffset);
            FileName = reader.ReadYuniversal();
            reader.Seek(StartOffset + DataOffset);
            ParameterData = reader.ReadBytes(DataSize);

            ModuleName = ID switch
            {
                "SE00" => "Advanced Scrolling",
                "SE10" => "Back",
                "BAND" => "Bands",
                "SE12" => "Cell",
                "DOOR" => "Door",
                "FADE" => "Fade",
                "SE03" => "Line",
                "MOSA" => "Mosaic",
                "SE05" => "Open",
                "SE06" => "Push",
                "SCRL" => "Scrolling",
                "SE01" => "Square",
                "SE07" => "Stretch",
                "SE09" => "Stretch 2",
                "SE08" => "Turn",
                "SE02" => "Turn 2",
                "SE13" => "Weft",
                "ZIGZ" => "Zigzag",
                "SE04" => "ZigZag 2",
                "ZOOM" => "Zoom",
                "SE11" => "Zoom 2",
                _ => "Transition"
            };
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            FileName = reader.ReadAutoYuniversal();
            ModuleName = reader.ReadAutoYuniversal();
            Module = reader.ReadInt();
            ID = reader.ReadAscii(4);
            Duration = reader.ReadInt();
            UseColor = reader.ReadInt() != 0;
            Color = reader.ReadColor();
            ParameterData = reader.ReadBytes(reader.ReadInt());
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(FileName);
            writer.WriteAutoYunicode(ModuleName);
            writer.WriteInt(Module);
            writer.WriteAscii(ID);
            writer.WriteInt(Duration);
            writer.WriteInt(UseColor ? 1 : 0);
            writer.WriteColor(Color);
            writer.WriteInt(ParameterData.Length);
            writer.WriteBytes(ParameterData);
        }
    }
}
