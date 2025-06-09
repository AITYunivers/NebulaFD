using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Frame
{
    internal class FrameBank : IReadable
    {
        private FrameItem[] _frameItems = [];

        public void Read(ByteReader reader)
        {
            int frameCount = reader.ReadInt();
            this.Log($"Found {frameCount} frame(s)", Logger.LogType.Debug);
            if (frameCount < 0)
                throw new InvalidDataException("Invalid frame count. Expected greater than or equal to 0, got " + frameCount);

            int[] frameOffsets = new int[frameCount];
            for (int i = 0; i < frameCount; i++)
                frameOffsets[i] = reader.ReadInt();

            long returnOffset = reader.Tell();
            _frameItems = new FrameItem[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                reader.Seek(frameOffsets[i], SeekOrigin.Begin);
                FrameItem frameItem = new FrameItem();
                frameItem.Read(reader);
                _frameItems[i] = frameItem;
                this.Log($"Frame {i}: {frameItem.Name}", Logger.LogType.Debug);
            }
        }

        public FrameItem this[int index]
        {
            get => _frameItems[index];
            set => _frameItems[index] = value;
        }
    }
}
