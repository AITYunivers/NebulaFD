using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class GlobalStrings : Chunk
    {
        public string[] Strings = new string[0];

        public GlobalStrings()
        {
            ChunkName = "GlobalStrings";
            ChunkID = 0x2233;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.ReadInt();
            List<string> tempStrings = new();
            while (reader.HasMemory(2))
                tempStrings.Add(reader.ReadYuniversal());
            Strings = tempStrings.ToArray();

            SapDCore.PackageData.GlobalStrings = this;
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
