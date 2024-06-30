using Nebula.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts
{
    public class TrueTypeFontInfo : Chunk
    {
        public TrueTypeFontInfo()
        {
            ChunkName = "TrueTypeFontInfo";
            ChunkID = 0x2258;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            TrueTypeFontBank bank = NebulaCore.PackageData.TrueTypeFontBank;

            int count = (int)reader.Size() / 96;
            List<string> names = new List<string>();
            int ii = 0;
            for (int i = 0; i < count; i++)
            {
                reader.Skip(28);
                string name = reader.ReadYunicodeStop(32);
                reader.Skip(4);

                if (!names.Contains(name))
                {
                    if (bank.Fonts.Count < ii)
                        bank.Fonts.Add(new TrueTypeFont());
                    bank[ii++].Name = name;
                    names.Add(name);
                }
            }
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
    }
}
