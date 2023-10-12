using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class MenuItem : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "0", "1", "2", "3", "4", "5", "6", "7"
        });

        public string Name = string.Empty;
        public ushort ID;
        public string Mnemonic = string.Empty;

        public List<MenuItem> Items;

        public MenuItem()
        {
            ChunkName = "MenuItem";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags.Value = reader.ReadUShort();
            if (!Flags["4"])
                ID = reader.ReadUShort();

            Name = reader.ReadYuniversal();
            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == '&')
                {
                    Mnemonic = Name[i + 1].ToString();
                    Name = Name.Replace("&", "");
                    break;
                }
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Flags.Value = reader.ReadUShort();
            if (!Flags["4"])
                ID = reader.ReadUShort();

            Name = reader.ReadYuniversal();
            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == '&')
                {
                    Mnemonic = Name[i + 1].ToString();
                    Name = Name.Replace("&", "");
                    break;
                }
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
