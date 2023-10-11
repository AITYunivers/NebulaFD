using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameInstances : Chunk
    {
        public FrameInstance[] Instances = new FrameInstance[0];

        public FrameInstances()
        {
            ChunkName = "FrameInstances";
            ChunkID = 0x3338;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Instances = new FrameInstance[reader.ReadInt()];
            for (int i = 0; i < Instances.Length; i++)
            {
                FrameInstance instance = new FrameInstance();
                instance.ReadCCN(reader);
                Instances[i] = instance;
            }

            ((Frame)extraInfo[0]).FrameInstances = this;
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
