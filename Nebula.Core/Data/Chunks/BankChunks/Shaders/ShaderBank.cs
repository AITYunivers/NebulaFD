using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Shaders
{
    public class ShaderBank : Chunk
    {
        public int[] Offsets = new int[0];
        public Dictionary<int, Shader> Shaders = new();
        public List<string> MFAShaderLookup = new();

        public ShaderBank()
        {
            ChunkName = "ShaderBank";
            ChunkID = 0x2243;
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
                    Shaders.Add(shd.Handle = i, shd);
                }
            }

            NebulaCore.PackageData.ShaderBank = this;
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

        public Shader this[int key]
        {
            get => (Shaders.ContainsKey(key) ? Shaders[key] : NebulaCore.PackageData.DX9ShaderBank[key]);
            set
            {
                if ((!Shaders.ContainsKey(key) || Shaders[key] == null) && NebulaCore.PackageData.DX9ShaderBank.Shaders.ContainsKey(key))
                    NebulaCore.PackageData.DX9ShaderBank[key] = value;
                else
                    Shaders[key] = value;
            }
        }

        public Shader this[uint key]
        {
            get => (Shaders.ContainsKey((int)key) ? Shaders[(int)key] : NebulaCore.PackageData.DX9ShaderBank[key]);
            set
            {
                if ((!Shaders.ContainsKey((int)key) || Shaders[(int)key] == null) && NebulaCore.PackageData.DX9ShaderBank.Shaders.ContainsKey((int)key))
                    NebulaCore.PackageData.DX9ShaderBank[key] = value;
                else
                    Shaders[(int)key] = value;
            }
        }
    }
}
