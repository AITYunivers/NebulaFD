using SapphireD.Core.Data.Chunks;
using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Fonts;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.BankChunks.Sounds;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.FileReaders;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data
{
    public abstract class PackageData
    {
        public PackData PackData = new();
        public string ModulesDir = string.Empty;

        public string Header = string.Empty;
        public short RuntimeVersion;
        public short RuntimeSubversion;
        public int ProductVersion;
        public int ProductBuild;

        public AppHeader AppHeader = new();                 // 0x2223
        public string AppName = string.Empty;               // 0x2224
        public string Author = string.Empty;                // 0x2225
        public MenuBar MenuBar = new();                     // 0x2226
        public string ExtensionsPath = string.Empty;        // 0x2227
        public FrameItems FrameItems = new();               // 0x2229 & 0x223F
        public string HelpFile = string.Empty;              // 0x2230
        public short[] FrameHandles = new short[0];         // 0x222B
        public ExtensionData ExtensionData = new();         // 0x222C
        public string EditorFilename = string.Empty;        // 0x222E
        public string TargetFilename = string.Empty;        // 0x222F
        public TransitionFile TransitionFile = new();       // 0x2231
        public GlobalValues GlobalValues = new();           // 0x2232
        public GlobalStrings GlobalStrings = new();         // 0x2233
        public Extensions Extensions = new();               // 0x2234
        public AppIcon AppIcon = new();                     // 0x2235
        public byte[] SerialNumber = new byte[0];           // 0x2237
        public BinaryFiles BinaryFiles = new();             // 0x2238
        public string About = string.Empty;                 // 0x223A
        public string Copyright = string.Empty;             // 0x223B
        public GlobalValueNames GlobalValueNames = new();   // 0x223C
        public GlobalStringNames GlobalStringNames = new(); // 0x223D
        public bool ExeOnly;                                // 0x2240
        public byte[] Protection = new byte[0];             // 0x2242
        public ExtendedHeader ExtendedHeader = new();       // 0x2245
        public int AppCodePage;                             // 0x2246
        public Dictionary<int, Frame> Frames = new();       // 0x3333
        public ObjectAnimations ObjectAnimations = new();   // 0x4449
        public AnimationOffsets AnimationOffsets = new();   // 0x444A

        public ImageOffsets ImageOffsets = new();           // 0x5555
        public FontOffsets FontOffsets = new();             // 0x5556
        public SoundOffsets SoundOffsets = new();           // 0x5557
        public MusicOffsets MusicOffsets = new();           // 0x5558
        public ImageBank ImageBank = new();                 // 0x6666
        public FontBank FontBank = new();                   // 0x6667
        public SoundBank SoundBank = new();                 // 0x6668

        public abstract void Read(ByteReader reader);
        public abstract void CliUpdate();
    }
}
