using Nebula.Core.Memory;
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
            "", "", "", "LoadOnCall",       // Load on Call
            "PlayFromDisk",                 // Play from Disk
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
                Flags["PlayFromDisk"] = false;
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
                Data = FixSoundData(reader.ReadBytes(decompressedSize - nameLength * (NebulaCore.Yunicode ? 2 : 1)));
            }
            else
            {
                if (Compressed && !Flags["PlayFromDisk"])
                {
                    int size = reader.ReadInt();
                    soundData = new ByteReader(Decompressor.DecompressBlock(reader, size));
                }
                else
                    soundData = new ByteReader(reader.ReadBytes(decompressedSize));
                Name = soundData.ReadYuniversalStop(nameLength);
                if (Flags["PlayFromDisk"]) soundData.Seek(0);
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
            if (Flags["PlayFromDisk"]) soundData.Seek(0);
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
            writer.WriteInt(Data.Length + (Flags["PlayFromDisk"] ? 0 : Name.Length * 2 + 2));
            writer.WriteUInt(Flags.Value);
            writer.WriteInt(Frequency);
            writer.WriteInt(Name.Length + 1);

            if (Flags["PlayFromDisk"])
                writer.WriteBytes(Data);
            else
            {
                writer.WriteYunicode(Name, true);
                writer.WriteBytes(Data);
            }
        }

        public static byte[] FixSoundData(byte[] data)
        {
            using ByteReader old_file = new ByteReader(data);
            short formatTag = old_file.ReadShort();
            short channels = old_file.ReadShort();
            
            byte[] header;
            byte[] audio;


            using ByteWriter writer = new ByteWriter(new MemoryStream());
            switch (formatTag) {
                //ADPCM
                case 2:
                    header = old_file.ReadBytes(46);
                    audio = old_file.ReadBytes();

                    writer.WriteAscii("RIFF");
                    writer.WriteInt32((data.Length + 44 - 8));
                    writer.WriteAscii("WAVEfmt ");
                    writer.WriteInt32(50);
                    writer.WriteShort(formatTag);
                    writer.WriteShort(channels);
                    writer.WriteBytes(header);
                    writer.WriteAscii("fact");
                    writer.WriteInt32(4);
                    writer.WriteInt32((audio.Length - 6 * channels) * 2 / channels);
                    writer.WriteAscii("data");
                    writer.WriteBytes(audio);
                    break;
                //PCM (fallback)
                default:
                    header = old_file.ReadBytes(12);
                    audio = old_file.ReadBytes();
                    writer.WriteAscii("RIFF");
                    writer.WriteInt32((data.Length + 44 - 8));
                    writer.WriteAscii("WAVEfmt ");
                    writer.WriteInt32(16);
                    writer.WriteShort(formatTag);
                    writer.WriteShort(channels);
                    writer.WriteBytes(header);
                    writer.WriteAscii("data");
                    writer.WriteBytes(audio);
                    break;

            }
            return writer.ToArray();
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
