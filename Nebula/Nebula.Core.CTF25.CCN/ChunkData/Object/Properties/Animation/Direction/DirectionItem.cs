using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Object.Data.Animation.Direction
{
    internal class DirectionItem : IReadable, IWritable
    {
        public byte MinSpeed;
        public byte MaxSpeed;
        public short RepeatCount;
        public ushort RepeatFrom;
        public uint[] FrameHandles = [];

        public void Read(ByteReader reader)
        {
            MinSpeed = reader.ReadByte();
            MaxSpeed = reader.ReadByte();
            RepeatCount = reader.ReadShort();
            RepeatFrom = reader.ReadUShort();
            FrameHandles = reader.ReadUInts(reader.ReadInt());
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteByte(MinSpeed);
            writer.WriteByte(MaxSpeed);
            writer.WriteShort(RepeatCount);
            writer.WriteUShort(RepeatFrom);
            writer.WriteInt(FrameHandles.Length);
            writer.WriteUInts(FrameHandles);
        }
    }
}
