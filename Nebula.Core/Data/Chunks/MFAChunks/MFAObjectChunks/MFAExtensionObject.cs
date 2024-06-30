using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks
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

            uint RealSize = reader.ReadUInt();
            uint EndPositon = (uint)reader.Tell() + RealSize;
            int DataSize = reader.ReadInt();
            reader.Skip(4);
            Version = reader.ReadInt();
            ID = reader.ReadInt();
            Private = reader.ReadInt();
            Data = reader.ReadBytes(Math.Max(0, DataSize - 20));
            reader.Seek(EndPositon);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            // Enable this if ya want, only made it
            // for myself though so won't enable it

            //if (FileName == "INI++15.mfx" && Version == 2)
            //    PopgoesHardcode();

            writer.WriteInt(Type);
            if (Type == -1)
            {
                writer.WriteAutoYunicode(Name);
                writer.WriteAutoYunicode(FileName);
                writer.WriteUInt(Magic);
                writer.WriteAutoYunicode(SubType);
            }

            int offset = 20 + (NebulaCore.Fusion == 1.5f ? 4 : 0);

            writer.WriteInt(Data.Length + offset);
            writer.WriteInt(Data.Length + offset);
            writer.WriteInt(-1);
            writer.WriteInt(Version);
            writer.WriteInt(ID);
            writer.WriteInt(Private);

            if (NebulaCore.Fusion == 1.5f)
                writer.Skip(4);

            writer.WriteBytes(Data);
        }

        private void PopgoesHardcode()
        {
            ByteReader reader = new ByteReader(Data);
            byte b_defaultFile = reader.ReadByte();
            byte b_ReadOnly = reader.ReadByte();
            string defaultFile = reader.ReadYunicode();
            int defaultFolder = reader.ReadInt();
            string defaultText = reader.ReadYunicode();
            byte bool_CanCreateFolders = reader.ReadByte();
            byte bool_AutoSave = reader.ReadByte();
            byte bool_stdINI = reader.ReadByte();
            byte bool_compress = reader.ReadByte();
            byte bool_encrypt = reader.ReadByte();
            string encrypt_key = reader.ReadYunicode();
            byte bool_newline = reader.ReadByte();
            string newline = reader.ReadYunicode();
            byte bool_QuoteStrings = reader.ReadByte();
            int repeatGroups = reader.ReadInt();
            sbyte repeatItems = reader.ReadSByte();
            sbyte undoCount = reader.ReadSByte();
            sbyte redoCount = reader.ReadSByte();
            byte saveRepeated = reader.ReadByte();
            byte bool_EscapeGroup = reader.ReadByte();
            byte bool_EscapeItem = reader.ReadByte();
            byte bool_EscapeValue = reader.ReadByte();
            byte bool_CaseSensitive = reader.ReadByte();
            byte globalObject = reader.ReadByte();
            byte index = reader.ReadByte();
            byte autoLoad = reader.ReadByte();
            byte subGroups = reader.ReadByte();
            byte allowEmptyGroups = reader.ReadByte();
            string globalKey = reader.ReadYunicode();
            reader.Dispose();

            ByteWriter writer = new ByteWriter(new MemoryStream());
            writer.WriteByte(b_defaultFile);
            writer.WriteByte(b_ReadOnly);
            writer.WriteAscii(defaultFile[..Math.Min(defaultFile.Length, 260)]);
            writer.Skip(Math.Max(0, 260 - defaultFile.Length));
            writer.Skip(2); // Allignment
            writer.WriteInt(defaultFolder);
            writer.WriteAscii(defaultText[..Math.Min(defaultText.Length, 3000)]);
            writer.Skip(Math.Max(0, 3000 - defaultText.Length));
            writer.WriteByte(bool_CanCreateFolders);
            writer.WriteByte(bool_AutoSave);
            writer.WriteByte(bool_stdINI);
            writer.WriteByte(bool_compress);
            writer.WriteByte(bool_encrypt);
            writer.WriteAscii(encrypt_key[..Math.Min(encrypt_key.Length, 128)]);
            writer.Skip(Math.Max(0, 128 - encrypt_key.Length));
            writer.WriteByte(bool_newline);
            writer.WriteAscii(newline[..Math.Min(newline.Length, 10)]);
            writer.Skip(Math.Max(0, 10 - newline.Length));
            writer.WriteByte(bool_QuoteStrings);
            writer.Skip(3); // Allignment
            writer.WriteInt(repeatGroups);
            writer.WriteByte((byte)repeatItems);
            writer.WriteByte((byte)undoCount);
            writer.WriteByte((byte)redoCount);
            writer.Skip(3); // Placeholder
            writer.WriteByte(saveRepeated);
            writer.WriteByte(bool_EscapeGroup);
            writer.WriteByte(bool_EscapeItem);
            writer.WriteByte(bool_EscapeValue);
            writer.WriteByte(bool_CaseSensitive);
            writer.WriteByte(globalObject);
            writer.WriteByte(index);
            writer.WriteByte(autoLoad);
            writer.WriteByte(subGroups);
            writer.WriteByte(allowEmptyGroups);
            writer.WriteAscii(globalKey[..Math.Min(globalKey.Length, 32)]);
            writer.Skip(Math.Max(0, 32 - globalKey.Length));
            writer.Skip(2); // Allignment

            Data = writer.ToArray();
            Version = 1;
            writer.Dispose();
        }
    }
}
