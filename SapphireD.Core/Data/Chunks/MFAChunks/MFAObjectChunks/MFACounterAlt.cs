using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFACounterAlt : MFAObjectLoader
    {
        public int Player;
        public uint[] Images = new uint[0];
        public bool UseText;
        public Color Color;
        public int Font;
        public int Width;
        public int Height;

        public MFACounterAlt()
        {
            // This is for Lives and Score objects
            ChunkName = "MFACounterAlt";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Player = reader.ReadInt();
            Images = new uint[reader.ReadInt()];
            for (int i = 0; i < Images.Length; i++)
                Images[i] = reader.ReadUInt();
            UseText = reader.ReadInt() != 0;
            Color = reader.ReadColor();
            Font = reader.ReadInt();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteInt(Player);
            writer.WriteInt(Images.Length);
            foreach (uint img in Images)
                writer.WriteUInt(img);
            writer.WriteInt(UseText ? 1 : 0);
            writer.WriteColor(Color);
            writer.WriteInt(Font);
            writer.WriteInt(Width);
            writer.WriteInt(Height);
        }
    }
}
