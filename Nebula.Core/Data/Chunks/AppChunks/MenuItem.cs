using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class MenuItem : Chunk
    {
        public BitDict Flags = new BitDict( // Flags
            "Disabled", "", "", 
            "Checked",
            "Parent", "", "",
            "Footer"
        );

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
            if (!Flags["Parent"])
                ID = reader.ReadUShort();

            Name = reader.ReadWideString();
            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == '&')
                {
                    if (i + 1 != Name.Length)
                        Mnemonic = Name[i + 1].ToString();
                    else
                        Mnemonic = "";
                    Name = Name.Replace("&", "");
                    break;
                }
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUShort((ushort)Flags.Value);
            if (!Flags["Parent"])
                writer.WriteUShort(ID);
            writer.WriteUnicode(string.IsNullOrEmpty(Mnemonic) ? Name : Name.ReplaceFirst(Mnemonic, "&" + Mnemonic), true);

            if (Flags["Parent"])
                foreach (MenuItem menuItem in Items)
                    menuItem.WriteMFA(writer);
        }
    }
}
