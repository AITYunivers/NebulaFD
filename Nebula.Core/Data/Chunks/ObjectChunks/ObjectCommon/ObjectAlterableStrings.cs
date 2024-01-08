using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAlterableStrings : Chunk
    {
        public string[] AlterableStrings = new string[0];
        public string[] Names = new string[0];

        public ObjectAlterableStrings()
        {
            ChunkName = "ObjectAlterableStrings";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            AlterableStrings = new string[reader.ReadShort()];
            Names = new string[AlterableStrings.Length];
            for (int i = 0; i < AlterableStrings.Length; i++)
            {
                Names[i] = string.Empty;
                AlterableStrings[i] = reader.ReadYuniversal();
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            AlterableStrings = new string[reader.ReadInt()];
            Names = new string[AlterableStrings.Length];
            for (int i = 0; i < AlterableStrings.Length; i++)
            {
                Names[i] = reader.ReadAutoYuniversal();
                int type = reader.ReadInt();// reader.Skip(4);
                AlterableStrings[i] = reader.ReadAutoYuniversal();
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(AlterableStrings.Length);
            for (int i = 0; i < AlterableStrings.Length; i++)
            {
                writer.WriteAutoYunicode(Names[i]);
                writer.WriteInt(2);
                writer.WriteAutoYunicode(AlterableStrings[i]);
            }
        }
    }
}
