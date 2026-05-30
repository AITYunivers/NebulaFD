using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
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
            if (FrameEvents.QualifierJumptable.ContainsKey(Tuple.Create(ObjectInfo, ObjectType)))
                ObjectInfo = FrameEvents.QualifierJumptable[Tuple.Create(ObjectInfo, ObjectType)];

            writer.WriteShort(ObjectInfoList);
            writer.WriteUShort(ObjectInfo);
            writer.WriteShort(ObjectType);
        }

        public override string ToString()
        {
            ObjectInfo? objectInfo = GetObject();
            if (objectInfo != null)
                return objectInfo.Name;
            else
                return ObjectCommon.TryGetQualifier(this, ObjectInfo, ObjectType);
        }

        public ObjectInfo? GetObject()
        {
            // added here just because without it - any other changes with qualifiers won't do anything (will still get KeyNotFoundException, means that it won't get added)
            if (NebulaCore.Windows && NebulaCore.Build >= 296 && NebulaCore.Fusion >= 2.5 && (ObjectInfo & 0x8000) != 0)
                return null;
            if (Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo).Any() == true)
                return null;
            else if (NebulaCore.MFA && Parent?.FrameEvents?.EventObjects.Count > 0)
                return NebulaCore.PackageData.FrameItems.Items[(int)Parent.FrameEvents.EventObjects[ObjectInfo].ItemHandle];
            else
                return NebulaCore.PackageData.FrameItems.Items[ObjectInfo];
        }
    }
}
