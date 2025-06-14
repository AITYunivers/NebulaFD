using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Music
{
    public class MusicBank : Collection<MusicItem>, IReadable
    {
        public void Read(ByteReader reader)
        {
            string bankHeader = reader.ReadAscii(4);
            if (bankHeader != "ASUM")
                throw new InvalidDataException("Invalid music bank header. Expected ASUM, got " + bankHeader);

            int musicCount = reader.ReadInt();
            this.Log($"Found {musicCount} music", Logger.LogType.Debug);
            if (musicCount < 0)
                throw new InvalidDataException("Invalid music count. Expected greater than or equal to 0, got " + musicCount);

            foreach (MusicItem musicItem in reader.ReadIReadables<MusicItem>(musicCount))
            {
                Add(musicItem);
                this.Log($"Music {musicItem.Handle}: {musicItem.Name}", Logger.LogType.Debug);
            }
        }
    }
}
