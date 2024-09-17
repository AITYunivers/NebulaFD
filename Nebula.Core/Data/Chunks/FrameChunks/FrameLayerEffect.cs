using Nebula.Core.Data.Chunks.BankChunks.Shaders;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameLayerEffect : Chunk
    {
        public int InkEffect;
        public uint InkEffectParam;
        public Color RGBCoeff = Color.White;
        public byte BlendCoeff;
        public int ShaderHandle;
        public Shader Shader = new Shader();
        public ShaderParameter[] ShaderParameters = new ShaderParameter[0];

        public FrameLayerEffect()
        {
            ChunkName = "FrameLayerEffect";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long startOffset = (long)extraInfo[0];
            InkEffect = reader.ReadShort();
            reader.Skip(2);
            var b = reader.ReadByte();
            var g = reader.ReadByte();
            var r = reader.ReadByte();
            RGBCoeff = Color.FromArgb(0, r, g, b);
            BlendCoeff = (byte)(255 - reader.ReadByte());
            ShaderHandle = reader.ReadInt();
            ShaderParameters = new ShaderParameter[reader.ReadInt()];
            int paramOffset = reader.ReadInt();

            if (ShaderHandle >= 0)
                Shader = NebulaCore.PackageData.ShaderBank[ShaderHandle];

            long returnOffset = reader.Tell();
            if (paramOffset != 0)
            {
                reader.Seek(startOffset + paramOffset);
                for (int i = 0; i < Shader.Parameters.Length; i++)
                {
                    ShaderParameters[i] = new ShaderParameter();
                    ShaderParameters[i].Name = Shader.Parameters[i].Name;
                    ShaderParameters[i].Type = Shader.Parameters[i].Type;
                    if (ShaderParameters[i].Type == 1)
                        ShaderParameters[i].FloatValue = reader.ReadFloat();
                    else
                        ShaderParameters[i].Value = reader.ReadInt();
                }
                Array.Resize(ref ShaderParameters, Shader.Parameters.Length);
            }
            reader.Seek(returnOffset);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            InkEffect = reader.ReadInt();
            var b = reader.ReadByte();
            var g = reader.ReadByte();
            var r = reader.ReadByte();
            RGBCoeff = Color.FromArgb(0, r, g, b);
            BlendCoeff = (byte)(255 - reader.ReadByte());
            bool hasShader = reader.ReadInt() == 1;

            ShaderBank bnk = NebulaCore.PackageData.ShaderBank;
            if (hasShader)
            {
                Shader = new Shader();
                Shader.ReadMFA(reader);
                Shader.Handle = bnk.MFAShaderLookup.Contains(Shader.Name) ? bnk.MFAShaderLookup.IndexOf(Shader.Name) : bnk.Shaders.Count;
                ShaderParameters = new ShaderParameter[Shader.Parameters.Length];
                for (int i = 0; i < Shader.Parameters.Length; i++)
                {
                    ShaderParameters[i] = new ShaderParameter();
                    ShaderParameters[i].Name = Shader.Parameters[i].Name;
                    ShaderParameters[i].Type = Shader.Parameters[i].Type;
                    ShaderParameters[i].Value = Shader.Parameters[i].Value;
                    ShaderParameters[i].FloatValue = Shader.Parameters[i].FloatValue;
                }

                if (!bnk.Shaders.ContainsKey(Shader.Handle))
                {
                    bnk.Shaders.Add(Shader.Handle, Shader);
                    bnk.MFAShaderLookup.Add(Shader.Name);
                }
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(InkEffect);
            writer.WriteByte(RGBCoeff.B);
            writer.WriteByte(RGBCoeff.G);
            writer.WriteByte(RGBCoeff.R);
            writer.WriteByte((byte)(255 - BlendCoeff));
            writer.WriteInt(Shader.Handle >= 0 ? 1 : 0);
            if (Shader.Handle >= 0)
            {
                Shader.Parameters = ShaderParameters;
                Shader.WriteMFA(writer);
            }
        }
    }
}
