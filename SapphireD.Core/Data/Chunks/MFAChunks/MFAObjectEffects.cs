using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFAObjectEffects : Chunk
    {
        public Color RGBCoeff;
        public byte BlendCoeff;

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
            reader.Skip(4);

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
            chunkWriter.WriteInt(0);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
