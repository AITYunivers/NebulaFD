using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.MFAChunks
{
    public class MFAExtensions : Chunk
    {
        public MFAExtension[] Extensions = new MFAExtension[0];

        public MFAExtensions()
        {
            ChunkName = "MFAExtensions";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Extensions = new MFAExtension[reader.ReadInt()];
            for (int i = 0; i < Extensions.Length; i++)
            {
                Extensions[i] = new MFAExtension();
                Extensions[i].ReadMFA(reader);
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
