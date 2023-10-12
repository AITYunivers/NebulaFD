using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFACounter : MFAObjectLoader
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int Value;
        public int Minimum;
        public int Maximum;
        public uint DisplayType;
        public Color Color1;
        public Color Color2;
        public uint Gradient;
        public int ColorType;
        public int Width;
        public int Height;
        public int[] Images = new int[0];
        public uint Font;

        public MFACounter()
        {
            ChunkName = "MFACounter";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Value = reader.ReadInt();
            Minimum = reader.ReadInt();
            Maximum = reader.ReadInt();
            DisplayType = reader.ReadUInt();
            Flags.Value = reader.ReadUInt();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            Gradient = reader.ReadUInt();
            ColorType = reader.ReadInt();
            Width = reader.ReadInt();
            Height = reader.ReadInt();

            Images = new int[reader.ReadUInt()];
            for (int i = 0; i < Images.Length; i++)
                Images[i] = reader.ReadInt();

            Font = reader.ReadUInt();
        }
    }
}
