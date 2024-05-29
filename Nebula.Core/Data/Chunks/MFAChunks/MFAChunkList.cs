using Nebula.Core.Data.Chunks.AppChunks;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAChunkList
    {
        public static Dictionary<short, Type> ChunkJumpTable = new Dictionary<short, Type>()
        {
            { 0x0000, typeof(Last)                },
            { 0x003C, typeof(MFAExtraFlags)       },
            { 0x009A, typeof(MFAAppIcon)          }
        };
    }
}
