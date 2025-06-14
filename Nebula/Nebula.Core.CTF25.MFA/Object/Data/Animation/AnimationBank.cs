using Nebula.Core.CTF25.MFA.Object.Data.Behaviour;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Object.Data.Animation
{
    internal class AnimationBank : Collection<AnimationItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int animationCount = reader.ReadInt();
            this.Log($"Found {animationCount} animation(s)", Logger.LogType.Debug);
            if (animationCount < 0)
                throw new InvalidDataException("Invalid animation count. Expected greater than or equal to 0, got " + animationCount);

            foreach (AnimationItem animationItem in reader.ReadIReadables<AnimationItem>(animationCount))
                Add(animationItem);

            for (int i = 0; i < animationCount; i++)
                this.Log($"Animation {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
