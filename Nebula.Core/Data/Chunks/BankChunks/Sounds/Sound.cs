using Nebula.Core.Memory;
using System.Diagnostics;
using System.Text;

namespace Nebula.Core.Data.Chunks.BankChunks.Sounds
{
    public class Sound : Chunk
    {
        public uint Handle;
        public int Checksum;
        public uint References;
        public BitDict Flags = new BitDict( // Flags
            "Check",                        // Will not compile exes if off
            "", "", "", "", "Decompressed", // Decompressed
            "", "", "HasName",              // Has Name (Android Only?)
            "", "", "", "", "", "NameCrop"  // Name Crop
        );
        public int Frequency;
        public string Name = string.Empty;
        public byte[] Data = new byte[0];
        public bool Compressed = true;

        public Sound()
        {
            ChunkName = "Sound";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int decompressedSize;
            int nameLength;
            ByteReader soundData;
            if (NebulaCore.Android)
            {
                Handle = reader.ReadUShort();
                Flags.Value = reader.ReadUShort();
                reader.Skip(4);
                Frequency = reader.ReadInt();
                if (Flags["HasName"])
                    Name = reader.ReadYuniversal(reader.ReadShort());
                else
                    Name = "S" + Handle.ToString("D4");

                // Temp until reading from the apk is added
                Flags["Decompressed"] = false;
                return;
            }
            else if (NebulaCore.iOS)
            {
                Handle = reader.ReadUShort();
                Name = reader.ReadYuniversal(reader.ReadShort());
                reader.Skip(4);
                Data = reader.ReadBytes(reader.ReadInt());
                return;
            }
            else if (NebulaCore.Flash)
            {
                Handle = reader.ReadUShort();
                Name = reader.ReadYuniversal(reader.ReadShort());
                return;
            }
            else if (NebulaCore.HTML)
            {
                Handle = reader.ReadUShort();
                reader.Skip(1);
                Frequency = reader.ReadInt();
                Name = reader.ReadYuniversal(reader.ReadShort());
                return;
            }

            Handle = reader.ReadUInt();
            if (NebulaCore.Fusion >= 2.5f)
                Handle--;
            else if (NebulaCore.Fusion == 1.5f)
            {
                List<int> offsets = NebulaCore.PackageData.SoundOffsets.SortedOffsets;
                int startingOffset = (int)reader.Tell();
                int compressedSize = 3;
                if (offsets.IndexOf(startingOffset) != offsets.Count - 1)
                    compressedSize = offsets[offsets.IndexOf(startingOffset) + 1] - startingOffset - 4;
                decompressedSize = reader.ReadInt();
                var compressedBuffer = reader.ReadBytes(compressedSize - 4);
                reader = new ByteReader(Decompressor.DecompressOPFBlock(compressedBuffer));
            }
            if (NebulaCore.Fusion == 1.5f)
                Checksum = reader.ReadShort();
            else
                Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            decompressedSize = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Frequency = reader.ReadInt();
            nameLength = reader.ReadInt();

            if (NebulaCore.Fusion == 1.5f)
            {
                Name = reader.ReadYuniversalStop(nameLength);
                Data = FixSoundData(reader.ReadBytes());
            }
            else
            {
                if (Compressed && !Flags["Decompressed"])
                {
                    int size = reader.ReadInt();
                    soundData = new ByteReader(Decompressor.DecompressBlock(reader, size));
                }
                else
                    soundData = new ByteReader(reader.ReadBytes(decompressedSize));
                Name = soundData.ReadYuniversalStop(nameLength);
                if (Flags["Decompressed"]) soundData.Seek(0);
                Data = soundData.ReadBytes();
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt() - 1;
            Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            int decompressedSize = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Frequency = reader.ReadInt();
            int nameLength = reader.ReadInt();
            ByteReader soundData;
            soundData = new ByteReader(reader.ReadBytes(decompressedSize));
            Name = soundData.ReadYunicodeStop(nameLength);
            if (Flags["Decompressed"]) soundData.Seek(0);
            Data = soundData.ReadBytes();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            Flags["NameCrop"] = false;
            Flags["Check"] = true;
            writer.WriteUInt(Handle + 1);
            writer.WriteInt(Checksum);
            writer.WriteUInt(References);
            writer.WriteInt(Data.Length + (Flags["Decompressed"] ? 0 : Name.Length * 2 + 2));
            writer.WriteUInt(Flags.Value);
            writer.WriteInt(Frequency);
            writer.WriteInt(Name.Length + 1);

            if (Flags["Decompressed"])
                writer.WriteBytes(Data);
            else
            {
                writer.WriteYunicode(Name, true);
                writer.WriteBytes(Data);
            }
        }

        public static byte[] FixSoundData(byte[] data)
        {
            byte[] output = new byte[data.Length + 44];
            Array.Copy(Encoding.ASCII.GetBytes("RIFF"), 0, output, 0, 4);
            Array.Copy(BitConverter.GetBytes(8146), 0, output, 4, 4);
            Array.Copy(Encoding.ASCII.GetBytes("WAVEfmt "), 0, output, 8, 8);
            Array.Copy(BitConverter.GetBytes(16), 0, output, 16, 4);
            Array.Copy(data, 0, output, 20, 16);
            Array.Copy(Encoding.ASCII.GetBytes("data"), 0, output, 36, 4);
            Array.Copy(BitConverter.GetBytes(data.Length - 16), 0, output, 40, 4);
            Array.Copy(data, 16, output, 44, data.Length - 16);
            return output;
        }

        public string GetSoundType()
        {
            string header = Encoding.ASCII.GetString(Data[0..3]);
            switch (header)
            {
                case "RIFF":
                    return "WAV";
                case "AIFF":
                    return "AIFF";
                case "OggS":
                    return "OGG";
                default:
                    return "MOD";
            }
        }
    }
}
