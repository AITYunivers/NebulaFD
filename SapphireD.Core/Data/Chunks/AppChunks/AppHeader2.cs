using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class AppHeader2 : Chunk
    {
        public int hdr2Options;       // Options
        public short hdr2Orientation; // Orientation

        public AppHeader2()
        {
            ChunkName = "AppHeader2";
            ChunkID = 0x2245;
        }

        public override void ReadCCN(ByteReader reader)
        {
            hdr2Options = reader.ReadInt();
            reader.Skip(10);
            hdr2Orientation = reader.ReadShort();

            SapDCore.PackageData.AppHeader2 = this;
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
