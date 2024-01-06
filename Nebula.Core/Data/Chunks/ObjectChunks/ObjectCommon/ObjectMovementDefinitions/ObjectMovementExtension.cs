using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementExtension : ObjectMovementDefinition
    {
        public string FileName;
        public int ID;
        public byte[] Data = new byte[0];

        public ObjectMovementExtension()
        {
            ChunkName = "ObjectMovementExtension";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            reader.Skip(14);
            Data = reader.ReadBytes((int)extraInfo[0] - 14);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Data = reader.ReadBytes((int)extraInfo[0] + 12);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteBytes(Data);
        }
    }
}
