using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Music
{
    public class MusicBank : IReadable
    {
        private MusicItem[] _musicItems = [];

        public void Read(ByteReader reader)
        {
            string bankHeader = reader.ReadAscii(4);
            if (bankHeader != "ASUM")
                throw new InvalidDataException("Invalid music bank header. Expected ASUM, got " + bankHeader);

            int musicCount = reader.ReadInt();
            this.Log($"Found {musicCount} music", Logger.LogType.Debug);
            if (musicCount < 0)
                throw new InvalidDataException("Invalid music count. Expected greater than or equal to 0, got " + musicCount);

            _musicItems = new MusicItem[musicCount];
            for (int i = 0; i < musicCount; i++)
            {
                MusicItem musicItem = new MusicItem();
                musicItem.Read(reader);
                _musicItems[i] = musicItem;

                this.Log($"Music {musicItem.Handle}: {musicItem.Name}", Logger.LogType.Debug);
            }
        }

        public MusicItem this[int index]
        {
            get => _musicItems[index];
            set => _musicItems[index] = value;
        }
    }
}
