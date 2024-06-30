using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts
{
    public class TrueTypeFontBank : Chunk
    {
        public List<TrueTypeFont> Fonts = new();

        public TrueTypeFontBank()
        {
            ChunkName = "TrueTypeFontBank";
            ChunkID = 0x2259;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int i = 0;
            while (reader.HasMemory(8))
            {
                if (Fonts.Count <= i)
                    Fonts.Add(new TrueTypeFont());
                Fonts[i].ReadCCN(reader, false);
                Fonts[i].Name = i++.ToString();
            }

            NebulaCore.PackageData.TrueTypeFontBank = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }

        public TrueTypeFont this[int key]
        {
            get => Fonts[key];
            set => Fonts[key] = value;
        }

        public TrueTypeFont this[uint key]
        {
            get => Fonts[(int)key];
            set => Fonts[(int)key] = value;
        }
    }
}
