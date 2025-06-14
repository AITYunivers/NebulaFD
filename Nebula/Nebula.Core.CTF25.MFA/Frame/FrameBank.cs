using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Frame
{
    internal class FrameBank : Collection<FrameItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int frameCount = reader.ReadInt();
            this.Log($"Found {frameCount} frame(s)", Logger.LogType.Debug);
            if (frameCount < 0)
                throw new InvalidDataException("Invalid frame count. Expected greater than or equal to 0, got " + frameCount);

            int[] frameOffsets = reader.ReadInts(frameCount);
            int returnOffset = reader.ReadInt();
            foreach (FrameItem frameItem in reader.ReadIReadables<FrameItem, int>(frameCount, frameOffsets))
                Add(frameItem);
            reader.Seek(returnOffset);

            for (int i = 0; i < frameCount; i++)
                this.Log($"Frame {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteIWritablesWithOffsets<FrameItem, int>(this);
        }
    }
}
