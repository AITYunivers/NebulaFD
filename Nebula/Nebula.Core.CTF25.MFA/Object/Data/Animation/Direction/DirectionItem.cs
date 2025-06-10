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

            FrameHandles = new uint[reader.ReadInt()];
            for (int i = 0; i < FrameHandles.Length; i++)
                FrameHandles[i] = reader.ReadUInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(Handle);
            writer.WriteInt(MinSpeed);
            writer.WriteInt(MaxSpeed);
            writer.WriteInt(RepeatCount);
            writer.WriteInt(RepeatFrom);

            writer.WriteInt(FrameHandles.Length);
            foreach (uint frameHandle in FrameHandles)
                writer.WriteUInt(frameHandle);
        }
    }
}
