using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Sounds
{
    public class SoundBank : Chunk
    {
        public Dictionary<uint, Sound> Sounds;

        public SoundBank()
        {
            ChunkName = "SoundBank";
            ChunkID = 0x6668;
        }

        public override void ReadCCN(ByteReader reader)
        {
            Sounds = new();
            var count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Sound snd = new Sound();
                snd.ReadCCN(reader);
                Sounds[snd.Handle] = snd;
            }
            SapDCore.PackageData.SoundBank = this;
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
