using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectShaders : Chunk
    {
        public ObjectShaders()
        {
            ChunkName = "ObjectShaders";
            ChunkID = 0x2255;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int curHandle = 0;
            FrameItems listItems = NebulaCore.PackageData.FrameItems;
            while (reader.Size() > reader.Tell())
            {
                ObjectInfoShader oIShader = new ObjectInfoShader();
                int size = reader.ReadInt();
                int start = (int)reader.Tell();
                if (size > 0)
                    listItems.Items[curHandle].Shader.ReadCCN(reader, listItems.Items[curHandle]);
                reader.Seek(start + size);
                curHandle++;
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
