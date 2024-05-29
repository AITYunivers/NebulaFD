using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAObjectChunkList
    {
        public static Dictionary<short, Type> ChunkJumpTable = new Dictionary<short, Type>()
        {
            { 0x0000, typeof(Last)                },
            { 0x0016, typeof(MFACounterFlags)     },
            { 0x0017, typeof(MFACounterAltFlags)  },
            { 0x002D, typeof(MFAObjectEffects)    },
            { 0x0039, typeof(MFAAltFlags)         },
            { 0x003A, typeof(MFAAltValueIndex)    },
            { 0x003B, typeof(MFAAltStringIndex)   },
            { 0x003C, typeof(MFAAltFlagIndex)     },
        };
    }
}
