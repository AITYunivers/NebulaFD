using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Sound
{
    public class SoundBank : Collection<SoundItem>, IReadable
    {
        public void Read(ByteReader reader)
        {
            string bankHeader = reader.ReadAscii(4);
            if (bankHeader != "APMS")
                throw new InvalidDataException("Invalid sound bank header. Expected APMS, got " + bankHeader);

            int soundCount = reader.ReadInt();
            this.Log($"Found {soundCount} sound(s)", Logger.LogType.Debug);
            if (soundCount < 0)
                throw new InvalidDataException("Invalid sound count. Expected greater than or equal to 0, got " + soundCount);

            foreach (SoundItem soundItem in reader.ReadIReadables<SoundItem>(soundCount))
            {
                Add(soundItem);
                this.Log($"Sound {soundItem.Handle}: {soundItem.Name}", Logger.LogType.Debug);
            }
        }
    }
}
