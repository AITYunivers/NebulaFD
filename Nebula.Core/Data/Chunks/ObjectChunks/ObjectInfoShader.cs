using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfoShader : Chunk
    {
        public int? ShaderHandle;
        public int[] ShaderParameters = new int[0];

        public ObjectInfoShader()
        {
            ChunkName = "ObjectInfoShader";
            ChunkID = 0x4448;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ShaderHandle = reader.ReadInt();
            ShaderParameters = new int[reader.ReadInt()];

            this.Log($"Shader ID {ShaderHandle}, Parameters: {ShaderParameters.Length}", Spectre.Console.Color.DeepPink1_1);

            for (int i = 0; i < ShaderParameters.Length; i++)
                ShaderParameters[i] = reader.ReadInt();

            ((ObjectInfo)extraInfo[0]).Shader = this;
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
