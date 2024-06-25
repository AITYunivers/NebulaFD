using Nebula;
using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.MFAChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using Size = System.Drawing.Size;
using ZelTranslator_SD.Parsers;
using Spectre.Console;


// MAKE SURE TO DISABLE THESE BEFORE PUSHING
namespace Nebula.Tools.ZelTranslator_SD
{
    public class GameMakerStudio2 //: INebulaTool // Comment ': INebulaTool' out to disable
    {
        public string Name => "GameMaker Studio 2 Translator";
        public void Execute()
        {
            GMS2Writer.Write(NebulaCore.PackageData);
        }
    }
    public class GameMakerStudio1 //: INebulaTool // Comment ': INebulaTool' out to disable
    {
        public string Name => "GMS1 Translator";
        public void Execute()
        {
            GMS1Writer.Write(NebulaCore.PackageData);
        }
    }
}