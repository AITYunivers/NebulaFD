using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.BankChunks.Music
{
    public class MusicBank : Chunk
    {
        public int Count;
        public Dictionary<uint, Music> Music = new();

        public MusicBank()
        {
            ChunkName = "MusicBank";
            ChunkID = 0x6669;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            NebulaCore.PackageData.MusicBank = this;
            if (Parameters.DontIncludeMusic)
                return;

            if (NebulaCore.Android || NebulaCore.iOS || NebulaCore.Flash || NebulaCore.HTML)
                Count = reader.ReadShort();
            else
                Count = reader.ReadInt();


            if (NebulaCore.Fusion == 1.5f)
                return;

            for (int i = 0; i < Count; i++)
            {
                Music msc = new Music();
                msc.ReadCCN(reader);
                Music[msc.Handle] = msc;
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt();
            for (int i = 0; i < Count; i++)
            {
                Music msc = new Music();
                msc.ReadMFA(reader);
                Music[msc.Handle] = msc;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Music.Count);
            foreach (Music msc in Music.Values)
                msc.WriteMFA(writer);
        }
    }
}
