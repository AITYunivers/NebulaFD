using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.LayerChunks
{
    public class FrameLayer : Chunk
    {
        public int Options;
        public float XCoefficient;
        public float YCoefficient;
        public int BackdropCount;
        public int BackdropIndex;
        public string Name;

        public FrameLayer()
        {
            ChunkName = "FrameLayer";
        }

        public override void ReadCCN(ByteReader reader)
        {
            Options = reader.ReadInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
            BackdropCount = reader.ReadInt();
            BackdropIndex = reader.ReadInt();
            Name = reader.ReadYuniversal();
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
