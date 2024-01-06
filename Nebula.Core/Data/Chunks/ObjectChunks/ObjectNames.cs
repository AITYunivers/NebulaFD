using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectNames : Chunk
    {
        public ObjectNames()
        {
            ChunkName = "ObjectNames";
            ChunkID = 0x2254;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int curHandle = 0;
            FrameItems listItems = NebulaCore.PackageData.FrameItems;
            while (reader.Size() > reader.Tell())
                listItems.Items[curHandle++].Name = reader.ReadYuniversal();
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
