using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Animation
{
    internal class AnimationBank : Collection<AnimationItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            reader.Skip(2); // Size
            short animationCount = reader.ReadShort();
            this.Log($"Found {animationCount} animation(s)", Logger.LogType.Debug);
            if (animationCount < 0)
                throw new InvalidDataException("Invalid animation count. Expected greater than or equal to 0, got " + animationCount);
            
            foreach (AnimationItem animationItem in reader.ReadIReadables<AnimationItem, ushort>(animationCount))
                Add(animationItem);
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
