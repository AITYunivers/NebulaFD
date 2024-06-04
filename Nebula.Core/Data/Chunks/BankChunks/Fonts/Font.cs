using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Fonts
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
            if (NebulaCore.Fusion >= 2.0f && NebulaCore.Build < 284)
                Handle++;

            ByteReader dataReader;
            if (NebulaCore.Fusion == 1.5f)
            {
                List<int> offsets = NebulaCore.PackageData.FontOffsets.SortedOffsets;
                int startingOffset = (int)reader.Tell();
                int compressedSize = 3;
                if (offsets.IndexOf(startingOffset) != offsets.Count - 1)
                    compressedSize = offsets[offsets.IndexOf(startingOffset) + 1] - startingOffset - 4;
                int decompressedSize = reader.ReadInt();
                var compressedBuffer = reader.ReadBytes(compressedSize - 4);
                dataReader = new ByteReader(Decompressor.DecompressOPFBlock(compressedBuffer));
            }
            else if (Compressed) dataReader = Decompressor.DecompressAsReader(reader, out var decompSize);
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
            writer.WriteUInt(Handle);
            writer.WriteInt(Checksum);
            writer.WriteInt(References);
            writer.WriteInt(0);

            writer.WriteInt(Height);
            writer.WriteInt(Width);
            writer.WriteInt(Escapement);
            writer.WriteInt(Orientation);
            writer.WriteInt(Weight);
            writer.WriteByte(Italic);
            writer.WriteByte(Underline);
            writer.WriteByte(StrikeOut);
            writer.WriteByte(CharSet);
            writer.WriteByte(OutPrecision);
            writer.WriteByte(ClipPrecision);
            writer.WriteByte(Quality);
            writer.WriteByte(PitchAndFamily);
            writer.WriteYunicode(Name, 32);
        }
    }
}
