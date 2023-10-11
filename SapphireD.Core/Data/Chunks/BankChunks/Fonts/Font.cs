using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Fonts
{
    public class Font : Chunk
    {
        public uint Handle;
        public int Checksum;
        public int References;
        public bool Compressed = true;

        public int Height;
        public int Width;
        public int Escapement;
        public int Orientation;
        public int Weight;
        public byte Italic;
        public byte Underline;
        public byte StrikeOut;
        public byte CharSet;
        public byte OutPrecision;
        public byte ClipPrecision;
        public byte Quality;
        public byte PitchAndFamily;
        public string Name = string.Empty;

        public Font()
        {
            ChunkName = "Font";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();

            ByteReader dataReader = null;
            if (Compressed)
            {
                dataReader = Decompressor.DecompressAsReader(reader, out var decompSize);
            }
            else dataReader = reader;
            Checksum = dataReader.ReadInt();
            References = dataReader.ReadInt();
            var size = dataReader.ReadInt();

            Height = dataReader.ReadInt();
            Width = dataReader.ReadInt();
            Escapement = dataReader.ReadInt();
            Orientation = dataReader.ReadInt();
            Weight = dataReader.ReadInt();
            Italic = dataReader.ReadByte();
            Underline = dataReader.ReadByte();
            StrikeOut = dataReader.ReadByte();
            CharSet = dataReader.ReadByte();
            OutPrecision = dataReader.ReadByte();
            ClipPrecision = dataReader.ReadByte();
            Quality = dataReader.ReadByte();
            PitchAndFamily = dataReader.ReadByte();
            Name = dataReader.ReadYuniversal(32);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();

            Checksum = reader.ReadInt();
            References = reader.ReadInt();
            var size = reader.ReadInt();

            Height = reader.ReadInt();
            Width = reader.ReadInt();
            Escapement = reader.ReadInt();
            Orientation = reader.ReadInt();
            Weight = reader.ReadInt();
            Italic = reader.ReadByte();
            Underline = reader.ReadByte();
            StrikeOut = reader.ReadByte();
            CharSet = reader.ReadByte();
            OutPrecision = reader.ReadByte();
            ClipPrecision = reader.ReadByte();
            Quality = reader.ReadByte();
            PitchAndFamily = reader.ReadByte();
            Name = reader.ReadYuniversal(32);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
