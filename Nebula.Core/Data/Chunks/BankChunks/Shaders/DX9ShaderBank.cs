using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Shaders
{
    public class DX9ShaderBank : Chunk
    {
        public int[] Offsets = new int[0];
        public Dictionary<int, Shader> Shaders = new();

        public DX9ShaderBank()
        {
            ChunkName = "DX9ShaderBank";
            ChunkID = 0x225A;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int Count = reader.ReadInt();
            Offsets = new int[Count];
            for (int i = 0; i < Count; i++)
                Offsets[i] = reader.ReadInt();

            Shaders = new();
            for (int i = 0; i < Count; i++)
            {
                if (Offsets[i] != 0)
                {
                    reader.Seek(Offsets[i]);
                    Shader shd = new Shader();
                    shd.ReadCCN(reader);
                    Shaders.Add(i + 1, shd);
                }
            }

            NebulaCore.PackageData.DX9ShaderBank = this;
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
