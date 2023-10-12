using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAlterableValues : Chunk
    {
        public int[] AlterableValues = new int[0];
        public string[] AlterableValueNames = new string[0];
        public uint AlterableFlags;

        public ObjectAlterableValues()
        {
            ChunkName = "ObjectAlterableValues";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            AlterableValues = new int[reader.ReadShort()];
            AlterableValueNames = new string[AlterableValues.Length];
            for (int i = 0; i < AlterableValues.Length; i++)
                AlterableValues[i] = reader.ReadInt();

            AlterableFlags = reader.ReadUInt();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            AlterableValues = new int[reader.ReadInt()];
            AlterableValueNames = new string[AlterableValues.Length];
            for (int i = 0; i < AlterableValues.Length; i++)
            {
                AlterableValueNames[i] = reader.ReadAutoYuniversal();
                reader.Skip(4);
                AlterableValues[i] = reader.ReadInt();
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
