using Nebula.Core.CTF25.MFA.Object.Data.Behaviour;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation
{
    internal class AnimationBank : IReadable, IWritable
    {
        private AnimationItem[] _animationItems = [];

        public void Read(ByteReader reader)
        {
            int animationCount = reader.ReadInt();
            this.Log($"Found {animationCount} animation(s)", Logger.LogType.Debug);
            if (animationCount < 0)
                throw new InvalidDataException("Invalid animation count. Expected greater than or equal to 0, got " + animationCount);

            _animationItems = new AnimationItem[animationCount];
            for (int i = 0; i < animationCount; i++)
            {
                AnimationItem animationItem = new AnimationItem();
                animationItem.Read(reader);
                _animationItems[i] = animationItem;
                this.Log($"Animation {i}: {animationItem.Name}", Logger.LogType.Debug);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_animationItems.Length);
            foreach (AnimationItem animationItem in _animationItems)
                animationItem.Write(writer);
        }

        public AnimationItem this[int index]
        {
            get => _animationItems[index];
            set => _animationItems[index] = value;
        }
    }
}
