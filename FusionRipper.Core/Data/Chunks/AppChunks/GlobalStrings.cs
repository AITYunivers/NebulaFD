using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
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
            Strings = new string[reader.ReadInt()];
            for (int i = 0; i < Strings.Length; i++)
                Strings[i] = reader.ReadYuniversal();

            FRipCore.PackageData.GlobalStrings = this;
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
            writer.WriteInt(Strings.Length);
            for (int i = 0; i < Strings.Length; i++)
            {
                writer.WriteAutoYunicode(((GlobalStringNames)extraInfo[0]).Names.Length > i ? ((GlobalStringNames)extraInfo[0]).Names[i] : "");
                writer.WriteInt(2);
                writer.WriteAutoYunicode(Strings[i]);
            }
        }
    }
}
