using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Fonts;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.BankChunks.Sounds;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.StringChunks;

namespace SapphireD.Core.Data.Chunks
{
    public class ChunkList
    {
        public static Dictionary<short, Type> ChunkJumpTable = new Dictionary<short, Type>()
        {
            { 0x2223, typeof(AppHeader) },
            { 0x2224, typeof(AppName) },
            //0x2225
            //0x2226
            { 0x2229, typeof(FrameItems) },
            { 0x222B, typeof(FrameHandles) },
            //0x222C
            { 0x222E, typeof(EditorFilename) },
            { 0x222F, typeof(TargetFilename) },
            //0x2231
            { 0x2234, typeof(Extensions) },
            //0x2235
            //0x2237
            { 0x223B, typeof(Copyright) },
            { 0x223F, typeof(FrameItems) },
            //0x2240
            //0x2242
            { 0x2245, typeof(AppHeader2) },
            { 0x2246, typeof(AppCodePage) },
            //0x224D
            //0x224F
            { 0x3333, typeof(Frame) },
            { 0x3334, typeof(FrameHeader) },
            { 0x3335, typeof(FrameName) },
            //0x3337
            //0x3338
            //0x333B
            //0x333C
            //0x333D
            { 0x3341, typeof(FrameLayers) },
            { 0x3342, typeof(FrameRect) },
            //0x3345
            { 0x3347, typeof(FrameMoveTimer) },
            { 0x3349, typeof(FrameEffects) },
            { 0x4444, typeof(ObjectInfoHeader) },
            //0x4445
            //0x4446
            { 0x5555, typeof(ImageOffsets) },
            { 0x5556, typeof(FontOffsets) },
            { 0x5557, typeof(SoundOffsets) },
            { 0x5558, typeof(MusicOffsets) },
            { 0x6666, typeof(ImageBank) },
            { 0x6667, typeof(FontBank) },
            { 0x6668, typeof(SoundBank) },
            { 0x7F7F, typeof(Last) }
        };
    }
}
