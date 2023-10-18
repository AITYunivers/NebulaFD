using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.LayerChunks
{
    public class FrameLayer : Chunk
    {
        public BitDict Options = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public float XCoefficient;
        public float YCoefficient;
        public int BackdropCount;
        public int BackdropIndex;
        public string Name = string.Empty;
        public FrameLayerEffect Effect = new FrameLayerEffect();

        public FrameLayer()
        {
            ChunkName = "FrameLayer";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Options.Value = reader.ReadUInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
            BackdropCount = reader.ReadInt();
            BackdropIndex = reader.ReadInt();
            Name = reader.ReadYuniversal();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Name = reader.ReadAutoYuniversal();
            Options.Value = reader.ReadUInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
