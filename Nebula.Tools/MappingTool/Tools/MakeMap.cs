using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Utilities;
using Nebula.Tools.MappingTool.Structure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Spectre.Console;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Nebula.Tools.MappingTool.Structure.MapStructure;
using About = Nebula.Tools.MappingTool.Structure.MapStructure.About;
using Frame = Nebula.Tools.MappingTool.Structure.MapStructure.Frame;
using NFrame = Nebula.Core.Data.Chunks.FrameChunks.Frame;
using NLayer = Nebula.Core.Data.Chunks.FrameChunks.FrameLayer;
using NTransition = Nebula.Core.Data.Chunks.ChunkTypes.TransitionChunk;
using NShader = Nebula.Core.Data.Chunks.BankChunks.Shaders.Shader;
using NShaderParameter = Nebula.Core.Data.Chunks.BankChunks.Shaders.ShaderParameter;

namespace Nebula.Tools.MappingTool.Tools
{
    // I'll probably have every map have to be created manually
    // I lied.

    public class MakeMap
    {
        public static void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            PackageData data = NebulaCore.PackageData;
            Project proj = new();
            MakeSettings(proj.Settings, data);
            MakeWindow(proj.Window, data);
            MakeRuntimeOptions(proj.RuntimeOptions, data);
            MakeValues(proj.Values, data);
            MakeEvents(proj.Events, data);
            MakeAbout(proj.About, data);
            MakeWindows(proj.Windows, data);
            MakeFrames(proj, data);

            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Mapping.nmf";
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonConvert.SerializeObject(proj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public static void MakeSettings(Settings proj, PackageData data)
        {
            proj.GraphicMode = (GraphicMode)data.AppHeader.GraphicMode;
            proj.BuildType = (BuildType)data.ExtendedHeader.BuildType;
            proj.BuildFilename = data.TargetFilename;
            proj.CompressionLevel = data.ExtendedHeader.CompressionFlags["CompressionLevelMax"] ? CompressionLevel.Maximum : CompressionLevel.Normal;
            proj.CompressSounds = data.ExtendedHeader.CompressionFlags["CompressSounds"];
            proj.EnableDebuggerShortcuts = data.AppHeader.OtherFlags["DebuggerShortcuts"];
            proj.ShowDebugger = data.AppHeader.OtherFlags["ShowDebugger"];
            proj.OptimizeEvents = FrameEvents.OptimizedEvents;
            proj.OptimizeImageSizeInRAM = data.ExtendedHeader.CompressionFlags["OptimizeImageSize"];
            proj.OptimizePlaySample = data.ExtendedHeader.Flags["OptimizePlaySample"];
        }

        public static void MakeWindow(Window proj, PackageData data)
        {
            proj.Width = data.AppHeader.AppWidth;
            proj.Height = data.AppHeader.AppHeight;
            proj.BorderColor = new int[3]
            { 
                data.AppHeader.BorderColor.R, 
                data.AppHeader.BorderColor.G, 
                data.AppHeader.BorderColor.B
            };
            proj.Heading = !data.AppHeader.Flags["HeadingDisabled"];
            proj.HeadingWhenMaximized = data.AppHeader.Flags["HeadingWhenMaximized"];
            proj.RightToLeftReading = data.ExtendedHeader.Flags["RightToLeftReading"];
            proj.RightToLeftLayout = data.ExtendedHeader.Flags["RightToLeftLayout"];
            proj.DisableCloseButton = data.AppHeader.NewFlags["DisableCloseButton"];
            proj.NoMinimizeBox = data.AppHeader.NewFlags["NoMinimizeBox"];
            proj.NoMaximizeBox = data.AppHeader.NewFlags["NoMaximizeBox"];
            proj.NoThickFrame = data.AppHeader.NewFlags["NoThickFrame"];
            proj.MaximizedOnBootup = data.AppHeader.Flags["MaximizedOnBoot"];
            proj.HiddenAtStart = data.AppHeader.NewFlags["HiddenAtStart"];
            proj.ShowMenuBar = data.AppHeader.Flags["MenuBar"];
            MakeMenuBar(proj.MenuBar, data);
            proj.MenuDisplayedOnBootup = !data.AppHeader.Flags["MenuDisplayedDisabled"];
            proj.ChangeResolutionMode = data.AppHeader.Flags["ChangeResolutionMode"];
            proj.AllowUserToSwitchToFromFullScreen = data.AppHeader.Flags["AllowFullscreenSwitch"];
            proj.KeepScreenRatio = data.ExtendedHeader.Flags["KeepScreenRatio"];
            proj.ScreenRatioTolerance = data.ExtendedHeader.ScreenRatio;
            proj.ResizeDisplayToFillWindowSize = data.AppHeader.Flags["ResizeDisplay"];
            proj.FitInsideBlackBars = data.AppHeader.Flags["FitInside"];
            proj.AntiAliasingWhenResizing = data.ExtendedHeader.Flags["AntiAliasing"];
            proj.DoNotCenterFrameAreaInWindow = data.AppHeader.NewFlags["DontCenterFrame"];
        }

        public static void MakeMenuBar(MapStructure.MenuBar proj, PackageData data)
        {
            // Might never implement, I hate the MenuBar
            proj.MenuItems = new();
        }

        public static void MakeRuntimeOptions(RuntimeOptions proj, PackageData data)
        {
            proj.FrameRate = data.AppHeader.FrameRate;
            proj.MachineIndependentSpeed = data.AppHeader.Flags["MachineIndependentSpeed"];
            proj.RunWhenMinimized = data.AppHeader.NewFlags["RunWhenMinimized"];
            proj.RunWhileResizing = data.AppHeader.NewFlags["RunWhileResizing"];
            proj.DoNotStopScreenSaverWhenInputEvent = data.AppHeader.NewFlags["DontStopScreenSaver"];
            proj.DoNotShareDataIfRunAsSubApplication = data.AppHeader.OtherFlags["DontShareSubAppData"];
            proj.DisplayMode = DisplayMode.Standard;
            if (data.AppHeader.OtherFlags["Direct3D9or11"] &&
                data.AppHeader.OtherFlags["Direct3D8or11"])
                proj.DisplayMode = DisplayMode.Direct3D11;
            else if (data.AppHeader.OtherFlags["Direct3D9or11"])
                proj.DisplayMode = DisplayMode.Direct3D9;
            else if (data.AppHeader.OtherFlags["Direct3D8or11"])
                proj.DisplayMode = DisplayMode.Direct3D8;
            proj.VSync = data.AppHeader.NewFlags["VSync"];
            //proj.NumberOfBackBuffers = 2;
            proj.PremultipliedAlpha = data.ExtendedHeader.Flags["PremultipliedAlpha"];
            //proj.OptimizeStringObjects = true;
            proj.MultiSamples = data.AppHeader.Flags["MultiSamples"];
            proj.PlaySoundsOverFrames = data.AppHeader.NewFlags["PlaySoundsOverFrames"];
            proj.DoNotMuteSamplesOnLoseFocus = data.AppHeader.NewFlags["DontMuteOnLostFocus"];
            //proj.Input = CharacterInputEncoding.ANSI;
            //proj.Output = CharacterOutputEncoding.Unicode;
            proj.InitialScore = (data.AppHeader.InitScore + 1) * -1;
            proj.InitialLives = (data.AppHeader.InitLives + 1) * -1;
            MakeDefaultControls(proj.DefaultControls, data);
            proj.IgnoreDestroyIfTooFar = data.ExtendedHeader.Flags["DontIgnoreDestroyFar"];
            //proj.WindowsLikeCollision = false;
        }

        public static void MakeDefaultControls(DefaultControls proj, PackageData data)
        {
            //proj.Player1 = data.AppHeader.ControlKeys[0];
            //proj.Player2 = data.AppHeader.ControlKeys[1];
            //proj.Player3 = data.AppHeader.ControlKeys[2];
            //proj.Player4 = data.AppHeader.ControlKeys[3];
        }

        public static void MakeValues(Values proj, PackageData data)
        {
            proj.GlobalValues = new GlobalValue[data.GlobalValues.Values.Length];
            for (int i = 0; i < proj.GlobalValues.Length; i++)
            {
                proj.GlobalValues[i] = new GlobalValue();
                proj.GlobalValues[i].Value = data.GlobalValues.Values[i];
                if (data.GlobalValueNames.Names.Length > 0)
                    proj.GlobalValues[i].Name = data.GlobalValueNames.Names[i];
            } 

            proj.GlobalStrings = new GlobalString[data.GlobalStrings.Strings.Length];
            for (int i = 0; i < proj.GlobalStrings.Length; i++)
            {
                proj.GlobalStrings[i] = new GlobalString();
                proj.GlobalStrings[i].Value = data.GlobalStrings.Strings[i];
                if (data.GlobalStringNames.Names.Length > 0)
                    proj.GlobalStrings[i].Name = data.GlobalStringNames.Names[i];
            }    
        }

        public static void MakeEvents(Events proj, PackageData data)
        {
            //proj.AllowGlobalEventsWithGhostObjects = true;
            //proj.BaseFrame = 1;
            //proj.EventOrder = EventOrder.FrameGlobalBehaviors;
            //proj.Qualifiers = null;
            //proj.AllowAlterablesForCounterString = false;
        }

        public static void MakeAbout(About proj, PackageData data)
        {
            proj.Name = data.AppName;
            //proj.Icons = new int[11] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            proj.Filename = data.EditorFilename;
            proj.HelpFile = data.HelpFile;
            //proj.Language = Language.EnglishUnitedStates;
            proj.Author = data.Author;
            proj.Copyright = data.Copyright;
            proj.AboutBox = data.About;
        }

        public static void MakeWindows(Windows proj, PackageData data)
        {
            MakeImageFilters(proj.ImageFilters, data);
            MakeSoundFilters(proj.SoundFilters, data);
            //proj.DPIAware = false;
            proj.EnableVisualThemes = data.AppHeader.NewFlags["EnableVisualThemes"];
            proj.DisableIME = data.ExtendedHeader.Flags["DisableIME"];
            proj.ReduceCPUUsage = data.ExtendedHeader.Flags["ReduceCPUUsage"];
            proj.UnpackedEXE = !string.IsNullOrEmpty(data.ModulesDir);
            proj.ModulesSubDirectory = data.ModulesDir;
            proj.IncludeExternalFiles = data.AppHeader.OtherFlags["IncludeExternalFiles"];
            MakeInstallSettings(proj.InstallSettings, data);
            //proj.ExecutionLevel = ExecutionLevel.AsInvoker;
        }

        public static void MakeImageFilters(ImageFilters proj, PackageData data)
        {
            proj.Automatic = true;
            proj.AutodeskFLIC = true;
            proj.CompuserveBitmap = true;
            proj.JPEG = true;
            proj.PaintBrush = true;
            proj.PortableNetworkGraphics = true;
            proj.TargaBitmap = true;
            proj.VideoForWindows = true;
            proj.WindowsBitmap = true;
        }

        public static void MakeSoundFilters(SoundFilters proj, PackageData data)
        {
            proj.Automatic = true;
            proj.AIFF = true;
            proj.MOD = true;
            proj.MP3 = true;
            proj.OggVorbis = true;
            proj.WAVE = true;
        }

        public static void MakeInstallSettings(InstallSettings proj, PackageData data)
        {
            // Wahh
        }

        public static void MakeFrames(Project proj, PackageData data)
        {
            proj.Frames = new Frame[data.Frames.Count];
            for (int i = 0; i < proj.Frames.Length; i++)
            {
                proj.Frames[i] = new Frame();
                MakeFrame(proj.Frames[i], data.Frames[i]);
            }
        }

        public static void MakeFrame(Frame proj, NFrame data)
        {
            MakeFrameSettings(proj.Settings, data);
            MakeFrameRuntimeOptions(proj.RuntimeOptions, data);
            MakeFrameAbout(proj.About, data);
            MakeFrameLayers(proj, data);
            proj.Instances = new ObjectInstance[0];
        }

        public static void MakeFrameSettings(FrameSettings proj, NFrame data)
        {
            proj.Width = data.FrameHeader.Width;
            proj.Height = data.FrameHeader.Height;
            proj.VirtualWidth = data.FrameRect.Right;
            proj.VirtualHeight = data.FrameRect.Bottom;
            proj.BackgroundColor = new int[3] 
            { 
                data.FrameHeader.Background.R,
                data.FrameHeader.Background.G,
                data.FrameHeader.Background.B
            };
            proj.DontIncludeAtBuildTime = data.FrameHeader.FrameFlags["DontInclude"];
            MakeTransition(proj.FadeIn, data.FrameTransitionIn);
            MakeTransition(proj.FadeOut, data.FrameTransitionOut);
            MakeFrameEffect(proj.Effect, data);
            proj.BlendCoefficient = data.FrameEffects.BlendCoeff;
            proj.RGBCoefficient = new int[3]
            {
                data.FrameEffects.RGBCoeff.R,
                data.FrameEffects.RGBCoeff.G,
                data.FrameEffects.RGBCoeff.B
            };
            //proj.DemoFile = string.Empty;
            proj.RNGSeed = data.FrameSeed;
            //proj.IncludeAnotherFrame = -1;
        }

        public static void MakeTransition(Transition proj, NTransition data)
        {
            proj.FileName = data.FileName;
            proj.ModuleName = data.ModuleName;
            proj.Module = data.Module;
            proj.ModuleID = data.ID;
            proj.Duration = data.Duration;
            proj.UseColor = data.UseColor;
            proj.Color = new int[3]
            {
                data.Color.R,
                data.Color.G,
                data.Color.B
            };
        }

        public static void MakeFrameEffect(Effect proj, NFrame data)
        {
            proj.InkEffect = data.FrameEffects.InkEffect;
            proj.InkEffectParams = data.FrameEffects.InkEffectParam;
            proj.Shader = null;
            if (data.FrameEffects.Shader.Handle >= 0)
            {
                proj.Shader = new Shader();
                MakeShader(proj.Shader, data.FrameEffects.Shader);
            }
        }

        public static void MakeShader(Shader proj, NShader data)
        {
            proj.Handle = data.Handle;
            proj.Parameters = new ShaderParameter[data.Parameters.Length];
            for (int i = 0; i < proj.Parameters.Length; i++)
            {
                proj.Parameters[i] = new ShaderParameter();
                MakeParameter(proj.Parameters[i], data.Parameters[i]);
            }
        }

        public static void MakeParameter(ShaderParameter proj, NShaderParameter data)
        {
            proj.Name = data.Name;
            proj.Type = data.Type;
            proj.Value = data.Value;
            proj.FloatValue = data.FloatValue;
        }

        public static void MakeFrameRuntimeOptions(FrameRuntimeOptions proj, NFrame data)
        {
            proj.GrabDesktopAtStart = data.FrameHeader.FrameFlags["GrabDesktop"];
            proj.KeepDisplayFromPreviousFrame = data.FrameHeader.FrameFlags["KeepDisplay"];
            proj.HandleBackgroundCollisionsEvenOutOfWindow = data.FrameHeader.FrameFlags["HandleBgCollisions"];
            proj.DisplayFrameTitleInWindowCaption = data.FrameHeader.FrameFlags["DisplayTitle"];
            proj.ResizeToScreenSizeAtStart = data.FrameHeader.FrameFlags["ResizeToScreen"];
            proj.Direct3DDontEraseBackground = data.FrameHeader.FrameFlags["DontEraseBackground"];
            proj.ForceLoadOnCall = ForceLoadOnCall.No;
            if (data.FrameHeader.FrameFlags["ForceLoadOnCall"])
                proj.ForceLoadOnCall = ForceLoadOnCall.YesForce;
            if (data.FrameHeader.FrameFlags["ForceLoadOnCallIgnore"])
                proj.ForceLoadOnCall = ForceLoadOnCall.YesIgnore;
            //proj.ScreenSaverSetupFrame = false;
            //proj.IncludeGlobalEvents = true;
            proj.NumberOfObjects = data.FrameEvents.MaxObjects;
            proj.TimerBasedMovements = data.FrameHeader.FrameFlags["TimerBasedMovements"];
            proj.MovementTimerBase = data.FrameMoveTimer;
            proj.Password = data.FramePassword;
        }

        public static void MakeFrameAbout(FrameAbout proj, NFrame data)
        {
            proj.Name = data.FrameName;
        }

        public static void MakeFrameLayers(Frame proj, NFrame data)
        {
            proj.Layers = new Layer[data.FrameLayers.Layers.Length];
            for (int i = 0; i < proj.Layers.Length; i++)
            {
                proj.Layers[i] = new Layer();
                MakeFrameLayer(proj.Layers[i], data.FrameLayers.Layers[i]);
            }
        }

        public static void MakeFrameLayer(Layer proj, NLayer data)
        {
            MakeFrameLayerSettings(proj.Settings, data);
            proj.VisibleInEditor = data.MFALayerFlags["Visible"];
            proj.LockedInEditor = data.MFALayerFlags["Locked"];
        }

        public static void MakeFrameLayerSettings(LayerSettings proj, NLayer data)
        {
            proj.Name = data.Name;
            proj.VisibleAtStart = !data.LayerFlags["HiddenAtStart"];
            proj.SaveBackground = !data.LayerFlags["DontSaveBackground"];
            proj.XCoefficient = data.XCoefficient;
            proj.YCoefficient = data.YCoefficient;
            proj.WrapHorizontally = data.LayerFlags["WrapHorizontally"];
            proj.WrapVertically = data.LayerFlags["WrapVertically"];
            proj.SameEffectAsPreviousLayer = data.LayerFlags["PrevEffect"];
            MakeFrameLayerEffect(proj.Effect, data);
            proj.BlendCoefficient = data.Effect.BlendCoeff;
            proj.RGBCoefficient = new int[3]
            {
                data.Effect.RGBCoeff.R,
                data.Effect.RGBCoeff.G,
                data.Effect.RGBCoeff.B
            };
        }

        public static void MakeFrameLayerEffect(Effect proj, NLayer data)
        {
            proj.InkEffect = data.Effect.InkEffect;
            proj.InkEffectParams = data.Effect.InkEffectParam;
            proj.Shader = null;
            if (data.Effect.Shader.Handle >= 0)
            {
                proj.Shader = new Shader();
                MakeShader(proj.Shader, data.Effect.Shader);
            }
        }
    }
}