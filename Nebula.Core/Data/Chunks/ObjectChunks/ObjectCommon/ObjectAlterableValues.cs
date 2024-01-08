using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAlterableValues : Chunk
    {
        public int[] AlterableValues = new int[0];
        public string[] Names = new string[0];
        public uint AlterableFlags;

        public ObjectAlterableValues()
        {
            ChunkName = "ObjectAlterableValues";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            AlterableValues = new int[reader.ReadShort()];
            Names = new string[AlterableValues.Length];
            for (int i = 0; i < AlterableValues.Length; i++)
            {
                Names[i] = string.Empty;
                AlterableValues[i] = reader.ReadInt();
            }

            AlterableFlags = reader.ReadUInt();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            AlterableValues = new int[reader.ReadInt()];
            Names = new string[AlterableValues.Length];
            for (int i = 0; i < AlterableValues.Length; i++)
            {
                Names[i] = reader.ReadAutoYuniversal();
                reader.Skip(4);
                AlterableValues[i] = reader.ReadInt();
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(AlterableValues.Length);
            for (int i = 0; i < AlterableValues.Length; i++)
            {
                writer.WriteAutoYunicode(Names[i]);
                writer.WriteInt(0);
                writer.WriteInt(AlterableValues[i]);
            }
        }
    }
}
