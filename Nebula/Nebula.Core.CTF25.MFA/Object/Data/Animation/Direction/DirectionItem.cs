using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation.Direction
{
    internal class DirectionItem : IReadable, IWritable
    {
        public uint Handle;
        public int MinSpeed;
        public int MaxSpeed;
        public int RepeatCount;
        public int RepeatFrom;
        public uint[] FrameHandles = [];

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            MinSpeed = reader.ReadInt();
            MaxSpeed = reader.ReadInt();
            RepeatCount = reader.ReadInt();
            RepeatFrom = reader.ReadInt();
            FrameHandles = reader.ReadUInts(reader.ReadInt());
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(Handle);
            writer.WriteInt(MinSpeed);
            writer.WriteInt(MaxSpeed);
            writer.WriteInt(RepeatCount);
            writer.WriteInt(RepeatFrom);
            writer.WriteInt(FrameHandles.Length);
            writer.WriteUInts(FrameHandles);
        }
    }
}
