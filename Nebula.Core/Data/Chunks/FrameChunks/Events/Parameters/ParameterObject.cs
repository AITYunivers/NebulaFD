using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterObject : ParameterChunk
    {
        public short ObjectInfoList;
        public short ObjectInfo;
        public short ObjectType;

        public ParameterObject()
        {
            ChunkName = "ParameterObject";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfoList = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectType = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(ObjectInfoList);
            writer.WriteShort(ObjectInfo);
            writer.WriteShort(ObjectType);
        }

        public override string ToString()
        {
            return NebulaCore.PackageData.FrameItems.Items[ObjectInfo].Name;
        }
    }
}
