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
            Strings = new string[reader.ReadInt()];
            ((GlobalStringNames)extraInfo[0]).Names = new string[Strings.Length];
            for (int i = 0; i < Strings.Length; i++)
            {
                ((GlobalStringNames)extraInfo[0]).Names[i] = reader.ReadAutoYuniversal();
                reader.Skip(4); // Type (Always string)
                Strings[i] = reader.ReadAutoYuniversal();
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
