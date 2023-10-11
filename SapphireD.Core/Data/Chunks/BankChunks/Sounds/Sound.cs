using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.BankChunks.Sounds
{
    public class Sound : Chunk
    {
        public uint Handle;
        public int Checksum;
        public uint References;
        public int Flags;
        public int Frequency;
        public string Name;
        public byte[] Data;
        public bool Compressed = true;

        public Sound()
        {
            ChunkName = "Sound";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt() - 1;
            Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            int decompressedSize = reader.ReadInt();
            Flags = reader.ReadByte();
            reader.Skip(3);
            Frequency = reader.ReadInt();
            int nameLength = reader.ReadInt();
            ByteReader soundData;
            if (Compressed && Flags != 33)
            {
                int size = reader.ReadInt();
                soundData = new ByteReader(Decompressor.DecompressBlock(reader, size));
            }
            else
                soundData = new ByteReader(reader.ReadBytes(decompressedSize));
            Name = Utilities.Utilities.ClearName(soundData.ReadYuniversal(nameLength).Trim());
            if (Flags == 33) soundData.Seek(0);
            Data = soundData.ReadBytes();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt() - 1;
            Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            int decompressedSize = reader.ReadInt();
            Flags = reader.ReadByte();
            reader.Skip(3);
            Frequency = reader.ReadInt();
            int nameLength = reader.ReadInt();
            ByteReader soundData;
            soundData = new ByteReader(reader.ReadBytes(decompressedSize));
            Name = Utilities.Utilities.ClearName(soundData.ReadWideString(nameLength).Trim());
            if (Flags == 33) soundData.Seek(0);
            Data = soundData.ReadBytes();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
