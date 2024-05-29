using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAFrameChunks
{
    public class MFAFrameChunkList
    {
        public static Dictionary<short, Type> ChunkJumpTable = new Dictionary<short, Type>()
        {
            { 0x0000, typeof(Last)                },
            { 0x0021, typeof(FrameRect)           },
            //0x0022         FrameDemoPath
            { 0x0023, typeof(FrameSeed)           },
            { 0x0025, typeof(FrameLayerEffects)   },
            //0x0026
            { 0x0027, typeof(FrameMoveTimer)      },
            { 0x0028, typeof(FrameEffects)        },
            //0x002A
            //0x002B
            //0x002C
            //0x002D
            //0x002E
            //0x002F
            //0x0030
            //0x0031         FrameInclude
        };
    }
}
