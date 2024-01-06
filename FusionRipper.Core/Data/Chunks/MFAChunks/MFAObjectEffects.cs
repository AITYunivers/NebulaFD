using FusionRipper.Core.Data.Chunks.BankChunks.Shaders;
using FusionRipper.Core.Memory;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace FusionRipper.Core.Data.Chunks.MFAChunks
{
    public class MFAObjectEffects : Chunk
    {
        public Color RGBCoeff = Color.White;
        public byte BlendCoeff;
        public int ShaderHandle;
        public Shader Shader = new Shader();
        public ShaderParameter[] ShaderParameters = new ShaderParameter[0];

        public MFAObjectEffects()
        {
            ChunkName = "MFAObjectEffects";
            ChunkID = 0x002D;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            var b = reader.ReadByte();
            var g = reader.ReadByte();
            var r = reader.ReadByte();
            RGBCoeff = Color.FromArgb(0, r, g, b);
            BlendCoeff = (byte)(255 - reader.ReadByte());
            ShaderHandle = reader.ReadInt();

            if (ShaderHandle != 0)
            {
                Shader = new Shader();
                Shader.ReadMFA(reader);
                ShaderParameters = new ShaderParameter[Shader.Parameters.Length];
                for (int i = 0; i < Shader.Parameters.Length; i++)
                {
                    ShaderParameters[i] = new ShaderParameter();
                    ShaderParameters[i].Name = Shader.Parameters[i].Name;
                    ShaderParameters[i].Type = Shader.Parameters[i].Type;
                    ShaderParameters[i].Value = Shader.Parameters[i].Value;
                    ShaderParameters[i].FloatValue = Shader.Parameters[i].FloatValue;
                }

                if (!FRipCore.PackageData.ShaderBank.Shaders.ContainsKey(ShaderHandle))
                    FRipCore.PackageData.ShaderBank.Shaders.Add(ShaderHandle, Shader);
            }

            (extraInfo[0] as MFAObjectInfo).ObjectEffects = this;
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteByte((byte)ChunkID);
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            chunkWriter.WriteByte(RGBCoeff.B);
            chunkWriter.WriteByte(RGBCoeff.G);
            chunkWriter.WriteByte(RGBCoeff.R);
            chunkWriter.WriteByte((byte)(255 - BlendCoeff));
            chunkWriter.WriteInt(0);//ShaderHandle);
            if (true == false && ShaderHandle != 0)
            {
                Shader.Parameters = ShaderParameters;
                Shader.WriteMFA(chunkWriter);
            }
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
