using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterObject : ParameterChunk
    {
        public short ObjectInfoList;
        public ushort ObjectInfo;
        public short ObjectType;

        public ParameterObject()
        {
            ChunkName = "ParameterObject";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfoList = reader.ReadShort();
            ObjectInfo = reader.ReadUShort();
            ObjectType = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            if (FrameEvents.QualifierJumptable.ContainsKey(ObjectInfo))
                ObjectInfo = FrameEvents.QualifierJumptable[ObjectInfo];

            writer.WriteShort(ObjectInfoList);
            writer.WriteUShort(ObjectInfo);
            writer.WriteShort(ObjectType);
        }

        public override string ToString()
        {
            return NebulaCore.PackageData.FrameItems.Items[ObjectInfo].Name;
        }
    }
}
