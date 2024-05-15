using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;
using Action = Nebula.Core.Data.Chunks.FrameChunks.Events.Action;
using Color = System.Drawing.Color;

namespace Nebula.Plugins.EncKeyDumper
{
    public class KeyDumper : INebulaTool
    {
        public string Name => "Ini Encryption Key Dumper";
        private HashSet<string> Keys = new();

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);
            AnsiConsole.Write(new Markup("[DeepSkyBlue3]Reading Objects[/]"));

            Keys = new();
            GetKeysFromObjects();
            GetKeysFromEvents();

            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);
            AnsiConsole.Write(new Markup($"[DeepSkyBlue2]Found {Keys.Count} Encryption Key{(Keys.Count == 1 ? "" : "s")}:[/]\n"));
            foreach (string key in Keys)
                AnsiConsole.Write(new Markup($"[lightslateblue]{key}[/]\n"));
            Console.ReadKey();
        }

        public void GetKeysFromObjects()
        {
            PackageData data = NebulaCore.PackageData;
            foreach (ObjectInfo oI in data.FrameItems.Items.Values)
            {
                if (oI.Header.Type >= 32 && oI.Properties is ObjectCommon)
                {
                    ObjectCommon props = (ObjectCommon)oI.Properties;
                    string key = GetKeyFromData(data.Extensions.Exts[oI.Header.Type - 32].FileName, props.ObjectExtension.ExtensionData);
                    if (!string.IsNullOrEmpty(key))
                        Keys.Add(key.Trim());
                }
            }
        }

        public string GetKeyFromData(string extName, byte[] data)
        {
            ByteReader reader = new ByteReader(data);
            switch (extName)
            {
                case "INI++15.mfx":
                case "INI++.mfx": // Who tf renames this?? SUNKENSTUDIOS!!!
                    IniPlusPlus inipp = new IniPlusPlus(reader);
                    return inipp.UseEncryption ? inipp.EncryptionKey : "";
            }
            return "";
        }

        public void GetKeysFromEvents()
        {
            PackageData data = NebulaCore.PackageData;
            foreach (Frame frm in data.Frames.Values)
            {
                foreach (Event evt in frm.FrameEvents.Events)
                {
                    foreach (Action act in evt.Actions)
                    {
                        if (act.ObjectType < 32 || act.ObjectInfo < 0) continue;
                        ObjectInfo oI = data.FrameItems.Items[act.ObjectInfo];
                        ObjectCommon props = (ObjectCommon)oI.Properties;
                        string key = GetKeyFromAction(data.Extensions.Exts[oI.Header.Type - 32].FileName, act);
                        if (!string.IsNullOrEmpty(key))
                            Keys.Add(key.Trim());
                    }
                }
            }
        }

        public string GetKeyFromAction(string extName, Action act)
        {
            switch (extName)
            {
                case "INI++15.mfx":
                    return IniPlusPlus.KeyFromEvent(act);
            }
            return "";
        }
    }

    public class IniPlusPlus
    {
        public bool DefaultFile = true;
        public bool ReadOnly = false;
        public string DefaultFilePath = string.Empty;
        public byte BaseFolder = 2;
        public string InitialData = string.Empty;
        public bool CreateFolders = true;
        public bool EnableAutoRead = true;
        public bool UseStandardSettings = true;
        public bool UseCompression = false;
        public bool UseEncryption = false;
        public string EncryptionKey = string.Empty;
        public bool NewLine = false;
        public string NewLineText = string.Empty;
        public bool AlwaysQuoteStrings = false;
        public byte RepeatedGroups = 3;
        public byte RepeatedItems = 1;
        public char Undo = (char)0;
        public char Redo = (char)0;
        public bool CaseSensitive = false;
        public bool SaveRepeatedItems = false;
        public bool EscapeCharsInGroupNames = false;
        public bool EscapeCharsInItemNames = false;
        public bool EscapeCharsInItemValues = false;
        public bool GlobalData = false;
        public bool Index1Based = false;
        public bool EnableAutoLoad = false;
        public bool LoadAndSaveSubGroups = false;
        public bool AllowEmptyGroups = true;
        public string GlobalDataKey = string.Empty;

        public IniPlusPlus(ByteReader reader)
        {
            DefaultFile = reader.ReadByte() == 1;
            ReadOnly = reader.ReadByte() == 1;
            DefaultFilePath = reader.ReadAsciiStop(260);
            reader.Skip(2);
            BaseFolder = reader.ReadByte();
            reader.Skip(3);
            InitialData = reader.ReadAsciiStop(3000);
            CreateFolders = reader.ReadByte() == 1;
            EnableAutoRead = reader.ReadByte() == 1;
            UseStandardSettings = reader.ReadByte() == 1;
            UseCompression = reader.ReadByte() == 1;
            UseEncryption = reader.ReadByte() == 1;
            EncryptionKey = reader.ReadAsciiStop(32);
            reader.Skip(96);
            NewLine = reader.ReadByte() == 1;
            NewLineText = reader.ReadAsciiStop(10);
            AlwaysQuoteStrings = reader.ReadByte() == 1;
            reader.Skip(3);
            RepeatedGroups = reader.ReadByte();
            reader.Skip(3);
            RepeatedItems = reader.ReadByte();
            Undo = reader.ReadChar();
            Redo = reader.ReadChar();
            reader.Skip(1);
            SaveRepeatedItems = reader.ReadByte() == 1;
            EscapeCharsInGroupNames = reader.ReadByte() == 1;
            EscapeCharsInItemNames = reader.ReadByte() == 1;
            EscapeCharsInItemValues = reader.ReadByte() == 1;
            CaseSensitive = reader.ReadByte() == 1;
            GlobalData = reader.ReadByte() == 1;
            Index1Based = reader.ReadByte() == 1;
            EnableAutoLoad = reader.ReadByte() == 1;
            LoadAndSaveSubGroups = reader.ReadByte() == 1;
            AllowEmptyGroups = reader.ReadByte() == 1;
            GlobalDataKey = reader.ReadAsciiStop(32);
        }

        public static string KeyFromEvent(Action act)
        {
            if (act.Num != 131) return "";
            return act.Parameters[0].Data.ToString();
        }
    }

    public class LacewingBlue
    {
        public bool AutomaticallyClearBinary = true;
        public bool EnableMultithreading = true;
        public bool Global = true;
        public string GlobalIdentifier = string.Empty;
        public bool EnableTimeoutWarning = true;
        public bool KillConnectionWhenDisowned = false;
        public bool EnableInactivityTimer = true;

        public LacewingBlue(ByteReader reader)
        {
            reader.Skip(5);
            AutomaticallyClearBinary = reader.ReadByte() == 1;
            EnableMultithreading = reader.ReadByte() == 1;
            Global = reader.ReadByte() == 1;
            GlobalIdentifier = reader.ReadAsciiStop(255);
            EnableTimeoutWarning = reader.ReadByte() == 1;
            KillConnectionWhenDisowned = reader.ReadByte() == 1;
            EnableInactivityTimer = reader.ReadByte() == 1;
        }
    }

    public class DarkEdifTest
    {
        public string Header = string.Empty;
        public int Hash;
        public int HashTypes;
        public short PropertyCount;
        public int DataSize;
        public byte[] PropertyData;
        public DarkEdifData[] Properties;

        public DarkEdifTest(ByteReader reader)
        {
            Header = reader.ReadAscii(4);
            Hash = reader.ReadInt();
            HashTypes = reader.ReadInt();
            PropertyCount = reader.ReadShort();
            reader.Skip(2); // Padding
            DataSize = reader.ReadInt();
            PropertyData = reader.ReadBytes((int)Math.Ceiling((double)PropertyCount / 8));
            Properties = new DarkEdifData[PropertyCount];
            for (int i = 0; i < PropertyCount; i++)
            {
                DarkEdifData newData = new DarkEdifData();
                newData.Size = reader.ReadInt();
                newData.Type = reader.ReadShort();
                newData.NameSize = reader.ReadByte();
                newData.Name = reader.ReadAsciiStop(newData.NameSize);
                newData.Data = reader.ReadBytes(newData.Size - newData.NameSize - 7);
                newData.CheckboxState = (PropertyData[(int)Math.Floor((double)i / 8)] & (int)Math.Pow(2, i % 8)) != 0;
                Properties[i] = newData;
            }
        }

        public struct DarkEdifData
        {
            public int Size;
            public short Type;
            public byte NameSize;
            public string Name;
            public byte[] Data;
            public bool CheckboxState;
        }
    }

    public class FireflyEN
    {
        public ushort Width;
        public ushort Height;
        public short DeviceDriver;
        public bool StartEngineAtStart;
        public short AntiAlias;
        public Color BackgroundColour;
        public Color AmbientLightColour;
        public short RenderSpeed;
        public short TravelDistance;
        public Color FogColour;
        public float FogStartDistance;
        public float FogEndDistance;
        public bool CastShadows;
        public Color ShadowColour;

        public FireflyEN(ByteReader reader)
        {
            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
            DeviceDriver = reader.ReadShort();
            StartEngineAtStart = reader.ReadShort() != 0;
            AntiAlias = reader.ReadShort();
            reader.Skip(2);
            BackgroundColour = reader.ReadColor();
            AmbientLightColour = reader.ReadColor();
            RenderSpeed = reader.ReadShort();
            TravelDistance = reader.ReadShort();
            FogColour = reader.ReadColor();
            FogStartDistance = reader.ReadFloat();
            FogEndDistance = reader.ReadFloat();
            CastShadows = reader.ReadShort() != 0;
            reader.Skip(2);
            ShadowColour = reader.ReadColor();
        }
    }

    public class FireflyCA
    {
        public ushort Width;
        public ushort Height;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
        public bool Visible;
        public short AutomaticCulling;
        public bool DrawDebugData;
        public bool UseCollisions;
        public float RadiusX;
        public float RadiusY;
        public float RadiusZ;
        public float TranslationX;
        public float TranslationY;
        public float TranslationZ;
        public float TargetX;
        public float TargetY;
        public float TargetZ;
        public float UpVectorPositionX;
        public float UpVectorPositionY;
        public float UpVectorPositionZ;
        public float FOV;
        public float AspectRatio;
        public float NearClippingPlane;
        public float FarClippingPlane;
        public bool BindTargetAndRotation;
        public bool AutoAddToEngine;

        public FireflyCA(ByteReader reader)
        {
            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
            PositionX = reader.ReadFloat();
            PositionY = reader.ReadFloat();
            PositionZ = reader.ReadFloat();
            RotationX = reader.ReadFloat();
            RotationY = reader.ReadFloat();
            RotationZ = reader.ReadFloat();
            ScaleX = reader.ReadFloat();
            ScaleY = reader.ReadFloat();
            ScaleZ = reader.ReadFloat();
            Visible = reader.ReadShort() != 0;
            AutomaticCulling = reader.ReadShort();
            DrawDebugData = reader.ReadByte() != 0;
            UseCollisions = reader.ReadByte() != 0;
            reader.Skip(2);
            RadiusX = reader.ReadFloat();
            RadiusY = reader.ReadFloat();
            RadiusZ = reader.ReadFloat();
            TranslationX = reader.ReadFloat();
            TranslationY = reader.ReadFloat();
            TranslationZ = reader.ReadFloat();
            TargetX = reader.ReadFloat();
            TargetY = reader.ReadFloat();
            TargetZ = reader.ReadFloat();
            UpVectorPositionX = reader.ReadFloat();
            UpVectorPositionY = reader.ReadFloat();
            UpVectorPositionZ = reader.ReadFloat();
            FOV = 57.29578f * reader.ReadFloat();
            AspectRatio = reader.ReadFloat();
            NearClippingPlane = reader.ReadFloat();
            FarClippingPlane = reader.ReadFloat();
            BindTargetAndRotation = reader.ReadByte() != 0;
            AutoAddToEngine = reader.ReadByte() != 0;
            reader.Skip(2);
        }
    }

    public class FireflySK
    {
        public short Type;
        public ushort SkyboxBackTexture;
        public ushort SkyboxFrontTexture;
        public ushort SkyboxRightTexture;
        public ushort SkyboxLeftTexture;
        public ushort SkyboxBottomTexture;
        public ushort SkyboxTopTexture;
        public ushort SkydomeTexture;
        public int SkydomeHorizontalRes;
        public int SkydomeVerticalRes;
        public float SkydomeTextureHeight;
        public float SkydomeSphere;
        public float SkydomeRadius;
        public bool Visible;
        public bool AutoAddToEngine;

        public FireflySK(ByteReader reader)
        {
            Type = reader.ReadShort();
            SkyboxBackTexture = reader.ReadUShort();
            SkyboxFrontTexture = reader.ReadUShort();
            SkyboxRightTexture = reader.ReadUShort();
            SkyboxLeftTexture = reader.ReadUShort();
            SkyboxBottomTexture = reader.ReadUShort();
            SkyboxTopTexture = reader.ReadUShort();
            SkydomeTexture = reader.ReadUShort();
            SkydomeHorizontalRes = reader.ReadInt();
            SkydomeVerticalRes = reader.ReadInt();
            SkydomeTextureHeight = reader.ReadFloat();
            SkydomeSphere = reader.ReadFloat();
            SkydomeRadius = reader.ReadFloat();
            Visible = reader.ReadByte() != 0;
            AutoAddToEngine = reader.ReadByte() != 0;
            reader.Skip(26);
        }
    }
}
