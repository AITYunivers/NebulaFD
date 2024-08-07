using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts
{
    public class TrueTypeFontInfo : Chunk
    {
        public TrueTypeFontInfo()
        {
            ChunkName = "TrueTypeFontInfo";
            ChunkID = 0x2258;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            TrueTypeFontBank bank = NebulaCore.PackageData.TrueTypeFontBank;

            int count = (int)reader.Size() / 96;
            for (int i = 0; i < count; i++)
            {
                reader.Skip(28);
                string name = reader.ReadYuniversalStop(32);
                uint offset = reader.ReadUInt();

                if (bank.OffsetToIndex.ContainsKey(offset))
                    bank[bank.OffsetToIndex[offset]].Name = name;
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
