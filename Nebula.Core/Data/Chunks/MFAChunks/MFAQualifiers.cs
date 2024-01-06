using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAQualifiers : Chunk
    {
        public MFAQualifier[] Qualifiers = new MFAQualifier[0];

        public MFAQualifiers()
        {
            ChunkName = "MFAQualifiers";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Qualifiers = new MFAQualifier[reader.ReadInt()];
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                Qualifiers[i] = new MFAQualifier();
                Qualifiers[i].ReadMFA(reader);
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
