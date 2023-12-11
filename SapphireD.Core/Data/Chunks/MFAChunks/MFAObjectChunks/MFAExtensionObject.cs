using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAExtensionObject : MFAActive
    {
        public int Type;
        public string Name = string.Empty;
        public string FileName = string.Empty;
        public uint Magic;
        public string SubType = string.Empty;
        public int Version;
        public int ID;
        public int Private;
        public byte[] Data = new byte[0];

        public MFAExtensionObject()
        {
            ChunkName = "MFAExtensionObject";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Type = reader.ReadInt();
            if (Type == -1)
            {
                Name = reader.ReadAutoYuniversal();
                FileName = reader.ReadAutoYuniversal();
                Magic = reader.ReadUInt();
                SubType = reader.ReadAutoYuniversal();
            }

            int DataSize = reader.ReadInt();
            reader.Skip(8);
            Version = reader.ReadInt();
            ID = reader.ReadInt();
            Private = reader.ReadInt();
            Data = reader.ReadBytes(Math.Max(0, DataSize - 20));
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteInt(Type);
            if (Type == -1)
            {
                writer.WriteAutoYunicode(Name);
                writer.WriteAutoYunicode(FileName);
                writer.WriteUInt(Magic);
                writer.WriteAutoYunicode(SubType);
            }

            writer.WriteInt(Data.Length + 20);
            writer.WriteInt(0);
            writer.WriteInt(0);
            writer.WriteInt(Version);
            writer.WriteInt(ID);
            writer.WriteInt(Private);
            writer.WriteBytes(Data);
        }
    }
}
