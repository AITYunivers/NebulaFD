using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Animation
{
    internal class AnimationBank : IReadable, IWritable
    {
        private AnimationItem[] _animationItems = [];

        public void Read(ByteReader reader)
        {
            reader.Skip(2); // Size
            short animationCount = reader.ReadShort();
            this.Log($"Found {animationCount} animation(s)", Logger.LogType.Debug);
            if (animationCount < 0)
                throw new InvalidDataException("Invalid animation count. Expected greater than or equal to 0, got " + animationCount);
            
            _animationItems = reader.ReadIReadables<AnimationItem, ushort>(animationCount);
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }

        public AnimationItem this[int index]
        {
            get => _animationItems[index];
            set => _animationItems[index] = value;
        }
    }
}
