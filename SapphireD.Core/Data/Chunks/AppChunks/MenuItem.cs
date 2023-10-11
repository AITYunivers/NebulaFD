using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class MenuItem : Chunk
    {
        public string Name = string.Empty;
        public ushort Flags;
        public ushort ID;
        public string Mnemonic = string.Empty;

        public List<MenuItem> Items;

        public MenuItem()
        {
            ChunkName = "MenuItem";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags = reader.ReadUInt16();
            if (!ByteFlag.GetFlag(Flags, 4))
                ID = reader.ReadUInt16();

            Name = reader.ReadWideString();
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

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
