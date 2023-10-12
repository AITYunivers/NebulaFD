using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class TransitionChunk : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public string ModuleName = string.Empty;
        public int Module;
        public int ID;
        public int Duration;
        public Color Color = Color.Black;

        public string FileName = string.Empty;
        public byte[] ParameterData = new byte[0];

        public TransitionChunk()
        {
            ChunkName = "TransitionChunk";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();

            Module = reader.ReadInt();
            ID = reader.ReadInt();
            Duration = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Color = reader.ReadColor();

            int NameOffset = reader.ReadInt();
            int DataOffset = reader.ReadInt();
            int DataSize = reader.ReadInt();

            reader.Seek(StartOffset + NameOffset);
            FileName = reader.ReadYuniversal();
            reader.Seek(StartOffset + DataOffset);
            ParameterData = reader.ReadBytes(DataSize);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ModuleName = reader.ReadAutoYuniversal();
            FileName = reader.ReadAutoYuniversal();
            Module = reader.ReadInt();
            ID = reader.ReadInt();
            Duration = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Color = reader.ReadColor();
            ParameterData = reader.ReadBytes(reader.ReadInt());
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
