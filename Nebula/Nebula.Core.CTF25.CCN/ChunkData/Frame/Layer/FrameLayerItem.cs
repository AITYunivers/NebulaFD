using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Xml.Linq;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame.Layer
{
    internal class FrameLayerItem : IReadable, IWritable
    {
        public uint Flags;
        public float XCoefficient;
        public float YCoefficient;
        public int BackdropCount; // Is this right?
        public int BackdropIndex; // Is this right?
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Flags = reader.ReadUInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
            BackdropCount = reader.ReadInt();
            BackdropIndex = reader.ReadInt();
            Name = reader.ReadYuniversal();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(Flags);
            writer.WriteFloat(XCoefficient);
            writer.WriteFloat(YCoefficient);
            writer.WriteInt(BackdropCount);
            writer.WriteInt(BackdropIndex);
            writer.WriteYunicode(Name);
        }
    }
}
