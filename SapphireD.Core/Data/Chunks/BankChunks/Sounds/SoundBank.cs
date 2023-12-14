using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.BankChunks.Sounds
{
    public class SoundBank : Chunk
    {
        public int Count;
        public Dictionary<uint, Sound> Sounds = new();

        public SoundBank()
        {
            ChunkName = "SoundBank";
            ChunkID = 0x6668;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (SapDCore.Flash)
            {
                reader.Skip(2);
                Count = reader.ReadShort();
            }
            else
            {
                Count = reader.ReadInt();
            }
            for (int i = 0; i < Count; i++)
            {
                Sound snd = new Sound();
                snd.ReadCCN(reader);
                Sounds[snd.Handle] = snd;
            }
            SapDCore.PackageData.SoundBank = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt();
            for (int i = 0; i < Count; i++)
            {
                Sound snd = new Sound();
                snd.ReadMFA(reader);
                Sounds[snd.Handle] = snd;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Count);
            foreach (Sound snd in Sounds.Values)
                snd.WriteMFA(writer);
        }
    }
}
