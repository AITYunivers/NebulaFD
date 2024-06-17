using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nebula.Tools.MappingTool.Structure
{
    public class MapStructure
    {
        public class Project
        {
            public Settings Settings = new Settings();
            public Window Window = new Window();
            public RuntimeOptions RuntimeOptions = new RuntimeOptions();
            public Values Values = new Values();
            public Events Events = new Events();
            public About About = new About();
            public Windows Windows = new Windows();
            public Frame[] Frames = new Frame[0];
            public ObjectInfo[] ObjectInfos = new ObjectInfo[0];
        }

        public class Settings
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public GraphicMode GraphicMode = GraphicMode.C16Mil;
            [JsonConverter(typeof(StringEnumConverter))]
            public BuildType BuildType = BuildType.WindowsEXE;
            public string BuildFilename = string.Empty;
            public bool ShowDeprecatedBuildTypes = false;
            [JsonConverter(typeof(StringEnumConverter))]
            public CompressionLevel CompressionLevel = CompressionLevel.Normal;
            public bool CompressSounds = false;
            public bool DisplayBuildWarningMessages = true;
            public string CommandLine = string.Empty;
            public bool EnableDebuggerShortcuts = true;
            public bool ShowDebugger = true;
            public bool CrashDisplayLastEvent = true;
            public bool EnableProfiler = false;
            public bool StartProfilingAtStartOfFrame = true;
            public bool ProfileTopLevelConditionsOnly = false;
            public bool RecordSlowestAppLoops = false;
            public bool OptimizeEvents = true;
            public bool OptimizeImageSizeInRAM = true;
            public bool OptimizePlaySample = true;
            public bool MergePlayAndSetSampleActions = false;
            public bool BuildCache = false;
        }

        public class Window
        {
            public int Width = 640;
            public int Height = 480;
            public int[] BorderColor = new int[3] { 255, 255, 255 };
            public bool Heading = true;
            public bool HeadingWhenMaximized = true;
            public bool RightToLeftReading = false;
            public bool RightToLeftLayout = false;
            public bool DisableCloseButton = false;
            public bool NoMinimizeBox = false;
            public bool NoMaximizeBox = false;
            public bool NoThickFrame = false;
            public bool MaximizedOnBootup = false;
            public bool HiddenAtStart = false;
            public bool ShowMenuBar = true;
            public MenuBar MenuBar = new MenuBar();
            public bool MenuDisplayedOnBootup = true;
            public bool ChangeResolutionMode = false;
            public bool AllowUserToSwitchToFromFullScreen = false;
            public bool KeepScreenRatio = false;
            public int ScreenRatioTolerance = 8;
            public bool ResizeDisplayToFillWindowSize = false;
            public bool FitInsideBlackBars = false;
            public bool AntiAliasingWhenResizing = false;
            public bool DoNotCenterFrameAreaInWindow = false;
        }

        public class MenuBar
        {
            public Dictionary<string, string[]> MenuItems = new Dictionary<string, string[]>()
            {
                {
                    "&File",
                    new string[7]
                    {
                        "&New F2",
                        "-",
                        "Pass&word",
                        "&Pause Ctrl+P",
                        "Pla&yers Ctrl+Y",
                        "-",
                        "&Quit Alt+F4"
                    }
                },
                {
                    "&Options",
                    new string[6]
                    {
                        "Play &samples Ctrl+S",
                        "Play &musics Ctrl+M",
                        "-",
                        "&Hide the menu F8",
                        "-",
                        "&Full Screen Alt+Enter"
                    }
                },
                {
                    "&Help",
                    new string[3]
                    {
                        "&Contents F1",
                        "-",
                        "&About"
                    }
                }
            };
            public bool Colors = false;
            public int[] BackgroundColor = new int[3] { 255, 255, 255 };
            public int[] BarBackgroundColor = new int[3] { 255, 255, 255 };
            public int[] CheckboxColor = new int[3] { 0, 0, 0 };
            public int[] GrayedTextColor = new int[3] { 109, 109, 109 };
            public int[] SelectedItemBackgroundColor = new int[3] { 0, 120, 215 };
            public int[] SelectedItemTextColor = new int[3] { 255, 255, 255 };
            public int[] SeparatorColor = new int[3] { 160, 160, 160 };
            public int[] TextColor = new int[3] { 0, 0, 0 };
        }

        public class RuntimeOptions
        {
            public int FrameRate = 60;
            public bool MachineIndependentSpeed = false;
            public bool RunWhenMinimized = false;
            public bool RunWhileResizing = false;
            public bool DoNotStopScreenSaverWhenInputEvent = false;
            public bool DoNotShareDataIfRunAsSubApplication = false;
            [JsonConverter(typeof(StringEnumConverter))]
            public DisplayMode DisplayMode = DisplayMode.Direct3D11;
            public bool VSync = false;
            public int NumberOfBackBuffers = 2;
            public bool PremultipliedAlpha = true;
            public bool OptimizeStringObjects = true;
            public bool MultiSamples = true;
            public bool PlaySoundsOverFrames = false;
            public bool DoNotMuteSamplesOnLoseFocus = false;
            [JsonConverter(typeof(StringEnumConverter))]
            public CharacterInputEncoding Input = CharacterInputEncoding.ANSI;
            [JsonConverter(typeof(StringEnumConverter))]
            public CharacterOutputEncoding Output = CharacterOutputEncoding.Unicode;
            public int InitialScore = 0;
            public int InitialLives = 3;
            public DefaultControls DefaultControls = new DefaultControls();
            public bool IgnoreDestroyIfTooFar = false;
            public bool WindowsLikeCollision = false;
        }

        public class DefaultControls
        {
            public int[] Player1 = new int[9] { 5, 38, 40, 37, 39, 16, 17, 32, 13 };
            public int[] Player2 = new int[9] { 5, 38, 40, 37, 39, 16, 17, 32, 13 };
            public int[] Player3 = new int[9] { 5, 38, 40, 37, 39, 16, 17, 32, 13 };
            public int[] Player4 = new int[9] { 5, 38, 40, 37, 39, 16, 17, 32, 13 };
        }

        public class Values
        {
            public GlobalValue[] GlobalValues = new GlobalValue[0];
            public GlobalString[] GlobalStrings = new GlobalString[0];
        }

        public class GlobalValue
        {
            public string Name = "Global Value A";
            public int Value = 0;
        }

        public class GlobalString
        {
            public string Name = "Global String A";
            public string Value = string.Empty;
        }

        public class Events
        {
            public bool AllowGlobalEventsWithGhostObjects = true;
            public int BaseFrame = 1;
            [JsonConverter(typeof(StringEnumConverter))]
            public EventOrder EventOrder = EventOrder.FrameGlobalBehaviors;
            public string[] Qualifiers = new string[100]
            {
                "Player",
                "Good",
                "Neutral",
                "Bad",
                "Enemies",
                "Friends",
                "Bullets",
                "Arms",
                "Bonus",
                "Collectables",
                "Traps",
                "Doors",
                "Keys",
                "Texts",
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "Parents",
                "Children",
                "Data",
                "Timed",
                "Engine",
                "Areas",
                "Reference Points",
                "Radar Enemies",
                "Radar Friends",
                "Radar Neutrals",
                "Music",
                "Sound",
                "Waveform",
                "Background Scenary",
                "Foreground Scenary",
                "Decorations",
                "Water",
                "Clouds",
                "Empty",
                "Fog",
                "Flowers",
                "Animals",
                "Bosses",
                "NPC",
                "Vehicles",
                "Rockets",
                "Balls",
                "Bombs",
                "Explosions",
                "Particles",
                "Clothes",
                "Glow",
                "Arrows",
                "Buttons",
                "Cursors",
                "Drawing Tools",
                "Indicator",
                "Shapes",
                "Shields",
                "Shifting Blocks",
                "Magnets",
                "Negative Matter",
                "Neutral Matter",
                "Positive Matter",
                "Breakable",
                "Dissolving",
                "Dialogue",
                "HUD",
                "Inventory",
                "Inventory Item",
                "Interface",
                "Movable",
                "Perspective",
                "Calculation Objects",
                "Invisible",
                "Masks",
                "Obstacles",
                "Value Holder",
                "Helpful",
                "Powerups",
                "Targets",
                "Trapdoors",
                "Dangers",
                "Forbidden",
                "Physical Objects",
                "3D Objects",
                "Generic 1",
                "Generic 2",
                "Generic 3",
                "Generic 4",
                "Generic 5",
                "Generic 6",
                "Generic 7",
                "Generic 8",
                "Generic 9",
                "Generic 10"
            };
            public bool AllowAlterablesForCounterString = false;
        }

        public class About
        {
            public string Name = "Application 1";
            public int[] Icons = new int[11] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            public string Filename = string.Empty;
            public string HelpFile = string.Empty;
            [JsonConverter(typeof(StringEnumConverter))]
            public Language Language = Language.EnglishUnitedStates;
            public string Author = string.Empty;
            public string Copyright = string.Empty;
            public string AboutBox = string.Empty;
        }

        public class Windows
        {
            public ImageFilters ImageFilters = new ImageFilters();
            public SoundFilters SoundFilters = new SoundFilters();
            public bool DPIAware = false;
            public bool EnableVisualThemes = true;
            public bool DisableIME = false;
            public bool ReduceCPUUsage = false;
            public bool UnpackedEXE = false;
            public string ModulesSubDirectory = "Modules";
            public bool IncludeExternalFiles = false;
            public InstallSettings InstallSettings = new InstallSettings();
            [JsonConverter(typeof(StringEnumConverter))]
            public ExecutionLevel ExecutionLevel = ExecutionLevel.AsInvoker;
        }

        public class ImageFilters
        {
            public bool Automatic = true;
            public bool AutodeskFLIC = true;
            public bool CompuserveBitmap = true;
            public bool JPEG = true;
            public bool PaintBrush = true;
            public bool PortableNetworkGraphics = true;
            public bool TargaBitmap = true;
            public bool VideoForWindows = true;
            public bool WindowsBitmap = true;
        }

        public class SoundFilters
        {
            public bool Automatic = true;
            public bool AIFF = true;
            public bool MOD = true;
            public bool MP3 = true;
            public bool OggVorbis = true;
            public bool WAVE = true;
        }

        public class InstallSettings
        {
            public string ProductTitle = "Application 1";
            public string DefaultInstallDirectory = "#Program Files#\\#Title";
            public int Font = 0;
            public InstallerFile[] Files = new InstallerFile[0];
            public InstallerWizardTexts WizardTexts = new InstallerWizardTexts();
            public Uninstaller Uninstaller = new Uninstaller();
        }

        public class InstallerWizardTexts
        {
            public string[] Presentation = new string[6]
            {
                "Welcome",
                "<Empty>",
                "<b><fontsize=12>Welcome to the #title Install program</font></b>.",
                "This program allows you to install #title on your hard drive.",
                "It is strongly recommended that before proceeding, you ensure that no other Windows programs are running.",
                "If you do not wish to install #title, click 'Exit' now, otherwise click 'Next' to continue."
            };
            public string[] Information = new string[4]
            {
                "Information",
                "Please read the information below.",
                "",
                "<Empty>"
            };
            public string[] License = new string[6]
            {
                "License",
                "Please read the license agreement below.",
                "Please read the license agreement below and select \"I Agree\" if you agree with its terms and conditions.",
                "",
                "I agree with the above terms and conditions",
                "I do not agree"
            };
            public string[] Directory = new string[9]
            {
                "Directory",
                "Choose an installation folder and click Next to continue.",
                "#Title's files will be installed in the following directory:",
                "<Empty>",
                "<Empty>",
                "Click 'Next' to continue.",
                "Disk space needed :",
                "Available disk space :",
                "%d Mb"
            };
            public string[] Confirmation = new string[6]
            {
                "Confirmation",
                "You are now ready to install #title.",
                "<Empty>",
                "This program will install #title into %s.",
                "<Empty>",
                "Click 'Start' to install #title."
            };
            public string[] Progress = new string[4]
            {
                "Installing",
                "Installation in progress, please wait.",
                "File :",
                "<Empty>"
            };
            public string[] End = new string[10]
            {
                "End",
                "Installation completed.",
                "#Title has been successfully installed.",
                "<Empty>",
                "<Empty>",
                "#Title has not been totally installed because of the following reason:\n\n%s\n\nYou will have to run this utility again to completely install #title.",
                "View %s",
                "Launch #title",
                "You need to restart Windows so that all installed options can take effect. Click 'Restart' if you want to restart Windows immediately.",
                "<Empty>"
            };
            public string[] MiscellaneousTexts = new string[33]
            {
                "#Title Install Program",
                "Please select a directory",
                "Directory name:",
                "Drives",
                "Checking...",
                "Invalid directory.",
                "The destination directory doesn't exist. Do you want it to be created?",
                "One or more files are write-protected. Do you want to overwrite them anyway?",
                "This program cannot install #title because of the following reason:",
                "The access to the following file is denied:\n%s",
                "The following file is in use:\n\n%s\n\nPlease close it and retry.",
                "There is not enough disk space in the target directory. Do you want to try anyway?",
                "Out of memory!",
                "You are currently in the process of installing components.\nIf you exit now, these components will not be installed correctly.\n\nAre you sure you want to cancel?",
                "Install has not completed. Are you sure you want to exit?",
                "The Install wizard did not complete the installation successfully.",
                "Cannot open the following file:\n%s",
                "Cannot create the following file:\n%s",
                "File corrupt or unreadable:\n%s",
                "Cannot write to the following file:\n%s",
                "Disk full!",
                "CRC error - cannot install the following file:\n%s",
                "The access to the following file is denied:\n%s",
                "successfully updated",
                "successfully removed",
                "successfully installed",
                "cannot remove, denied access",
                "error - cannot replace, denied access",
                "Please insert disk number %d.",
                "<Empty>",
                "%s\n\nThis file exists and is a different language than the file to install.\n\nDo yo want to overwrite the installed version anyway?",
                "This file contains invalid data.",
                "Data error in the following file:\n%s\n\nDo you want to continue anyway?"
            };
            public string[] Buttons = new string[7]
            {
                "< &Back",
                "&Next >",
                "E&xit",
                "&Start",
                "OK",
                "&Cancel",
                "&Restart"
            };
        }

        public class Uninstaller
        {
            public string UninstallerFileName = "Uninstall.exe";
            public string StartMenuFolder = "#Title";
            public string StartMenuShortcutName = "Uninstall #Title";
            public string[] Texts = new string[15]
            {
                "Uninstall",
                "This program will uninstall #title from your hard drive. Click OK to continue.",
                "#Title has been successfully removed from your hard drive.",
                "&Yes",
                "Yes to &all",
                "&No",
                "No t&o all",
                "Out of memory!",
                "Invalid uninstall info. This program cannot uninstall #title.",
                "The following file is write-protected:\n%s\nDo you want to remove it anyway?",
                "The following system file is apparently no longer used:\n%s\nDo you want to remove it?",
                "Removing files...",
                "Removing icons...",
                "Removing directories...",
                "Removing registry keys..."
            };
            public string[] Registry = new string[0];
        }

        public class InstallerFile
        {
            public string FileName = string.Empty;
            public InstallFileStartMenu StartMenu = new InstallFileStartMenu();
            public InstallFileShortcut Shortcut = new InstallFileShortcut();
            public InstallFileViewRun ViewRun = new InstallFileViewRun();
            public InstallFileWindows Windows = new InstallFileWindows();
            public InstallFileMac Mac = new InstallFileMac();
        }

        public class InstallFileStartMenu
        {
            public string Folder = "#Title";
            public string Name = "#Title";
            public string CommandLineOptions = string.Empty;
            public string IconFile = string.Empty;
        }

        public class InstallFileShortcut
        {
            public string DesktopShortcutName = "#Title";
            public string CommandLineOptions = string.Empty;
            public string IconFile = string.Empty;
        }

        public class InstallFileViewRun
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public View View = View.No;
            [JsonConverter(typeof(StringEnumConverter))]
            public Run Run = Run.No;
            public string CommandLineOptions = string.Empty;
            public bool WaitForEndBeforeContinuing = false;
            public bool ViewRunOnlyAfterReboot = false;
        }

        public class InstallFileWindows
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public InstallDirectory InstallDirectory = InstallDirectory.DestinationDirectory;
            public string OtherDirectory = string.Empty;
            public bool SetAsScreensaver = false;
            public bool RegisterOCXDLLREG = false;
            public bool IncrementDLLUsage = false;
        }

        public class InstallFileMac
        {
            public bool DoNotInstall = false;
            public bool DoNotUninstall = false;
            public bool OnlyCreateDirectory = false;
            public bool DoNotIfFileExists = false;
            public bool DoNotIfBetterVersion = false;
        }

        public class Frame
        {
            public FrameSettings Settings = new FrameSettings();
            public FrameRuntimeOptions RuntimeOptions = new FrameRuntimeOptions();
            public FrameAbout About = new FrameAbout();
            public Layer[] Layers = new Layer[0];
            public ObjectInstance[] Instances = new ObjectInstance[0];
        }

        public class FrameSettings
        {
            public int Width = 640;
            public int Height = 480;
            public int VirtualWidth = 640;
            public int VirtualHeight = 480;
            public int[] BackgroundColor = new int[3] { 255, 255, 255 };
            public bool DontIncludeAtBuildTime = false;
            public Transition FadeIn = new Transition();
            public Transition FadeOut = new Transition();
            public Effect Effect = new Effect();
            public int BlendCoefficient = 0;
            public int[] RGBCoefficient = new int[3] { 255, 255, 255 };
            public string DemoFile = string.Empty;
            public int RNGSeed = -1;
            public int IncludeAnotherFrame = -1;
        }

        public class Transition
        {
            public string FileName = "cctrans.dll";
            public string ModuleName = "None";
            public int Module = 0;
            public string ModuleID = string.Empty;
            public int Duration = 0;
            public bool UseColor = false;
            public int[] Color = new int[3] { 0, 0, 0 };
        }

        public class Effect
        {
            public int InkEffect = 0;
            public uint InkEffectParams = 0;
            public Shader? Shader = null;
        }

        public class FrameRuntimeOptions
        {
            public bool GrabDesktopAtStart = false;
            public bool KeepDisplayFromPreviousFrame = false;
            public bool HandleBackgroundCollisionsEvenOutOfWindow = true;
            public bool DisplayFrameTitleInWindowCaption = false;
            public bool ResizeToScreenSizeAtStart = false;
            public bool Direct3DDontEraseBackground = false;
            [JsonConverter(typeof(StringEnumConverter))]
            public ForceLoadOnCall ForceLoadOnCall = ForceLoadOnCall.No;
            public bool ScreenSaverSetupFrame = false;
            public bool IncludeGlobalEvents = true;
            public int NumberOfObjects = 1000;
            public bool TimerBasedMovements = true;
            public int MovementTimerBase = 60;
            public string Password = string.Empty;
        }

        public class FrameAbout
        {
            public string Name = "Frame 1";
        }

        public class Layer
        {
            public LayerSettings Settings = new LayerSettings();
            public bool VisibleInEditor = true;
            public bool LockedInEditor = false;
        }

        public class LayerSettings
        {
            public string Name = "Untitled";
            public bool VisibleAtStart = true;
            public bool SaveBackground = true;
            public float XCoefficient = 1.0f;
            public float YCoefficient = 1.0f;
            public bool WrapHorizontally = false;
            public bool WrapVertically = false;
            public bool SameEffectAsPreviousLayer = false;
            public Effect Effect = new Effect();
            public int BlendCoefficient = 0;
            public int[] RGBCoefficient = new int[3] { 255, 255, 255 };
        }

        public class Shader
        {
            public int Handle = 0;
            public ShaderParameter[] Parameters = new ShaderParameter[0];
        }

        public class ShaderParameter
        {
            public string Name = string.Empty;
            public int Type = 0;
            public int Value = 0;
            public float FloatValue = 0.0f;
        }

        public class ObjectInstance
        {
            public int Handle = 0;
            public InstancePosition Position = new InstancePosition();
            public InstanceValues Values = new InstanceValues();
            public int Layer = 0;
            public bool Locked = false;
            public bool CreateOnly = false;
        }

        public class InstancePosition
        {
            public int X = 0;
            public int Y = 0;
        }

        public class InstanceValues
        {
            public int InstanceValue = 0;
        }

        public class ObjectInfo
        {
            public ObjectDisplayOptions DisplayOptions = new ObjectDisplayOptions();
            public ObjectSize Size = new ObjectSize();
            public ObjectTextOptions TextOptions = new ObjectTextOptions();
            public ObjectMovement Movements = new ObjectMovement();
            public ObjectRuntimeOptions RuntimeOptions = new ObjectRuntimeOptions();
            public ObjectValues Values = new ObjectValues();
            public ObjectEvents Events = new ObjectEvents();
            public ObjectAbout About = new ObjectAbout();

            public QuickBackdropData? QuickBackdropData = null;
            public BackdropData? BackdropData = null;
            public ActiveData? ActiveData = null;
            public StringData? StringData = null;
            public QNAData? QuestionAndAnswerData = null;
            public ScoreData? ScoreData = null;
            public LivesData? LivesData = null;
            public CounterData? CounterData = null;
            public FormattedTextData? FormattedTextData = null;
            public SubAppData? SubApplicationData = null;
            public ExtensionData? ExtensionData = null;
        }

        public class ObjectDisplayOptions
        {
            public bool VisibleAtStart = true;
            public bool DisplayAsBackground = false;
            public bool SaveBackground = true;
            public bool WipeWithColor = false;
            public int[] WipeColor = new int[3] { 255, 255, 255 };
            public bool Transparent = true;
            public Effect Effect = new Effect();
            public int BlendCoefficient = 0;
            public int[] RGBCoefficient = new int[3] { 255, 255, 255 };
            public bool AntiAliasing = false;
            public Transition FadeIn = new Transition();
            public Transition FadeOut = new Transition();
        }

        public class ObjectSize
        {
            public int Width = 32;
            public int Height = 32;
        }

        public class ObjectTextOptions
        {
            public int Font = 0;
            public bool Bold = false;
            public bool Italic = false;
            public bool Underline = false;
            public bool Strikeout = false;
            public int[] Color = new int[3] { 0, 0, 0 };
            [JsonConverter(typeof(StringEnumConverter))]
            public HorizontalAllignment HorizontalAllignment = HorizontalAllignment.Left;
            [JsonConverter(typeof(StringEnumConverter))]
            public VerticalAllignment VerticalAllignment = VerticalAllignment.Top;
            public bool RightToLeftReading = false;
        }

        public class ObjectMovement
        {
            public int Type = 0;
        }

        public class ObjectRuntimeOptions
        {
            public bool DontIncludeAtBuildTime = false;
            public bool CreateAtStart = true;
            public bool CreateBeforeFrameTransition = false;
            public bool FollowTheFrame = true;
            public bool DestroyObjectIfTooFar = true;
            [JsonConverter(typeof(StringEnumConverter))]
            public InactivateIfTooFar InactivateIfTooFar = InactivateIfTooFar.Automatic;
            public bool UseFineDetection = true;
            [JsonConverter(typeof(StringEnumConverter))]
            public ObstacleType ObstacleType = ObstacleType.None;
            public bool CollisionWithBox = false;
            public bool LoadOnCall = false;
            public bool GlobalObject = false;
            [JsonConverter(typeof(StringEnumConverter))]
            public EditorSynchronization EditorSynchronization = EditorSynchronization.SameNameAndType;
            public bool AutomaticRotations = false;
            public bool DoNotResetFrameDuration = false;
        }

        public class ObjectValues
        {
            public AlterableValue[] AlterableValues = new AlterableValue[0];
            public AlterableString[] AlterableStrings = new AlterableString[0];
            public AlterableFlag[] AlterableFlags = new AlterableFlag[0];
        }

        public class AlterableValue
        {
            public string Name = "Alterable Value A";
            public int Value = 0;
        }

        public class AlterableString
        {
            public string Name = "Alterable String A";
            public string Value = string.Empty;
        }

        public class AlterableFlag
        {
            public string Name = "Flag 0";
            public bool Value = false;
        }

        public class ObjectEvents
        {
            public int[] Qualifiers = new int[8]
            {
                -1, -1, -1, -1, -1, -1, -1, -1
            };
        }

        public class ObjectAbout
        {
            public string Name = "Active";
            public bool AutoUpdate = true;
        }

        public class QuickBackdropData
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public BackdropShape Shape = BackdropShape.Rectangle;
            public int[] BorderColor = new int[3] { 0, 0, 0 };
            public int BorderWidth = 0;

            [JsonConverter(typeof(StringEnumConverter))]
            public BackdropFillType FillType = BackdropFillType.SolidColor;
            public int[] FillColor1 = new int[3] { 128, 128, 128 };
            public int[] FillColor2 = new int[3] { 255, 255, 255 };
            public bool VerticalGradient = false;

            public int Motif = 0;
            public bool IntegralDimensions = false;
        }

        public class BackdropData
        {
            public int Image = 0;
        }

        public class ActiveData
        {
            public Animation[] Animations = new Animation[0];
        }

        public class Animation
        {
            public string Name = "Stopped";
            public Direction[] Directions = new Direction[0];
        }

        public class Direction
        {
            public int Index = 0;
            public int MinimumSpeed = 50;
            public int MaximumSpeed = 50;
            public int Repeat = 1;
            public int RepeatFrame = 0;
            public int[] Frames = new int[0];
        }

        public class StringData
        {
            public string[] Paragraphs = new string[0];
        }

        public class QNAData
        {
            public Question Question = new Question();
            public Answers Answers = new Answers();
        }

        public class Question
        {
            public string Paragraph = "Question";
            public int Font = 0;
            public int[] Color = new int[3] { 0, 0, 0 };
            public bool Relief = false;
        }

        public class Answers
        {
            public Paragraph[] Paragraphs = new Paragraph[0];
            public int Font = 0;
            public int[] Color = new int[3] { 0, 0, 0 };
            public bool Relief = false;
        }

        public class Paragraph
        {
            public string Text = "Answer";
            public bool IsCorrect = false;
        }

        public class ScoreData
        {
            public int Player = 1;
            [JsonConverter(typeof(StringEnumConverter))]
            public ScoreType Type = ScoreType.Numbers;
            public bool UseFixedDigitCount = false;
            public int FixedNumberOfDigits = 9;
            public int[] Images = new int[14];
        }

        public class LivesData
        {
            public int Player = 1;
            [JsonConverter(typeof(StringEnumConverter))]
            public LivesType Type = LivesType.Image;
            public bool UseFixedDigitCount = false;
            public int FixedNumberOfDigits = 2;
            public int[] Images = new int[1];
        }

        public class CounterData
        {
            public int InitialValue = 0;
            public int MinimumValue = -999999999;
            public int MaximumValue = 999999999;

            public bool UseFixedDigitCount = false;
            public int FixedNumberOfDigits = 0;

            public bool UseSignificantDigitCount = false;
            public int NumberOfSignificantDigits = 16;
            public bool UseDecimalDigitCount = false;
            public int NumberOfDigitsAfterDecimalPoint = 2;
            public bool AddZerosToTheLeft = false;

            [JsonConverter(typeof(StringEnumConverter))]
            public CounterType Type = CounterType.Numbers;
            public int[] Images = new int[14];

            [JsonConverter(typeof(StringEnumConverter))]
            public CounterCount Count = CounterCount.UpLeft;
            [JsonConverter(typeof(StringEnumConverter))]
            public CounterFillType FillType = CounterFillType.Gradient;
            public int[] FillColor1 = new int[3] { 128, 128, 128 };
            public int[] FillColor2 = new int[3] { 255, 255, 255 };
            public bool VerticalGradient = false;
        }

        public class FormattedTextData
        {
            public int[] BackgroundColor = new int[3] { 255, 255, 255 };
            public bool AutoVerticalScrollbar = true;
        }

        public class SubAppData
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public SubAppSource Source = SubAppSource.FrameFromThisApplication;
            public string Filename = string.Empty;
            public int FrameNumber = 0;

            public bool ShareGlobalValues = false;
            public bool ShareLives = false;
            public bool ShareScores = false;
            public bool SharePlayerControls = false;

            public int WindowIcon = -1;
            public bool CustomizableSize = false;
            public bool StretchFrameToObjectSize = false;
            public bool DisplayAsSprite = false;

            public bool IgnoreParentsResizeDisplay = false;

            public bool PopupWindow = false;
            public bool ClipSiblings = false;
            public bool Border = false;
            public bool Resizable = false;
            public bool Caption = false;
            public bool ToolCaption = false;
            public bool SystemMenu = false;
            public bool DisableClose = false;
            public bool HiddenOnClose = false;
            public bool Modal = false;
        }

        public class ExtensionData
        {
            public int Type = 0;
            public string Name = string.Empty;
            public string FileName = string.Empty;
            public int Magic = 0;
            public string SubType = string.Empty;
            public int Version = 0;
            public int ID = 0;
            public int Private = 0;
            public int[] Data = new int[0];
        }

        public enum GraphicMode
        {
            C256 = 3,
            C32768 = 6,
            C65536 = 7,
            C16Mil = 4
        }

        public enum BuildType
        {
            WindowsEXE,
            WindowsScreenSaver,
            SubApplication,
            JavaSubApplication,
            JavaApplication,
            JavaInternetApplet,
            JavaWebStart,
            JavaForMobileDevices,
            JavaBluRay,
            JavaMacApplication,
            AdobeFlash,
            JavaForBlackberry,
            AndroidOUYAApplication,
            iOSApplication,
            iOSXCodeProject,
            FinaliOSXCodeProject,
            XNAXboxApp,
            MacApplication,
            XNAWindowsProject,
            XNAXboxProject,
            XNAPhoneProject,
            HTML5Development = 27,
            HTML5FinalProject,
            MacApplicationFile = 30,
            MacXCodeProject,
            UWPProject = 33,
            AndroidAppBundle,
            NintendoSwitch = 74,
            XboxOne,
            Playstation = 78
        }

        public enum CompressionLevel
        {
            Normal,
            Maximum
        }

        public enum DisplayMode
        {
            Standard,
            Direct3D9,
            Direct3D8,
            Direct3D11
        }

        public enum CharacterInputEncoding
        {
            ANSI,
            UTF8
        }

        public enum CharacterOutputEncoding
        {
            ANSI,
            UTF8,
            UTF8WithoutByteOrder,
            Unicode,
            UnicodeBigEndian
        }

        public enum PlayerControl
        {
            Joystick1,
            Joystick2,
            Joystick3,
            Joystick4,
            Keyboard
        }

        public enum EventOrder
        {
            FrameGlobalBehaviors,
            FrameBehaviorsGlobal,
            GlobalFrameBehaviors,
            GlobalBehaviorsFrame,
            BehaviorsFrameGlobal,
            BehaviorsGlobalFrame
        }

        public enum Language
        {
            Afrikaans,
            Albanian,
            ArabicAlgeria,
            ArabicBahrain,
            ArabicEgypt,
            ArabicIrad,
            ArabicJordan,
            ArabicKuwait,
            ArabicLebanon,
            ArabicLibyra,
            ArabicMorocco,
            ArabicOman,
            ArabicQatar,
            ArabicSaudiArabia,
            ArabicSyria,
            ArabicTunisia,
            ArabicUnitedArabEmirates,
            ArabicYemen,
            Armenian,
            AzerbaijaniCryllic,
            AzerbaijaniLatin,
            Basque,
            Belarusian,
            Bulgarian,
            Catalan,
            ChineseSimplifiedChina,
            ChineseSimplifiedSingapore,
            ChineseTraditionalHongKong,
            ChineseTraditionalMacao,
            ChineseTraditionalTaiwan,
            Croatian,
            Czech,
            Danish,
            Divehi,
            DutchBelgium,
            DutchNetherlands,
            EnglishAustralia,
            EnglishBelize,
            EnglishCanada,
            EnglishCaribbean,
            EnglishIreland,
            EnglishJamaica,
            EnglishNewZealand,
            EnglishPhilippines,
            EnglishSouthAfrica,
            EnglishTrinidadAndTobago,
            EnglishUnitedKingdom,
            EnglishUnitedStates,
            EnglishZimbabwe,
            Estonian,
            Faroese,
            Finnish,
            FrenchBelgium,
            FrenchCanada,
            FrenchFrance,
            FrenchLuxembourg,
            FrenchMonaco,
            FrenchSwitzerland,
            Galician,
            Georgian,
            GermanAustria,
            GermanGermany,
            GermanLiechtenstein,
            GermanLuxembourg,
            GermanSwitzerland,
            Greek,
            Gujarati,
            Hebrew,
            Hindi,
            Hungarian,
            Icelandic,
            Indonesian,
            ItalianItaly,
            ItalianSwitzerland,
            Japanese,
            Kannada,
            Kazakh,
            Kiswahili,
            Konkani,
            Korean,
            Kyrgyz,
            LanguageNeutral,
            Latvian,
            Lithuanian,
            Macedonian,
            MelayBrunei,
            MelayMalaysia,
            Marathi,
            Mongolian,
            NorwegianBokmål,
            NorwegianNynorsk,
            Persian,
            Polish,
            PortugueseBrazil,
            PortugueseProtugal,
            Punjabi,
            Romanian,
            Russian,
            Sanskrit,
            SerbianCyrillic,
            SerbianLatin,
            Slovak,
            Slovenian,
            SpanishArgentina,
            SpanishBolivia,
            SpanishChile,
            SpanishColombia,
            SpanishCostaRica,
            SpanishDominicanRepublic,
            SpanishEcuador,
            SpanishGuatemala,
            SpanishHonduras,
            SpanishMexico,
            SpanishNicaragua,
            SpanishPanama,
            SpanishParaguay,
            SpanishPeru,
            SpanishPeurtoRico,
            SpanishSpainInternational,
            SpanishSpainTraditional,
            SpanishUruguay,
            SpanishVenezuela,
            SwedishFinland,
            SwedishSweden,
            Syriac,
            Tamil,
            Tatar,
            Telugu,
            Thai,
            Turkish,
            Ukrainian,
            UzbekCyrillic,
            UzbekLatin,
            Vietnamese
        }

        public enum View
        {
            No,
            BeforeEndPage,
            WhileEndPageIsDisplayed,
            WhenInstallerExits,
            ViewButtonOnEndPage
        }

        public enum Run
        {
            No,
            BeforeEndPage,
            WhileEndPageIsDisplayed,
            WhenInstallerExits,
            LaunchCheckboxOnEndPage
        }

        public enum InstallDirectory
        {
            DestinationDirectory,
            WindowsDirectory,
            SystemDirectory,
            FontsDirectory,
            DesktopDirectory,
            MyDocumentsDirectory,
            ApplicationDataDirectory,
            Other
        }

        public enum ExecutionLevel
        {
            None,
            AsInvoker,
            AsAdministrator
        }

        public enum ForceLoadOnCall
        {
            No,
            YesForce,
            YesIgnore
        }

        public enum HorizontalAllignment
        {
            Left,
            Center,
            Right
        }

        public enum VerticalAllignment
        {
            Top,
            Center,
            Bottom
        }

        public enum InactivateIfTooFar
        {
            No,
            Yes,
            Automatic
        }

        public enum ObstacleType
        {
            None,
            Obstacle,
            Platform,
            Ladder
        }

        public enum EditorSynchronization
        {
            No,
            IndenticalObjects,
            SameNameAndType
        }

        public enum BackdropShape
        {
            Line,
            Rectangle,
            Ellipse
        }

        public enum BackdropFillType
        {
            None,
            SolidColor,
            Gradient,
            Motif
        }

        public enum ScoreType
        {
            Numbers,
            Text
        }

        public enum LivesType
        {
            Image,
            Numbers,
            Text
        }

        public enum CounterType
        {
            Hidden,
            Numbers,
            VerticalBar,
            HorizontalBar,
            Animation,
            Text
        }

        public enum CounterCount
        {
            UpLeft,
            DownRight
        }

        public enum CounterFillType
        {
            SolidColor,
            Gradient
        }

        public enum SubAppSource
        {
            OtherApplication,
            FrameFromThisApplication
        }
    }
}
