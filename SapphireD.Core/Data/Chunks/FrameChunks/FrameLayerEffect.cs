using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameLayerEffect : Chunk
    {
        public short Effect;
        public short EffectParameter;
        public Color RGBCoefficient;
        public int InkEffect;
        //public EffectShader Shader = new EffectShader();

        public FrameLayerEffect()
        {
            ChunkName = "FrameLayerEffect";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Effect = reader.ReadShort();
            EffectParameter = reader.ReadShort();
            RGBCoefficient = reader.ReadColor();
            InkEffect = reader.ReadInt();
            reader.Skip(8);
            /*Shader.Parameters = new EffectParameter[reader.ReadInt()];
            int dataOffset = reader.ReadInt();
            if (dataOffset == 0) return;

            long returnOffset = reader.Tell();
            reader.Seek(dataOffset);

            ShaderInfo parentShader = SapDCore.PackageData.Shaders.ShaderInfos[InkEffect];
            Shader.Name = parentShader.Name;
            Shader.Handle = InkEffect;

            for (int i = 0; i < Shader.Parameters.Length; i++)
            {
                Shader.Parameters[i] = new EffectParameter();
                Shader.Parameters[i].Name = parentShader.Parameters[i].Name;
                Shader.Parameters[i].Type = parentShader.Parameters[i].Type;
                Shader.Parameters[i].Data = reader.ReadBytes(4);
            }

            reader.Seek(returnOffset);*/
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
