using SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameInstances : Chunk
    {
        public FrameInstance[] Instances = new FrameInstance[0];
        public MFAObjectInfo[] MFAObjects = new MFAObjectInfo[0];

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
                Instances[i] = new FrameInstance();
                Instances[i].ReadCCN(reader);
            }

            ((Frame)extraInfo[0]).FrameInstances = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            MFAObjects = new MFAObjectInfo[reader.ReadInt()];
            for (int i = 0; i < MFAObjects.Length; i++)
            {
                MFAObjects[i] = new MFAObjectInfo();
                MFAObjects[i].ReadMFA(reader);
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
