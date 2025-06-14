using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class CounterData : IReadable, IWritable
    {
        public int Width;
        public int Height;
        public ushort PlayerHandle;
        public ushort DisplayType;
        public ushort Flags;
        public ushort FontHandle;
        public List<ushort>? FrameHandles;
        public ShapeData? Shape;

        public void Read(ByteReader reader)
        {
            reader.Skip(4); // Size
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            PlayerHandle = reader.ReadUShort();
            DisplayType = reader.ReadUShort();
            Flags = reader.ReadUShort();
            FontHandle = reader.ReadUShort();

            switch (DisplayType)
            {
                case 1: // Numbers
                case 4: // Animation
                    FrameHandles = [.. reader.ReadUShorts(reader.ReadUShort())];
                    break;
                case 2: // Vertical Bar
                case 3: // Horizontal Bar
                case 5: // Text
                    (Shape = new ShapeData()).Read(reader);
                    break;
            }
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
