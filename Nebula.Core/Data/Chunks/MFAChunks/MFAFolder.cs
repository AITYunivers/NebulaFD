using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAFolder : Chunk
    {
        public string Name = string.Empty;
        public byte Header;
        public bool HiddenFolder;
        public int[] Children = new int[0];


        public MFAFolder()
        {
            ChunkName = "MFAFolder";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(3);
            Header = reader.ReadByte();

            if (Header == 4)
            {
                HiddenFolder = false;
                Name = reader.ReadAutoYuniversal();
                Children = new int[reader.ReadInt()];
            }
            else
            {
                HiddenFolder = true;
                Children = new int[1];
            }

            for (int i = 0; i < Children.Length; i++)
                Children[i] = reader.ReadInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(112);
            writer.WriteByte(0);

            if (HiddenFolder)
                writer.WriteByte(3);
            else
            {
                writer.WriteByte(4);
                writer.WriteAutoYunicode(Name);
                writer.WriteInt(Children.Length);
            }

            foreach (int child in Children)
                writer.WriteInt(child);
        }
    }
}
