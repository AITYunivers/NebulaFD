using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Music
{
    public class Music : Chunk
    {
        public uint Handle;
        public int Checksum;
        public uint References;
        public uint Flags;
        public int Frequency;
        public string Name = string.Empty;
        public byte[] Data = new byte[0];

        public Music()
        {
            ChunkName = "Music";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ByteReader musicData;
            if (NebulaCore.Android || NebulaCore.iOS || NebulaCore.Flash || NebulaCore.HTML)
                return;

            Handle = reader.ReadUInt();
            if (NebulaCore.Fusion >= 2.5f)
                Handle--;
            int decompressedSize = reader.ReadInt();
            int size = reader.ReadInt();
            musicData = new ByteReader(Decompressor.DecompressBlock(reader, size));
            Checksum = musicData.ReadInt();
            References = musicData.ReadUInt();
            int dataSize = musicData.ReadInt();
            Flags = musicData.ReadUInt();
            Frequency = musicData.ReadInt();
            int nameLength = musicData.ReadInt();
            Name = musicData.ReadYuniversalStop(nameLength);
            Data = musicData.ReadBytes(dataSize - nameLength * 2);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();
            Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            int dataSize = reader.ReadInt();
            Flags = reader.ReadUInt();
            Frequency = reader.ReadInt();
            int nameLength = reader.ReadInt();
            Name = reader.ReadYuniversalStop(nameLength);
            Data = reader.ReadBytes(dataSize - nameLength * 2);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUInt(Handle);
            writer.WriteInt(Checksum);
            writer.WriteUInt(References);
            writer.WriteInt(Data.Length + Name.Length * 2);
            writer.WriteUInt(Flags);
            writer.WriteInt(Frequency);
            writer.WriteInt(Name.Length);
            writer.WriteYunicode(Name);
            writer.WriteBytes(Data);
        }
    }
}
