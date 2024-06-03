using Nebula.Core.Memory;
using System.Text;

namespace Nebula.Core.Data.Chunks.BankChunks.Shaders
{
    public class Shader : Chunk
    {
        public int Handle;
        public string Name = string.Empty;
        public bool Compiled = false;
        public byte[] FXData = new byte[0];
        public ShaderParameter[] Parameters = new ShaderParameter[0];

        public Shader()
        {
            ChunkName = "Shader";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int StartOffset = (int)reader.Tell();
            int NameOffset = reader.ReadInt();
            int FXDataOffset = reader.ReadInt();
            int ParameterOffset = reader.ReadInt();
            int OptionsOffset = reader.ReadInt();
            int FXDataSize = reader.ReadInt();

            if (NameOffset != 0)
            {
                reader.Seek(StartOffset + NameOffset);
                Name = reader.ReadAscii();
            }

            if (FXDataOffset != 0)
            {
                reader.Seek(StartOffset + FXDataOffset);
                string header = reader.ReadAscii(4);
                    Compiled = header == "DXBC";
                reader.Skip(-4);
                if (Compiled)
                    FXData = reader.ReadBytes(--FXDataSize);
                else
                    FXData = Encoding.ASCII.GetBytes(reader.ReadAscii());
            }

            if (ParameterOffset != 0)
            {
                reader.Seek(StartOffset + ParameterOffset);
                Parameters = new ShaderParameter[reader.ReadInt()];
                if (Parameters.Length > 0)
                {
                    int ParameterTypeOffset = reader.ReadInt();
                    int ParameterNameOffset = reader.ReadInt();

                    reader.Seek(StartOffset + ParameterOffset + ParameterTypeOffset);
                    for (int i = 0; i < Parameters.Length; i++)
                    {
                        Parameters[i] = new ShaderParameter();
                        Parameters[i].Type = reader.ReadByte();
                    }

                    reader.Seek(StartOffset + ParameterOffset + ParameterNameOffset);
                    for (int i = 0; i < Parameters.Length; i++)
                        Parameters[i].Name = reader.ReadAscii();
                }
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Name = reader.ReadAutoYuniversal();
            Parameters = new ShaderParameter[reader.ReadInt()];
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new ShaderParameter();
                Parameters[i].ReadMFA(reader);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Parameters.Length);
            foreach (var parameter in Parameters)
                parameter.WriteMFA(writer);
        }

        public string GetFXData()
        {
            return Encoding.ASCII.GetString(FXData);
        }
    }
}
