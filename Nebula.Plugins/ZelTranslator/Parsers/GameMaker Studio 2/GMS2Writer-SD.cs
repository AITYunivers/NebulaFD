using Microsoft.Win32;
using Nebula;
using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.FileReaders;
using Nebula.Core.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZelTranslator_SD.Parsers.GameMakerStudio2;
using static ZelTranslator_SD.Parsers.GameMakerStudio2.ExtensionReaders;
using System.Drawing;
using Color = System.Drawing.Color;
using Nebula.Core.Data.Chunks.BankChunks.Fonts;
using Font = Nebula.Core.Data.Chunks.BankChunks.Fonts.Font;
using static ZelTranslator_SD.Parsers.GameMakerStudio2.ObjectYY;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions;
using System.Reflection;
using Newtonsoft.Json;

namespace ZelTranslator_SD.Parsers
{
    public class GMS2Writer
    {
        public static int SpriteOrder = 0;
        public static int ObjectOrder = 0;
        public static int FrameOrder = 0;
        public static int SoundOrder = 0;
        public static int FolderOrder = 1;
        public static Dictionary<string, bool> Args = new Dictionary<string, bool>(){
            {"-nosnds", false },
            {"-noimgs", false},
            {"-nogml", false },
            {"-fnafworld", false }
        };

        public static string CleanString(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(str, "").Trim(' ');
        }
        public static string CleanStringFull(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(str, "").Trim(' ');
        }
        public static string ObjectName(ObjectInfo obj)
        {
            return "obj_" + CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
        }
        public static string SoundName(Sound item)
        {
            return "snd_" + CleanStringFull(item.Name) + "_" + item.Handle;
        }

        public static char RandomChar()
        {
            char newChar = (char)50;
            int type = RandomNumberGenerator.GetInt32(0, 2);
            switch (type)
            {
                case 0: // Numbers
                    newChar = (char)RandomNumberGenerator.GetInt32(48, 58);
                    break;
                case 1: // Letters
                    newChar = (char)RandomNumberGenerator.GetInt32(97, 123);
                    break;
            }
            return newChar;
        }
        public static uint ToUIntColour(Color colour)
        {
            uint UIntCol = (UInt32)255 << 24; // Alpha (no bg transparency in fusion)
            UIntCol += (UInt32)colour.B << 16;
            UIntCol += (UInt32)colour.G << 8;
            UIntCol += colour.R;
            return UIntCol;
        }
        public static string NewInstanceID()
        {
            var str = "";
            for (int i = 0; i < 8; i++)
                str += RandomChar();
            return str;
        }

        public static Dictionary<int, string> GMS2ObjectIDs = new();

        public static void Write(PackageData gameData)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Markup("[Green]GameMaker Studio 2 Translator[/]");
            AnsiConsole.MarkupLine($"\nArguments (leave empty for none):");

            string arguments = Console.ReadLine().Trim().ToLower();
            if (!string.IsNullOrEmpty(arguments))
            {
                string[] args = arguments.Split(" ");
                foreach (string arg in args)
                {
                    foreach (var option in Args)
                    {
                        if (option.Key == arg) Args[arg] = true;
                    }
                }
            }
            var outName = gameData.AppName ?? "Unknown Game";
            outName = CleanString(outName);
            var outPath = $"Dumps\\{outName}\\GMS2";

            var ProjectJSON = new ProjectYYP.RootObject();
            ProjectJSON.name = outName;

            // Object ID Generators 
            foreach (var obj in gameData.FrameItems.Items.Values)
            {
                if (GMS2ObjectIDs.Keys.Contains(obj.Header.Handle)) continue;
                var str = "";
                for (int i = 0; i < 8; i++)
                    str += RandomChar();

                GMS2ObjectIDs.Add(obj.Header.Handle, str);
            }

            // Options
            var Options = new List<ProjectYYP.Option>();

            var OptionMain = new ProjectYYP.Option();
            OptionMain.name = "Main";
            OptionMain.path = "options/main/options_main.yy";
            Options.Add(OptionMain);

            var OptionWindows = new ProjectYYP.Option();
            OptionWindows.name = "Windows";
            OptionWindows.path = "options/windows/options_windows.yy";
            Options.Add(OptionWindows);

            ProjectJSON.Options = Options.ToArray();
            Options.Clear();

            // Folders
            var Folders = new List<ProjectYYP.Folder>();

            var FolderSprites = new ProjectYYP.Folder();
            FolderSprites.name = "Sprites";
            FolderSprites.folderPath = "folders/Sprites.yy";
            FolderSprites.order = FolderOrder;
            FolderOrder++;
            Folders.Add(FolderSprites);

            var FolderSounds = new ProjectYYP.Folder();
            FolderSounds.name = "Sounds";
            FolderSounds.folderPath = "folders/Sounds.yy";
            FolderSounds.order = FolderOrder;
            FolderOrder++;
            Folders.Add(FolderSounds);

            var FolderScripts = new ProjectYYP.Folder();
            FolderScripts.name = "Scripts";
            FolderScripts.folderPath = "folders/Scripts.yy";
            FolderScripts.order = FolderOrder;
            FolderOrder++;
            Folders.Add(FolderScripts);

            var FolderObjects = new ProjectYYP.Folder();
            FolderObjects.name = "Objects";
            FolderObjects.folderPath = "folders/Objects.yy";
            FolderObjects.order = FolderOrder;
            FolderOrder++;
            Folders.Add(FolderObjects);

            var FolderRooms = new ProjectYYP.Folder();
            FolderRooms.name = "Rooms";
            FolderRooms.folderPath = "folders/Rooms.yy";
            FolderRooms.order = FolderOrder;
            FolderOrder++;
            Folders.Add(FolderRooms);

            ProjectJSON.Folders = Folders.ToArray();
            Folders.Clear();

            var Rooms = new List<RoomYY.RootObject>();
            var RoomOrderNodes = new List<ProjectYYP.RoomOrderNode>();
            var Resources = new List<ProjectYYP.Resource>();
            foreach (var Frame in gameData.Frames)
            {
                Logger.LogType(typeof(GMS2Writer), "Loading Frame " + Frame.FrameName + " - MvmtSpd=" + Frame.FrameMoveTimer.ToString());
                var newRoom = new RoomYY.RootObject();
                newRoom.name = $"rm{FrameOrder}_{CleanStringFull(Frame.FrameName)}";

                var Views = new List<RoomYY.View>();
                for (int a = 0; a < 8; a++)
                {
                    var newView = new RoomYY.View();
                    newView.wview = Frame.FrameHeader.Width;
                    newView.hview = Frame.FrameHeader.Height;
                    newView.wport = Frame.FrameHeader.Width;
                    newView.hport = Frame.FrameHeader.Height;
                    Views.Add(newView);
                }
                newRoom.views = Views.ToArray();
                Views.Clear();

                newRoom.roomSettings.Width = Frame.FrameHeader.Width;
                newRoom.roomSettings.Height = Frame.FrameHeader.Height;

                var roomLayers = new List<RoomYY.Layer>();

                // Background Layer
                var bgLayer = new RoomYY.Layer();
                bgLayer.resourceType = "GMRBackgroundLayer";
                bgLayer.name = "Background";
                bgLayer.visible = true;
                bgLayer.colour = ToUIntColour(Frame.FrameHeader.Background); // Convert Frame BG color to 32-bit unsigned int for GMS
                roomLayers.Add(bgLayer);

                // Instance Layers
                var instances = new List<RoomYY.InstanceCreationOrder>();
                int layer = 0;
                try
                {
                    foreach (var Layer in Frame.FrameLayers.Layers)
                    {
                        layer++;
                        var newLayer = new RoomYY.Layer();
                        newLayer.resourceType = "GMRInstanceLayer";
                        newLayer.name = CleanString(Layer.Name);
                        newLayer.visible = !Layer.LayerFlags["HiddenAtStart"];

                        var LayerInstances = new List<RoomYY.Instance>();
                        foreach (var LayerInstance in Frame.FrameInstances.Instances)
                        {
                            if (LayerInstance.ParentType != 0) continue; // Don't add instance if not included in frame at start
                            ObjectInfo inst = gameData.FrameItems.Items[(int)LayerInstance.ObjectInfo];
                            if (inst.Header.Type >= 2 && (inst.Properties as ObjectCommon).ObjectFlags["DontCreateAtStart"]) continue; // Don't add instance if Create At Start is disabled
                            if (LayerInstance.Layer == layer - 1)
                            {
                                var instance = gameData.FrameItems.Items[(int)LayerInstance.ObjectInfo];

                                var newInstance = new RoomYY.Instance();
                                newInstance.name = $"inst_{NewInstanceID().ToUpper()}"; // EVERY instance in GMS2 has to have a unique ID, even if it represents the same object
                                newInstance.x = LayerInstance.PositionX;
                                newInstance.y = LayerInstance.PositionY;
                                //newInstance.colour = ((long)instance.blend * 16777216) + 16777215; // Come back and fix for each color/graphics mode
                                newInstance.colour = 4294967295;

                                // Quick Backdrop instances
                                if (instance.Properties is ObjectQuickBackdrop quickbackdrop)
                                {
                                    try
                                    {
                                        newInstance.scaleX = (float)Decimal.Divide(quickbackdrop.Width, gameData.ImageBank.Images[quickbackdrop.Shape.Image].Width);
                                        newInstance.scaleY = (float)Decimal.Divide(quickbackdrop.Height, gameData.ImageBank.Images[quickbackdrop.Shape.Image].Height);
                                    }
                                    catch
                                    {
                                        newInstance.scaleX = quickbackdrop.Width;
                                        newInstance.scaleY = quickbackdrop.Height;
                                    }
                                    if (quickbackdrop.Shape.FillType == 1)
                                    {
                                        newInstance.colour = ToUIntColour(quickbackdrop.Shape.Color1);
                                    }
                                }

                                var objectID = new RoomYY.ObjectID();
                                objectID.name = CleanString(instance.Name).Replace(" ", "_") + "_" + instance.Header.Handle;
                                objectID.name = GMS2Writer.ObjectName(instance);
                                // Extensions instances (fix/rework this to be a loop)
                                if (instance.Properties is ObjectCommon common && common.Identifier != "SPRI" && common.Identifier != "SP" && common.Identifier != "CNTR" && common.Identifier != "CN")
                                {
                                    if (common.Identifier == "omfp" || common.Identifier == "2YOJ")
                                        objectID.name = common.Identifier + "_" + objectID.name;
                                }
                                /*
                                if (instance.properties is ObjectCommon commonCntr && (commonCntr.Identifier == "CNTR" || commonCntr.Identifier == "CN"))
                                {
                                    File.WriteAllText($"counters\\{objectID.name}-preferences.txt", $"{commonCntr.Preferences}");
                                    File.WriteAllText($"counters\\{objectID.name}-flags.txt", $"{commonCntr.Flags}" + $";\n\nLayerInstance.flags={LayerInstance.flags};\ninstance.Flags={instance.Flags}");
                                    File.WriteAllText($"counters\\{objectID.name}-newflags.txt", $"{commonCntr.NewFlags}");
                                }
                                */
                                objectID.path = $"objects/{objectID.name}/{objectID.name}.yy";
                                newInstance.objectId = objectID;
                                LayerInstances.Add(newInstance);

                                var instanceCreation = new RoomYY.InstanceCreationOrder();
                                instanceCreation.name = newInstance.name;
                                instanceCreation.path = $"rooms/{newRoom.name}/{newRoom.name}.yy";
                                instances.Add(instanceCreation);
                            }
                        }

                        newLayer.instances = LayerInstances.ToArray();
                        LayerInstances.Clear();

                        roomLayers.Add(newLayer);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogType(typeof(GMS2Writer), $"Error with frame layer: {ex}");
                }

                var newRoomLayers = new List<RoomYY.Layer>();

                for (int ly = 1; ly <= roomLayers.Count; ly++)
                {
                    var roomLayer = roomLayers[roomLayers.Count - ly];
                    newRoomLayers.Add(roomLayer);
                }

                newRoom.instanceCreationOrder = instances.ToArray();
                newRoom.layers = newRoomLayers.ToArray();
                instances.Clear();
                roomLayers.Clear();

                Rooms.Add(newRoom);

                var newRoomNode = new ProjectYYP.RoomOrderNode();
                var newRoomNodeID = new ProjectYYP.RoomID();

                newRoomNodeID.name = newRoom.name;
                newRoomNodeID.path = $"rooms/{newRoom.name}/{newRoom.name}.yy";
                newRoomNode.roomId = newRoomNodeID;
                RoomOrderNodes.Add(newRoomNode);

                var newRoomRes = new ProjectYYP.Resource();
                var newRoomResID = new ProjectYYP.ResourceID();

                newRoomResID.name = newRoom.name;
                newRoomResID.path = $"rooms/{newRoom.name}/{newRoom.name}.yy";
                newRoomRes.id = newRoomResID;
                newRoomRes.order = FrameOrder;
                FrameOrder++;
                Resources.Add(newRoomRes);
            }

            var Sprites = new List<SpriteYY.RootObject>();
            foreach (var obj in gameData.FrameItems.Items.Values)
            {
                try
                {
                    // Counters sprites
                    if (obj.Properties is ObjectCommon commonCntr && (commonCntr.Identifier == "CNTR" || commonCntr.Identifier == "CN" || !NebulaCore.Plus && obj.Header.Type == 7))
                    {
                        Logger.LogType(typeof(GMS2Writer), $"Counter found: {obj.Name}");
                        var counter = commonCntr.ObjectCounter;

                        var newSprite = new SpriteYY.RootObject();
                        uint[] imgs = { 0 };
                        if (counter.Frames.Length > 0) imgs = counter.Frames;
                        var baseimg = gameData.ImageBank.Images[imgs[0]];
                        newSprite.name = $"CounterSprite_{imgs[0]}";

                        newSprite.bbox_right = baseimg.Width - 1;
                        newSprite.bbox_bottom = baseimg.Height - 1;
                        newSprite.width = baseimg.Width;
                        newSprite.height = baseimg.Height;

                        newSprite.frames = new SpriteYY.Frame[imgs.Length];

                        for (int i = 0; i < imgs.Length; i++)
                        {
                            newSprite.frames[i] = new SpriteYY.Frame();
                            newSprite.frames[i].name = Guid.NewGuid().ToString();
                            newSprite.frames[i].ctfhandle = (int)imgs[i];
                        }

                        var newSequence = new SpriteYY.Sequence();
                        newSequence.name = $"CounterSprite_{obj.Header.Handle}";
                        newSequence.playbackSpeed = 0;
                        newSequence.length = imgs.Length;
                        newSequence.backdropWidth = gameData.AppHeader.AppWidth;
                        newSequence.backdropHeight = gameData.AppHeader.AppHeight;
                        newSequence.xorigin = baseimg.Width;
                        newSequence.yorigin = baseimg.Height;

                        var seqFrames = new List<SpriteYY.KeyFrame>();
                        int fi = 0;
                        foreach (var frame in newSprite.frames)
                        {
                            var newKeyFrame = new SpriteYY.KeyFrame();
                            newKeyFrame.id = Guid.NewGuid().ToString();
                            newKeyFrame.Key = fi;
                            newKeyFrame.Channels.ZEROREPLACE.Id.name = frame.name;
                            newKeyFrame.Channels.ZEROREPLACE.Id.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                            seqFrames.Add(newKeyFrame);
                            fi++;
                        }
                        newSequence.tracks[0].keyframes.Keyframes = seqFrames.ToArray();

                        newSprite.sequence = newSequence;
                        newSprite.layers[0].name = Guid.NewGuid().ToString();
                        Sprites.Add(newSprite);

                        var newSpriteRes = new ProjectYYP.Resource();
                        var newSpriteResID = new ProjectYYP.ResourceID();

                        newSpriteResID.name = newSprite.name;
                        newSpriteResID.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                        newSpriteRes.id = newSpriteResID;
                        newSpriteRes.order = SpriteOrder;
                        SpriteOrder++;
                        Resources.Add(newSpriteRes);
                        continue;
                    }

                    // Actives/Common Objs sprites
                    if (obj.Properties is ObjectCommon common)
                    {
                        if (common.ObjectAnimations.Animations.Count == 0)
                            continue;
                        foreach (int ad in common.ObjectAnimations.Animations.Keys)
                        {
                            if (common.ObjectAnimations.Animations[ad].Directions.Count == 0)
                                continue;
                            foreach (ObjectDirection direction in common.ObjectAnimations.Animations[ad].Directions)
                            {
                                int dd = direction.Index;
                                if (direction.Frames.Length == 0) continue;
                                var newSprite = new SpriteYY.RootObject();
                                var imgs = direction.Frames;
                                var baseimg = gameData.ImageBank.Images[imgs[0]];
                                var objName = CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
                                newSprite.name = $"spr_{objName}_{imgs[0]}_{ad}_{dd}";

                                newSprite.bbox_right = baseimg.Width - 1;
                                newSprite.bbox_bottom = baseimg.Height - 1;
                                newSprite.width = baseimg.Width;
                                newSprite.height = baseimg.Height;

                                newSprite.frames = new SpriteYY.Frame[imgs.Length];

                                for (int i = 0; i < imgs.Length; i++)
                                {
                                    newSprite.frames[i] = new SpriteYY.Frame();
                                    newSprite.frames[i].name = Guid.NewGuid().ToString();
                                    newSprite.frames[i].ctfhandle = (int)imgs[i];
                                }

                                var newSequence = new SpriteYY.Sequence();
                                newSequence.name = $"spr_{objName}_{obj.Header.Handle}_{ad}_{dd}";
                                newSequence.playbackSpeed = direction.MinimumSpeed;
                                newSequence.playbackSpeed = newSequence.playbackSpeed / 100.0f * 60.0f;
                                newSequence.length = imgs.Length;
                                newSequence.backdropWidth = gameData.AppHeader.AppWidth;
                                newSequence.backdropHeight = gameData.AppHeader.AppHeight;
                                newSequence.xorigin = gameData.ImageBank.Images[imgs[0]].HotspotX;
                                newSequence.yorigin = gameData.ImageBank.Images[imgs[0]].HotspotY;

                                var seqFrames = new List<SpriteYY.KeyFrame>();
                                int fi = 0;
                                foreach (var frame in newSprite.frames)
                                {
                                    var newKeyFrame = new SpriteYY.KeyFrame();
                                    newKeyFrame.id = Guid.NewGuid().ToString();
                                    newKeyFrame.Key = fi;
                                    newKeyFrame.Channels.ZEROREPLACE.Id.name = frame.name;
                                    newKeyFrame.Channels.ZEROREPLACE.Id.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                                    seqFrames.Add(newKeyFrame);
                                    fi++;
                                }
                                newSequence.tracks[0].keyframes.Keyframes = seqFrames.ToArray();

                                newSprite.sequence = newSequence;
                                newSprite.layers[0].name = Guid.NewGuid().ToString();
                                Sprites.Add(newSprite);

                                var newSpriteRes = new ProjectYYP.Resource();
                                var newSpriteResID = new ProjectYYP.ResourceID();

                                newSpriteResID.name = newSprite.name;
                                newSpriteResID.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                                newSpriteRes.id = newSpriteResID;
                                newSpriteRes.order = SpriteOrder;
                                SpriteOrder++;
                                Resources.Add(newSpriteRes);
                            }
                        }
                        continue;
                    }

                    // Backdrops sprites (FIX/REWORK come back to this)
                    if (obj.Properties is ObjectBackdrop backdrop)
                    {
                        var newSprite = new SpriteYY.RootObject();
                        var imgs = new List<int>() { (int)backdrop.Image };
                        var baseimg = gameData.ImageBank.Images[(uint)imgs[0]];
                        newSprite.name = $"Backdrop_{imgs[0]}";

                        newSprite.bbox_right = baseimg.Width - 1;
                        newSprite.bbox_bottom = baseimg.Height - 1;
                        newSprite.width = baseimg.Width;
                        newSprite.height = baseimg.Height;

                        newSprite.frames = new SpriteYY.Frame[imgs.Count];

                        for (int i = 0; i < imgs.Count; i++)
                        {
                            newSprite.frames[i] = new SpriteYY.Frame();
                            newSprite.frames[i].name = Guid.NewGuid().ToString();
                            newSprite.frames[i].ctfhandle = imgs[i];
                        }

                        var newSequence = new SpriteYY.Sequence();
                        newSequence.name = $"Backdrop-{obj.Header.Handle}";
                        newSequence.playbackSpeed = 0;
                        newSequence.length = 1;
                        newSequence.backdropWidth = gameData.AppHeader.AppWidth;
                        newSequence.backdropHeight = gameData.AppHeader.AppHeight;
                        newSequence.xorigin = 0;
                        newSequence.yorigin = 0;

                        var seqFrames = new List<SpriteYY.KeyFrame>();
                        int fi = 0;
                        foreach (var frame in newSprite.frames)
                        {
                            var newKeyFrame = new SpriteYY.KeyFrame();
                            newKeyFrame.id = Guid.NewGuid().ToString();
                            newKeyFrame.Key = fi;
                            newKeyFrame.Channels.ZEROREPLACE.Id.name = frame.name;
                            newKeyFrame.Channels.ZEROREPLACE.Id.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                            seqFrames.Add(newKeyFrame);
                            fi++;
                        }
                        newSequence.tracks[0].keyframes.Keyframes = seqFrames.ToArray();

                        newSprite.sequence = newSequence;
                        newSprite.layers[0].name = Guid.NewGuid().ToString();
                        Sprites.Add(newSprite);

                        var newSpriteRes = new ProjectYYP.Resource();
                        var newSpriteResID = new ProjectYYP.ResourceID();

                        newSpriteResID.name = newSprite.name;
                        newSpriteResID.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                        newSpriteRes.id = newSpriteResID;
                        newSpriteRes.order = SpriteOrder;
                        SpriteOrder++;
                        Resources.Add(newSpriteRes);
                        continue;
                    }
                    // Quick Backdrop sprites (functions fine, but needs a fix/rework)
                    if (obj.Properties is ObjectQuickBackdrop quickbackdrop)
                    {
                        Logger.LogType(typeof(GMS2Writer), $"QUICK BACKDROP Found - ShapeType: {quickbackdrop.Shape.ShapeType} | FillType: {quickbackdrop.Shape.FillType}");

                        if (quickbackdrop.Shape.FillType == 3)
                        {
                            var newSprite = new SpriteYY.RootObject();
                            var imgs = new List<int>() { (int)quickbackdrop.Shape.Image };
                            var baseimg = gameData.ImageBank.Images[(uint)imgs[0]];

                            newSprite.name = $"QuickBackdrop_{imgs[0]}";
                            newSprite.bbox_right = baseimg.Width - 1;
                            newSprite.bbox_bottom = baseimg.Height - 1;
                            newSprite.width = baseimg.Width;
                            newSprite.height = baseimg.Height;

                            newSprite.frames = new SpriteYY.Frame[imgs.Count];

                            newSprite.nineSlice.enabled = true;
                            newSprite.nineSlice.tileMode = new int[5] { 0, 0, 0, 0, 1 };

                            for (int i = 0; i < imgs.Count; i++)
                            {
                                newSprite.frames[i] = new SpriteYY.Frame();
                                newSprite.frames[i].name = Guid.NewGuid().ToString();
                                newSprite.frames[i].ctfhandle = imgs[i];
                            }

                            var newSequence = new SpriteYY.Sequence();
                            newSequence.name = $"QuickBackdrop-{obj.Header.Handle}";
                            newSequence.playbackSpeed = 0;
                            newSequence.length = 1;
                            newSequence.backdropWidth = gameData.AppHeader.AppWidth;
                            newSequence.backdropHeight = gameData.AppHeader.AppHeight;
                            newSequence.xorigin = 0;
                            newSequence.yorigin = 0;

                            var seqFrames = new List<SpriteYY.KeyFrame>();
                            int fi = 0;
                            foreach (var frame in newSprite.frames)
                            {
                                var newKeyFrame = new SpriteYY.KeyFrame();
                                newKeyFrame.id = Guid.NewGuid().ToString();
                                newKeyFrame.Key = fi;
                                newKeyFrame.Channels.ZEROREPLACE.Id.name = frame.name;
                                newKeyFrame.Channels.ZEROREPLACE.Id.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                                seqFrames.Add(newKeyFrame);
                                fi++;
                            }
                            newSequence.tracks[0].keyframes.Keyframes = seqFrames.ToArray();

                            newSprite.sequence = newSequence;
                            newSprite.layers[0].name = Guid.NewGuid().ToString();
                            Sprites.Add(newSprite);

                            var newSpriteRes = new ProjectYYP.Resource();
                            var newSpriteResID = new ProjectYYP.ResourceID();

                            newSpriteResID.name = newSprite.name;
                            newSpriteResID.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                            newSpriteRes.id = newSpriteResID;
                            newSpriteRes.order = SpriteOrder;
                            SpriteOrder++;
                            Resources.Add(newSpriteRes);
                        }
                        if (quickbackdrop.Shape.FillType == 1)
                        {
                            var newSprite = new SpriteYY.RootObject();
                            var imgs = new List<int>() { (int)quickbackdrop.Shape.Image };
                            var baseimg = gameData.ImageBank.Images[(uint)imgs[0]];

                            newSprite.name = $"QuickBackdrop_{imgs[0]}";
                            newSprite.bbox_right = baseimg.Width - 1;
                            newSprite.bbox_bottom = baseimg.Height - 1;
                            newSprite.width = baseimg.Width;
                            newSprite.height = baseimg.Height;

                            newSprite.frames = new SpriteYY.Frame[imgs.Count];

                            newSprite.nineSlice.enabled = true;
                            newSprite.nineSlice.tileMode = new int[5] { 0, 0, 0, 0, 1 };

                            for (int i = 0; i < imgs.Count; i++)
                            {
                                newSprite.frames[i] = new SpriteYY.Frame();
                                newSprite.frames[i].name = Guid.NewGuid().ToString();
                                newSprite.frames[i].ctfhandle = imgs[i];
                                newSprite.frames[i].FillType = 1;
                                newSprite.frames[i].solidColor = quickbackdrop.Shape.Color1;
                            }

                            var newSequence = new SpriteYY.Sequence();
                            newSequence.name = $"QuickBackdrop-{obj.Header.Handle}";
                            newSequence.playbackSpeed = 0;
                            newSequence.length = 1;
                            newSequence.backdropWidth = gameData.AppHeader.AppWidth;
                            newSequence.backdropHeight = gameData.AppHeader.AppHeight;
                            newSequence.xorigin = 0;
                            newSequence.yorigin = 0;

                            var seqFrames = new List<SpriteYY.KeyFrame>();
                            int fi = 0;
                            foreach (var frame in newSprite.frames)
                            {
                                var newKeyFrame = new SpriteYY.KeyFrame();
                                newKeyFrame.id = Guid.NewGuid().ToString();
                                newKeyFrame.Key = fi;
                                newKeyFrame.Channels.ZEROREPLACE.Id.name = frame.name;
                                newKeyFrame.Channels.ZEROREPLACE.Id.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                                seqFrames.Add(newKeyFrame);
                                fi++;
                            }
                            newSequence.tracks[0].keyframes.Keyframes = seqFrames.ToArray();

                            newSprite.sequence = newSequence;
                            newSprite.layers[0].name = Guid.NewGuid().ToString();
                            Sprites.Add(newSprite);

                            var newSpriteRes = new ProjectYYP.Resource();
                            var newSpriteResID = new ProjectYYP.ResourceID();

                            newSpriteResID.name = newSprite.name;
                            newSpriteResID.path = $"sprites/{newSprite.name}/{newSprite.name}.yy";
                            newSpriteRes.id = newSpriteResID;
                            newSpriteRes.order = SpriteOrder;
                            SpriteOrder++;
                            Resources.Add(newSpriteRes);
                        }
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogType(typeof(GMS2Writer), ex);
                }
            }

            // Fonts (ty Yunivers)
            Logger.LogType(typeof(GMS2Writer), $"Parsing Fonts");
            RegistryKey? rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts");
            RegistryKey? rk2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts");
            Dictionary<string, string> fontsToWrite = new();

            if (gameData.FontBank.Fonts.Count > 0)
            {
                foreach (var item in gameData.FontBank.Fonts)
                {
                    var fnt = item.Value;
                    Logger.LogType(typeof(GMS2Writer), $"FONT: Handle={fnt.Handle} [{fnt}]");
                    if (fontsToWrite.ContainsKey(fnt.Name)) continue;
                    bool found = false;
                    foreach (var key in rk.GetValueNames())
                        if (key.ToLower().StartsWith(fnt.Name.ToLower()))
                        {
                            string path = "C:\\Windows\\Fonts\\" + rk.GetValue(key).ToString();
                            //fontsToWrite.Add(Path.GetExtension(path).ToLower(), File.ReadAllBytes(path));
                            fontsToWrite.Add(fnt.Name, path);
                            found = true;
                            break;
                        }
                    if (!found)
                    {
                        foreach (var key in rk2.GetValueNames())
                            if (key.ToLower().StartsWith(fnt.Name.ToLower()))
                            {
                                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                                string path = $"{localAppData}\\Microsoft\\Windows\\Fonts\\{Path.GetFileName(rk2.GetValue(key).ToString())}";
                                //fontsToWrite.Add(Path.GetExtension(path).ToLower(), File.ReadAllBytes(path));
                                fontsToWrite.Add(fnt.Name, path);
                                found = true;
                                break;
                            }
                    }
                }
            }
            var Objects = new List<ObjectYY.RootObject>();
            List<ObjectYY.GMLFile> GMLFiles = new List<ObjectYY.GMLFile>();
            foreach (var obj in gameData.FrameItems.Items.Values)
            {
                if (obj.Properties is ObjectCommon common)
                {
                    //Counters objects
                    if (obj.Header.Type == 7)
                    {
                        Logger.LogType(typeof(GMS2Writer), $"Counter Object: {obj.Name}|{common.Identifier}");
                        var counters = common.ObjectCounter;
                        var counter = common.ObjectValue;
                        var str = common.ObjectAlterableStrings;
                        var imgs = new int[] { 0 };
                        try
                        {
                            if (counters.Frames.Length > 0) imgs = Array.ConvertAll(counters.Frames, item => (int)item);
                        }
                        catch
                        {
                            Logger.LogType(typeof(GMS2Writer), "ERROR: Counter does not have Frames?");
                        }

                        var newObj = new ObjectYY.RootObject();
                        newObj.name = CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
                        newObj.name = GMS2Writer.ObjectName(obj);
                        newObj.visible = common.NewObjectFlags["VisibleAtStart"];
                        newObj.spriteId.name = $"CounterSprite_{imgs[0]}";
                        newObj.spriteId.path = $"sprites/{newObj.spriteId.name}/{newObj.spriteId.name}.yy";

                        var events = new List<ObjectYY.Event>();

                        var createEv = new ObjectYY.Event();
                        createEv.eventNum = 0;
                        createEv.eventType = 0;
                        events.Add(createEv);

                        var createEvFile = new ObjectYY.GMLFile();
                        createEvFile.name = "Create_0";
                        createEvFile.path = $"objects\\{newObj.name}";
                        createEvFile.code = $"object_name = \"{obj.Name}\";\n"; // Object name
                        createEvFile.code += $"cntrvalue = {counter.Initial};\nminval = {counter.Minimum};\nmaxval = {counter.Maximum};\nspriteFont = font_add_sprite_ext({newObj.spriteId.name}, \"0123456789-+.e\", false, false);\nflash = [ 0, 0 ];\nimage_alpha = 1.0 - ({obj.Header.BlendCoeff}/255.0);";
                        
                        WriteMovement(common, createEvFile);

                        if (obj.Header.ObjectFlags["GlobalObject"]) // Global counters (scott cawthon moment)
                        {
                            createEvFile.code += "\nname = object_get_name(object_index);\nif variable_global_exists(name) then cntrvalue = variable_global_get(name);";

                            var stepEv = new ObjectYY.Event();
                            stepEv.eventNum = 0;
                            stepEv.eventType = 3;
                            events.Add(stepEv);

                            var stepEvFile = new ObjectYY.GMLFile();
                            stepEvFile.name = "Step_0";
                            stepEvFile.path = $"objects\\{newObj.name}";
                            stepEvFile.code = "variable_global_set(name, cntrvalue);";
                            GMLFiles.Add(stepEvFile);
                        }
                        File.WriteAllText($"counters\\{newObj.name}-preferences.txt", $"{obj.Properties}");
                        File.WriteAllText($"counters\\{newObj.name}-flags.txt", $"{common.ObjectFlags}" + $";\n\nobj.Flags={obj.Header.ObjectFlags.Value}");
                        File.WriteAllText($"counters\\{newObj.name}-newflags.txt", $"{common.NewObjectFlags}");


                        GMLFiles.Add(createEvFile);

                        // For Counters that don't follow the frame when scrolling
                        if (common.ObjectFlags["DontFollowFrame"])
                        {
                            var drawGuiEv = new ObjectYY.Event();
                            drawGuiEv.eventNum = 64;
                            drawGuiEv.eventType = 8;
                            events.Add(drawGuiEv);

                            var drawGuiEvFile = new ObjectYY.GMLFile();
                            drawGuiEvFile.name = "Draw_64";
                            drawGuiEvFile.path = $"objects\\{newObj.name}";
                            drawGuiEvFile.code = "draw_set_font(spriteFont);\ndraw_set_halign(fa_right);\ndraw_set_valign(fa_bottom);\ndraw_text(x, y, string(cntrvalue));\n";
                            drawGuiEvFile.code += @"if (flash[1] > 0) {
	flash[0] += delta_time/1000000.0
	if (flash[0] >= flash[1]) {
		flash[0] = 0;
		visible = !visible;
	}
}";
                            GMLFiles.Add(drawGuiEvFile);

                            var drawEv = new ObjectYY.Event();
                            drawEv.eventNum = 0;
                            drawEv.eventType = 8;
                            events.Add(drawEv);

                            var drawEvFile = new ObjectYY.GMLFile();
                            drawEvFile.name = "Draw_0";
                            drawEvFile.path = $"objects\\{newObj.name}";
                            drawEvFile.code = "/// @description LEAVE EMPTY";
                            GMLFiles.Add(drawEvFile);
                        }
                        else
                        {
                            var drawEv = new ObjectYY.Event();
                            drawEv.eventNum = 0;
                            drawEv.eventType = 8;
                            events.Add(drawEv);

                            var drawEvFile = new ObjectYY.GMLFile();
                            drawEvFile.name = "Draw_0";
                            drawEvFile.path = $"objects\\{newObj.name}";
                            drawEvFile.code = "draw_set_font(spriteFont);\ndraw_set_halign(fa_right);\ndraw_set_valign(fa_bottom);\ndraw_text(x, y, string(cntrvalue));\n";
                            drawEvFile.code += @"if (flash[1] > 0) {
	flash[0] += delta_time/1000000.0
	if (flash[0] >= flash[1]) {
		flash[0] = 0;
		visible = !visible;
	}
}";
                            GMLFiles.Add(drawEvFile);
                        }

                        newObj.eventList = events.ToArray();

                        Objects.Add(newObj);

                        var newObjectRes = new ProjectYYP.Resource();
                        var newObjectResID = new ProjectYYP.ResourceID();

                        newObjectResID.name = newObj.name;
                        newObjectResID.path = $"objects/{newObj.name}/{newObj.name}.yy";
                        newObjectRes.id = newObjectResID;
                        newObjectRes.order = ObjectOrder;
                        ObjectOrder++;
                        Resources.Add(newObjectRes);
                        continue;
                    }

                    //String objects
                    if (obj.Header.Type == 3)
                    {
                        var font = new Font();
                        string fname = "";
                        string fontVar = "-1; // MISSING FONT";
                        foreach (var fnt in gameData.FontBank.Fonts.Values)
                        {
                            if (fnt.Handle == common.ObjectParagraphs.Paragraphs[0].FontHandle)
                            {
                                font = fnt;
                                try
                                {
                                    fname = Path.GetFileName(fontsToWrite[font.Name]);
                                    fontVar = $"font_add(\"Assets/Fonts/{fname}\",size,bold,italic,0,256);";
                                }
                                catch // If font is missing from PC (-1 = default GML draw font)
                                {
                                    fontVar = $"-1; // MISSING FONT: \"{font.Name}\"";
                                    Logger.LogType(typeof(GMS2Writer), $"NOTICE: Font \"{font.Name}\" is errored or not installed!");
                                }

                                break;
                            }
                        }
                        var str = common.ObjectParagraphs;
                        var items = str.Paragraphs;
                        Logger.LogType(typeof(GMS2Writer), $"String Object: {obj.Name}|{common.Identifier}|{font.Name}|{font.Weight}|{font.Width}x{font.Height}|{font.PitchAndFamily}");

                        var newObj = new ObjectYY.RootObject();
                        newObj.name = GMS2Writer.ObjectName(obj);
                        newObj.visible = common.NewObjectFlags["VisibleAtStart"];

                        var events = new List<ObjectYY.Event>();

                        var createEv = new ObjectYY.Event();
                        createEv.eventNum = 0;
                        createEv.eventType = 0;
                        events.Add(createEv);

                        var createEvFile = new ObjectYY.GMLFile();
                        createEvFile.name = "Create_0";
                        createEvFile.path = $"objects\\{newObj.name}";

                        Color c = items[0].Color;
                        string paragraphs = "";
                        foreach (ObjectParagraph p in items) paragraphs += $"@\"{p.Value.Replace("\"", "\"+\"\\\"\"+@\"")}\",\n";
                        createEvFile.code =
$@"object_name = ""{obj.Name}"";
flash = [ 0, 0 ];
width = {str.Width};
height = {str.Height};
str_surf = surface_create(width,height)
paragraph = 1;

halign = fa_left;
size = 10;
bold = false;
italic = false;
font = {fontVar}
colorVal = make_color_rgb({c.R},{c.G},{c.B});

altString = false;
paragraphs = [ """",
{paragraphs}]
str = paragraphs[paragraph]
image_alpha = 1.0 - ({obj.Header.BlendCoeff}/255.0);";
                        GMLFiles.Add(createEvFile);

                        string drawcode =
@"if (paragraph > 0){
	altString = false;
	str = paragraphs[paragraph];
}
if (paragraph <= 0 && altString){
	str = paragraphs[paragraph]
} 

draw_set_font(font);
draw_set_halign(halign);
draw_set_valign(fa_top);
draw_set_color(colorVal);

var drawX = 0;
switch(draw_get_halign())
{
	case fa_center:
		drawX = width/2;
	break;
	case fa_right:
		drawX = width;
	break;
}
draw_text_ext(x + drawX, y, string(paragraphs[paragraph]),0,width);

draw_set_color(c_white);

if (flash[1] > 0) {
	flash[0] += delta_time/1000000.0
	if (flash[0] >= flash[1]) {
		flash[0] = 0;
		visible = !visible;
	}
}";
                        /*string drawcode =     /// FOR STRINGS WITH CUT-OFF BORDERS
@"if (paragraph > 0){
	altString = false;
	str = paragraphs[paragraph];
}
if (paragraph <= 0 && altString){
	str = paragraphs[paragraph]
} 
if surface_exists(str_surf){ // Save from non-existent surface
surface_set_target(str_surf);
draw_set_font(font);
draw_set_halign(halign);
draw_set_valign(fa_top);
draw_set_color(colorVal);

var drawX = 0;
switch(draw_get_halign())
{
	case fa_center:
		drawX = width/2;
	break;
	case fa_right:
		drawX = width;
	break;
}
draw_clear_alpha(0,0);
draw_text_ext(drawX, 0, string(paragraphs[paragraph]),0,width);

surface_reset_target();
draw_set_color(c_white);
draw_surface(str_surf,x,y);}
if (flash[1] > 0) {
	flash[0] += delta_time/1000000.0
	if (flash[0] >= flash[1]) {
		flash[0] = 0;
		visible = !visible;
	}
}";*/

                        // For Strings that don't follow the frame when scrolling
                        if (common.ObjectFlags["DontFollowFrame"])
                        {
                            var drawGuiEv = new ObjectYY.Event();
                            drawGuiEv.eventNum = 64;
                            drawGuiEv.eventType = 8;
                            events.Add(drawGuiEv);

                            var drawGuiEvFile = new ObjectYY.GMLFile();
                            drawGuiEvFile.name = "Draw_64";
                            drawGuiEvFile.path = $"objects\\{newObj.name}";
                            drawGuiEvFile.code = drawcode;
                            GMLFiles.Add(drawGuiEvFile);
                            //
                            var drawEv = new ObjectYY.Event();
                            drawEv.eventNum = 0;
                            drawEv.eventType = 8;
                            events.Add(drawEv);

                            var drawEvFile = new ObjectYY.GMLFile();
                            drawEvFile.name = "Draw_0";
                            drawEvFile.path = $"objects\\{newObj.name}";
                            drawEvFile.code = "/// @description LEAVE EMPTY";
                            GMLFiles.Add(drawEvFile);
                        }
                        else
                        {
                            var drawEv = new ObjectYY.Event();
                            drawEv.eventNum = 0;
                            drawEv.eventType = 8;
                            events.Add(drawEv);

                            var drawEvFile = new ObjectYY.GMLFile();
                            drawEvFile.name = "Draw_0";
                            drawEvFile.path = $"objects\\{newObj.name}";
                            drawEvFile.code = drawcode;
                            GMLFiles.Add(drawEvFile);
                        }

                        newObj.eventList = events.ToArray();

                        Objects.Add(newObj);

                        var newObjectRes = new ProjectYYP.Resource();
                        var newObjectResID = new ProjectYYP.ResourceID();

                        newObjectResID.name = newObj.name;
                        newObjectResID.path = $"objects/{newObj.name}/{newObj.name}.yy";
                        newObjectRes.id = newObjectResID;
                        newObjectRes.order = ObjectOrder;
                        ObjectOrder++;
                        Resources.Add(newObjectRes);
                        continue;
                    }

                    // Actives/Common Objs objects
                    if (obj.Header.Type == 2 || obj.Header.Type > 7)
                    {
                        Logger.LogType(typeof(GMS2Writer), $"Common Object: {obj.Name}|{common.Identifier}");
                        
                        if (common.ObjectAnimations.Animations.Count == 0 ||
                            (common.Identifier != "SPRI" && common.Identifier != "SP"))
                        //if (common.Animations == null || (common.Identifier != "SPRI" && common.Identifier != "SP"))
                        { // VVVVV For any CommonObjects that are unimplemented (i.e. Physics - Engine) pretty much only to stop missing resource complaints from GMS2
                            var newCommonObj = new ObjectYY.RootObject();
                            newCommonObj.name = CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
                            newCommonObj.name = GMS2Writer.ObjectName(obj);
                            newCommonObj.visible = false;

                            /*
                                EXTENSIONS
                            */
                            switch (common.Identifier.ToLower())
                            {
                                case "omfp": // Platform Movement object
                                    var pfmo = new PFMO();
                                    pfmo.Read(obj, newCommonObj, GMLFiles);
                                    break;
                                case "2yoj": // Joystick 2 object
                                    var joy2 = new Joystick2();
                                    joy2.Read(obj, newCommonObj, GMLFiles);
                                    break;
                                case "0i": // Ini object
                                case "0ini":
                                    var ini = new KcIni();
                                    ini.Read(obj, newCommonObj, GMLFiles);
                                    break;
                                case "tksp": // Perspective object
                                    break;
                                    /*
                                    case "0I":
                                    case "0INI":
                                        var ini = new Ini();
                                        ini.Read(common, newCommonObj, GMLFiles);
                                        break;
                                    */
                            }

                            Objects.Add(newCommonObj);

                            var newCommonObjectRes = new ProjectYYP.Resource();
                            var newCommonObjectResID = new ProjectYYP.ResourceID();

                            newCommonObjectResID.name = newCommonObj.name;
                            newCommonObjectResID.path = $"objects/{newCommonObj.name}/{newCommonObj.name}.yy";
                            newCommonObjectRes.id = newCommonObjectResID;
                            newCommonObjectRes.order = ObjectOrder;
                            ObjectOrder++;
                            Resources.Add(newCommonObjectRes);
                            continue;
                        }
                        /*
                            ACTIVES
                        */
                        var newObj = new ObjectYY.RootObject();
                        newObj.name = CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
                        newObj.name = GMS2Writer.ObjectName(obj);
                        var events = new List<ObjectYY.Event>();

                        //common.Movements
                        var createEv = new ObjectYY.Event();
                        createEv.eventNum = 0;
                        createEv.eventType = 0;
                        events.Add(createEv);

                        var createEvFile = new ObjectYY.GMLFile();
                        createEvFile.name = "Create_0";
                        createEvFile.path = $"objects\\{newObj.name}";

                        // For Objects that don't follow the frame when scrolling
                        if (common.ObjectFlags["DontFollowFrame"])
                        {
                            var drawGuiEv = new ObjectYY.Event();
                            drawGuiEv.eventNum = 64;
                            drawGuiEv.eventType = 8;
                            events.Add(drawGuiEv);

                            var drawGuiEvFile = new ObjectYY.GMLFile();
                            drawGuiEvFile.name = "Draw_64";
                            drawGuiEvFile.path = $"objects\\{newObj.name}";
                            drawGuiEvFile.code = "if sprite_get_name(sprite_index) != \"<undefined>\" then draw_self();";
                            GMLFiles.Add(drawGuiEvFile);

                            var drawEv = new ObjectYY.Event();
                            drawEv.eventNum = 0;
                            drawEv.eventType = 8;
                            events.Add(drawEv);

                            var drawEvFile = new ObjectYY.GMLFile();
                            drawEvFile.name = "Draw_0";
                            drawEvFile.path = $"objects\\{newObj.name}";
                            drawEvFile.code = "/// @description LEAVE EMPTY";
                            GMLFiles.Add(drawEvFile);
                        }

                        WriteValues(obj, createEvFile);
                        WriteMovement(common, createEvFile);

                        // Animations
                        var beginstepEvFile = new ObjectYY.GMLFile();
                        beginstepEvFile.name = "Step_1";
                        beginstepEvFile.path = $"objects\\{newObj.name}";

                        var beginstepEv = new ObjectYY.Event();
                        beginstepEv.eventNum = 1;
                        beginstepEv.eventType = 3;

                        WriteAnimations(common, newObj.name, createEvFile, beginstepEvFile);

                        events.Add(beginstepEv);
                        GMLFiles.Add(beginstepEvFile);

                        GMLFiles.Add(createEvFile);
                        newObj.eventList = events.ToArray();
                        int[] imgs = null;
                        int animdict1 = 0;
                        int dirdict1 = 0;

                        // Find some kind of valid animdict/dirdict values for setting the object's sprite index name
                        foreach (int ad in common.ObjectAnimations.Animations.Keys)
                        {
                            if (common.ObjectAnimations.Animations[ad].Directions.Count > 0)
                            {
                                foreach (ObjectDirection direction in common.ObjectAnimations.Animations[ad].Directions)
                                {
                                    if (direction.Frames.Length > 0)
                                    {
                                        animdict1 = ad;
                                        dirdict1 = direction.Index;
                                        imgs = Array.ConvertAll(direction.Frames, item => (int)item);
                                        break;
                                    }
                                }
                                break;
                            }
                        }

                        newObj.visible = common.NewObjectFlags["VisibleAtStart"];
                        newObj.spriteId.name = $"spr_{newObj.name.Substring(4)}_{imgs[0]}_{animdict1}_{dirdict1}";
                        newObj.spriteId.path = $"sprites/{newObj.spriteId.name}/{newObj.spriteId.name}.yy";

                        Objects.Add(newObj);

                        var newObjectRes = new ProjectYYP.Resource();
                        var newObjectResID = new ProjectYYP.ResourceID();

                        newObjectResID.name = newObj.name;
                        newObjectResID.path = $"objects/{newObj.name}/{newObj.name}.yy";
                        newObjectRes.id = newObjectResID;
                        newObjectRes.order = ObjectOrder;
                        ObjectOrder++;
                        Resources.Add(newObjectRes);
                        continue;
                    }
                }
                // Backdrops objects
                if (obj.Properties is ObjectBackdrop backdrop)
                {

                    var imgs = new List<int>() { (int)backdrop.Image };

                    var newObj = new ObjectYY.RootObject();
                    newObj.name = CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
                    newObj.name = GMS2Writer.ObjectName(obj);
                    newObj.visible = true;
                    newObj.spriteId.name = $"Backdrop_{imgs[0]}";
                    newObj.spriteId.path = $"sprites/{newObj.spriteId.name}/{newObj.spriteId.name}.yy";

                    /* OBSTACLE TYPES
                     * None = 0
                     * Solid = 1
                     * Platform = 2
                     * Ladder = 3
                     * Transparent = 4
                     */
                    /* COLLISION TYPES
                     * Fine = 0
                     * Box = 1
                     */
                    var events = new List<ObjectYY.Event>();

                    var createEv = new ObjectYY.Event();
                    createEv.eventNum = 0;
                    createEv.eventType = 0;
                    events.Add(createEv);

                    var createEvFile = new ObjectYY.GMLFile();
                    createEvFile.name = "Create_0";
                    createEvFile.path = $"objects\\{newObj.name}";
                    createEvFile.code = "";
                    if (backdrop.ObstacleType == 1) // Solid obstacle backdrop
                    {
                        var obstacleParent = new ObjectYY.ParentObjectID();
                        obstacleParent.name = "Backdrop_Obstacle";
                        obstacleParent.path = "objects/Backdrop_Obstacle/Backdrop_Obstacle.yy";
                        newObj.parentObjectId = obstacleParent;
                        createEvFile.code = $"IsObstacle = true;";
                    }
                    createEvFile.code += $"\nimage_alpha = 1.0 - ({obj.Header.BlendCoeff}/255.0);";

                    GMLFiles.Add(createEvFile);
                    newObj.eventList = events.ToArray();

                    Objects.Add(newObj);

                    var newObjectRes = new ProjectYYP.Resource();
                    var newObjectResID = new ProjectYYP.ResourceID();

                    newObjectResID.name = newObj.name;
                    newObjectResID.path = $"objects/{newObj.name}/{newObj.name}.yy";
                    newObjectRes.id = newObjectResID;
                    newObjectRes.order = ObjectOrder;
                    ObjectOrder++;
                    Resources.Add(newObjectRes);
                    continue;
                }
                // Quick Backdrop objects
                if (obj.Properties is ObjectQuickBackdrop quickbackdrop)
                {

                    var imgs = new List<int>() { (int)quickbackdrop.Shape.Image };

                    var newObj = new ObjectYY.RootObject();
                    newObj.name = CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
                    newObj.name = GMS2Writer.ObjectName(obj);
                    newObj.visible = true;
                    newObj.spriteId.name = $"QuickBackdrop_{imgs[0]}";
                    newObj.spriteId.path = $"sprites/{newObj.spriteId.name}/{newObj.spriteId.name}.yy";

                    var events = new List<ObjectYY.Event>();
                    var createEv = new ObjectYY.Event();
                    var createEvFile = new ObjectYY.GMLFile();

                    createEv.eventNum = 0;
                    createEv.eventType = 0;
                    events.Add(createEv);

                    createEvFile.name = "Create_0";
                    createEvFile.path = $"objects\\{newObj.name}";
                    createEvFile.code = "";
                    if (quickbackdrop.Shape.FillType == 1)
                    {
                        createEvFile.code = $"qbdCol = make_colour_rgb({quickbackdrop.Shape.Color1.R}, {quickbackdrop.Shape.Color1.G}, {quickbackdrop.Shape.Color1.B});";

                        var drawEv = new ObjectYY.Event();
                        drawEv.eventNum = 0;
                        drawEv.eventType = 8;
                        events.Add(drawEv);

                        var drawEvFile = new ObjectYY.GMLFile();
                        drawEvFile.name = "Draw_0";
                        drawEvFile.path = $"objects\\{newObj.name}";
                        drawEvFile.code = $"draw_rectangle_colour(x, y, x + {quickbackdrop.Width} - 1, y + {quickbackdrop.Height} - 1, qbdCol, qbdCol, qbdCol, qbdCol, false);";
                        GMLFiles.Add(drawEvFile);


                    }
                    if (quickbackdrop.ObstacleType == 1) // Solid obstacle backdrop
                    {
                        var obstacleParent = new ObjectYY.ParentObjectID();
                        obstacleParent.name = "Backdrop_Obstacle";
                        obstacleParent.path = "objects/Backdrop_Obstacle/Backdrop_Obstacle.yy";
                        newObj.parentObjectId = obstacleParent;

                        var roomstartEv = new ObjectYY.Event();
                        roomstartEv.eventNum = 4;
                        roomstartEv.eventType = 7;
                        events.Add(roomstartEv);

                        var roomstartEvFile = new ObjectYY.GMLFile();
                        roomstartEvFile.name = "Other_4";
                        roomstartEvFile.path = $"objects\\{newObj.name}";
                        roomstartEvFile.code = $"IsObstacle = true;";
                        GMLFiles.Add(roomstartEvFile);

                    }
                    createEvFile.code += $"\nimage_alpha = 1.0 - ({obj.Header.BlendCoeff}/255.0);";
                    GMLFiles.Add(createEvFile);
                    newObj.eventList = events.ToArray();


                    Objects.Add(newObj);

                    var newObjectRes = new ProjectYYP.Resource();
                    var newObjectResID = new ProjectYYP.ResourceID();

                    newObjectResID.name = newObj.name;
                    newObjectResID.path = $"objects/{newObj.name}/{newObj.name}.yy";
                    newObjectRes.id = newObjectResID;
                    newObjectRes.order = ObjectOrder;
                    ObjectOrder++;
                    Resources.Add(newObjectRes);
                    continue;
                }
            }

            // Sounds (come back to this and rework)
            var Sounds = new List<SoundYY.RootObject>();
            if (gameData.SoundBank.Count > 0)
            {
                foreach (var soundItem in gameData.SoundBank.Sounds.Values)
                {
                    Logger.LogType(typeof(GMS2Writer), $"Sound: {soundItem.Name}");

                    // Determine audio file type
                    var ext = ".wav";
                    if (soundItem.Data.Length > 0)
                        if (soundItem.Data[0] == 0xff || soundItem.Data[0] == 0x49)
                            ext = ".mp3";
                        else if (soundItem.Data[0] == 0x4F)
                            ext = ".ogg";

                    var newSound = new SoundYY.RootObject();
                    newSound.name = "snd_" + CleanStringFull(soundItem.Name);
                    //newSound.name = SoundName(soundItem);
                    newSound.soundFile = newSound.name + ext;

                    newSound.volume = 1.0f;
                    newSound.sampleRate = 44100; // (FIX/REWORK maybe make this an argument/option?)
                    if (soundItem.Frequency > 0) newSound.sampleRate = soundItem.Frequency;
                    

                    Sounds.Add(newSound);

                    var newSoundRes = new ProjectYYP.Resource();
                    var newSoundResID = new ProjectYYP.ResourceID();

                    newSoundResID.name = newSound.name;
                    newSoundResID.path = $"sounds/{newSound.name}/{newSound.name}.yy";
                    newSoundRes.id = newSoundResID;
                    newSoundRes.order = SoundOrder;
                    SoundOrder++;
                    Resources.Add(newSoundRes);
                }
            }

            Logger.LogType(typeof(GMS2Writer), "Writing Frame Events (unfinished)");
            List<string> missing_code = new List<string>();
            try
            {
                EventsToGML.Write(GMLFiles, Resources, Objects, gameData, Rooms, ref missing_code);
            }
            catch (Exception ex)
            {
                Logger.LogType(typeof(GMS2Writer), $"Problem trying to write Frame Events: {ex}");
            }
            missing_code = missing_code.Distinct().ToList();
            missing_code.Sort();

            ProjectJSON.AudioGroups = new ProjectYYP.AudioGroup[1];
            ProjectJSON.TextureGroups = new ProjectYYP.TextureGroup[1];
            ProjectJSON.AudioGroups[0] = new ProjectYYP.AudioGroup();
            ProjectJSON.TextureGroups[0] = new ProjectYYP.TextureGroup();

            ProjectJSON.RoomOrderNodes = RoomOrderNodes.ToArray();
            ProjectJSON.resources = Resources.ToArray();
            RoomOrderNodes.Clear();
            Resources.Clear();
            FrameOrder = 0;
            try
            {
                WriteToFile(outPath, outName, gameData, ProjectJSON, Rooms, Sprites, Objects, Sounds, GMLFiles, fontsToWrite, missing_code);
                Logger.LogType(typeof(GMS2Writer), $"GMS2 Project File created at: {outPath}");
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outPath);
                Process.Start("explorer.exe", fullPath);
            }
            catch (Exception ex)
            {
                Logger.LogType(typeof(GMS2Writer), $"Problem trying to write Project File: {ex}");
            }
        }

        public static void WriteToFile(string outPath,
                                         string outName,
                                    PackageData gameData,
                          ProjectYYP.RootObject ProjectJSON,
                        List<RoomYY.RootObject> RoomJSONs,
                      List<SpriteYY.RootObject> SpriteJSONs,
                      List<ObjectYY.RootObject> ObjectJSONs,
                       List<SoundYY.RootObject> SoundJSONs,
                         List<ObjectYY.GMLFile> gmlFiles,
                     Dictionary<string, string> fontsToWrite,
                                   List<string> missing_code)
        {
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);

            Logger.LogType(typeof(GMS2Writer), "Writing YYP/Project File");
            var WriteProjectJSON = JsonConvert.SerializeObject(ProjectJSON);
            Directory.CreateDirectory(outPath);
            File.WriteAllText($"{outPath}\\{outName}.yyp", WriteProjectJSON);

            #region datafiles
            string AssetsDir = $"{outPath}\\datafiles\\Assets";
            Directory.CreateDirectory(AssetsDir);

            // Write fonts
            Directory.CreateDirectory($"{AssetsDir}\\Fonts");
            foreach (string fontPath in fontsToWrite.Values)
            {
                string dest = $"{AssetsDir}\\Fonts\\{Path.GetFileName(fontPath)}";
                Logger.LogType(typeof(GMS2Writer), fontPath + " ; " + dest);
                File.Copy(fontPath, dest, true);
            }
            #endregion

            Task[] tasks = new Task[RoomJSONs.Count];
            int i = 0;
            Logger.LogType(typeof(GMS2Writer), "Writing rooms");
            foreach (var room in RoomJSONs)
            {
                var newTask = new Task(() =>
                {
                    var WriteRoomJSON = JsonConvert.SerializeObject(room);
                    Directory.CreateDirectory($"{outPath}\\rooms\\{room.name}");
                    File.WriteAllText($"{outPath}\\rooms\\{room.name}\\{room.name}.yy", WriteRoomJSON);
                });
                tasks[i] = newTask;
                newTask.Start();
                i++;
            }
            foreach (var item in tasks)
            {
                item.Wait();
            }


            Logger.LogType(typeof(GMS2Writer), "Writing sprites (may take a while)");
            foreach (var spr in SpriteJSONs)
            {
                foreach (var frame in spr.frames)
                {
                    
                RETRY_SAVE:
                    Directory.CreateDirectory($"{outPath}\\sprites\\{spr.name}\\layers\\{frame.name}");
                    try
                    {
                        string layerFramePath = $"{outPath}\\sprites\\{spr.name}\\layers\\{frame.name}\\{spr.layers[0].name}.png";
                        string framePath = $"{outPath}\\sprites\\{spr.name}\\{frame.name}.png";
                        if (!File.Exists(layerFramePath)) gameData.ImageBank.Images[(uint)frame.ctfhandle].GetBitmap().Save(layerFramePath);
                        if (!File.Exists(framePath)) gameData.ImageBank.Images[(uint)frame.ctfhandle].GetBitmap().Save(framePath);

                    }
                    catch (Exception ex)
                    {
                        Logger.LogType(typeof(GMS2Writer), $"Problem saving image: {ex}");
                        goto RETRY_SAVE;
                    }

                RETRY_SAVE_YY:
                    try
                    {
                        var WriteSpriteJSON = JsonConvert.SerializeObject(spr).Replace("ZEROREPLACE", "0");
                        if (!File.Exists($"{outPath}\\sprites\\{spr.name}\\{spr.name}.yy")) File.WriteAllText($"{outPath}\\sprites\\{spr.name}\\{spr.name}.yy", WriteSpriteJSON);
                    }
                    catch
                    {
                        goto RETRY_SAVE_YY;
                    }
                }
            }


            // Write sounds
            if (!Args["-nosnds"]) {
                tasks = new Task[SoundJSONs.Count];
                i = 0;
                Logger.LogType(typeof(GMS2Writer), "Writing sounds (may take a while)");
                foreach (var snd in SoundJSONs)
                {
                    var newTask = new Task(() =>
                    {
                    RETRY_SAVE:
                        Directory.CreateDirectory($"{outPath}\\sounds\\{snd.name}");
                        try
                        {
                            byte[] WriteSoundData = new byte[0];
                            foreach (var item in gameData.SoundBank.Sounds.Values)
                            {
                                if ("snd_" + CleanStringFull(item.Name) == snd.name)

                                {
                                    WriteSoundData = item.Data;
                                }
                            }
                            File.WriteAllBytes($"{outPath}\\sounds\\{snd.name}\\{snd.soundFile}", WriteSoundData);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogType(typeof(GMS2Writer), $"Problem writing sound file: {ex}");
                            //goto RETRY_SAVE;
                        }
                    RETRY_SAVE_YY:
                        try
                        {
                            var WriteSoundJSON = JsonConvert.SerializeObject(snd);
                            File.WriteAllText($"{outPath}\\sounds\\{snd.name}\\{snd.name}.yy", WriteSoundJSON);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogType(typeof(GMS2Writer), $"Problem writing sound YY: {ex}");
                            //goto RETRY_SAVE_YY;
                        }
                    });
                    tasks[i] = newTask;
                    newTask.Start();
                    i++;
                }
                foreach (var item in tasks)
                {
                    item.Wait();
                }
            }

            // Write Objects
            tasks = new Task[ObjectJSONs.Count];
            i = 0;
            Logger.LogType(typeof(GMS2Writer), "Writing objects");
            foreach (var obj in ObjectJSONs)
            {
                var newTask = new Task(() =>
                {
                    var WriteObjJSON = JsonConvert.SerializeObject(obj);
                    Directory.CreateDirectory($"{outPath}\\objects\\{obj.name}");
                    File.WriteAllText($"{outPath}\\objects\\{obj.name}\\{obj.name}.yy", WriteObjJSON);
                });
                tasks[i] = newTask;
                newTask.Start();
                i++;
            }
            foreach (var item in tasks)
            {
                item.Wait();
            }

            tasks = new Task[gmlFiles.Count];
            i = 0;
            Logger.LogType(typeof(GMS2Writer), "Writing GML files");
            if (missing_code.Count > 0) File.WriteAllLines($"{outPath}\\unimplemented.txt", missing_code);
            foreach (var obj in gmlFiles)
            {
                var newTask = new Task(() =>
                {
                RETRY_SAVE_GML:
                    Directory.CreateDirectory($"{outPath}\\{obj.path}");
                    try
                    {
                        if (obj.name == "ZelCTF")
                        {
                            var zelCtfScript = new ScriptYY.RootObject();
                            zelCtfScript.name = "ZelCTF";
                            var WriteScriptJSON = JsonConvert.SerializeObject(zelCtfScript);
                            Directory.CreateDirectory($"{outPath}\\objects\\{obj.name}");
                            File.WriteAllText($"{outPath}\\scripts\\{zelCtfScript.name}\\{zelCtfScript.name}.yy", WriteScriptJSON);
                        }
                        File.WriteAllText($"{outPath}\\{obj.path}\\{obj.name}.gml", obj.code);
                        //Logger.Log(typeof(GMS2Writer), $"Wrote GML to {outPath}\\{obj.path}\\{obj.name}.gml");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogType(typeof(GMS2Writer), $"Problem writing GML: {ex}");
                        goto RETRY_SAVE_GML;
                    }

                });
                tasks[i] = newTask;
                newTask.Start();
                i++;
            }
            foreach (var item in tasks)
            {
                item.Wait();
            }

        }

        public static void WriteValues(ObjectInfo obj, GMLFile createEvFile)
        {
            ObjectCommon common = obj.Properties as ObjectCommon;

            int maxValues = 260; // 2.5+ DLC allows up to 260 Alterables

            // Object name
            createEvFile.code = $"object_name = \"{obj.Name}\";\n";

            // Alterable Values (fix/rework to exclude flags)
            createEvFile.code += $"AlterableValues = [";
            int[] values = common.ObjectAlterableValues.AlterableValues;
            try
            {
                for (var i = 0; i < Math.Max(maxValues, values.Length); i++)
                {
                    if (i < values.Length) createEvFile.code += $"{values[i]}";
                    if (i >= values.Length) createEvFile.code += $"0";
                    if (i < Math.Max(maxValues, values.Length) - 1) createEvFile.code += ",";
                }
            }
            catch
            {
                for (var i = 0; i < maxValues; i++)
                {
                    if (i < maxValues - 1) createEvFile.code += "0,";
                    else if (i + 1 == maxValues) createEvFile.code += "0";
                }
            }
            createEvFile.code += $"];\n";

            // Alterable Strings
            createEvFile.code += $"AlterableStrings = [";
            string[] strings = common.ObjectAlterableStrings.AlterableStrings;
            try
            {
                for (var i = 0; i < Math.Max(maxValues, strings.Length); i++)
                {
                    if (i < strings.Length) createEvFile.code += $"\"{strings[i]}\"";
                    if (i >= strings.Length) createEvFile.code += $"\"\"";
                    if (i < Math.Max(maxValues, strings.Length) - 1) createEvFile.code += ",";
                }
            }
            catch
            {
                for (var i = 0; i < maxValues; i++)
                {
                    if (i < maxValues - 1) createEvFile.code += "\"\",";
                    else if (i + 1 == maxValues) createEvFile.code += "\"\"";
                }
            }
            createEvFile.code += $"];\n";

            // Flags (fix/rework to ACTUALLY include predefined user flag values, idfk how to obtain them properly)
            createEvFile.code += $"Flags = [";
            for (var i = 0; i < maxValues; i++)
            {
                if (i < maxValues - 1) createEvFile.code += "false,";
                else if (i + 1 == maxValues) createEvFile.code += "false";
            }
            createEvFile.code += $"];\n";
            createEvFile.code += $"flash = [ 0, 0 ];\nimage_alpha = 1.0 - ({obj.Header.BlendCoeff}/255.0);";
        }
        public static void WriteMovementOld(ObjectCommon common, GMLFile createEvFile)
        {
            int mvmtCount = 0;
            try
            {
                foreach (ObjectMovement movement in common.ObjectMovements.Movements)
                {
                    int initialdir = movement.StartingDirection;
                    bool moveatstart = Convert.ToBoolean(movement.Move);
                    Logger.LogType(typeof(GMS2Writer), $"Movement: {movement.MovementDefinition.GetType().Name} - Dir@Start={initialdir}");
                    string movementType = "";
                    short player = movement.Player;
                    bool moving = false;

                    int allowedDirs = 0;
                    string dir = "-1";

                    int spd = 0;
                    int accel = 0;
                    int decel = 0;

                    bool stickobstacles = false;

                    var numOfAngles = 32;
                    var randomizer = 20;
                    var security = 60;

                    int gravity = 50;
                    int strength = 50;
                    string control = "undefined";

                    if (movement.MovementDefinition is ObjectMovementBall ball)
                    {
                        movementType = "ball";
                        moving = moveatstart;
                        dir = $"{initialdir}";
                        spd = ball.Speed;
                        decel = ball.Decelerate;
                        numOfAngles = ball.Angles;
                        randomizer = ball.Bounce;
                        security = ball.Security;
                    }
                    if (movement.MovementDefinition is ObjectMovementGeneric eightdirs)
                    {
                        movementType = "eightdirections";
                        Logger.LogType(typeof(GMS2Writer), $"eightdirs.Directions = {eightdirs.Direction} eightdirs.player = ");
                        moving = Convert.ToBoolean(moveatstart);
                        allowedDirs = eightdirs.Direction;
                        dir = $"{initialdir}";
                        spd = eightdirs.Speed;
                        decel = eightdirs.Deceleration;
                        accel = eightdirs.Acceleration;
                    }
                    if (movement.MovementDefinition is ObjectMovementPlatform platform)
                    {
                        movementType = "platform";
                        moving = Convert.ToBoolean(moveatstart);
                        dir = $"{initialdir}";
                        spd = platform.Speed;
                        accel = platform.Acceleration;
                        decel = platform.Deceleration;

                        gravity = platform.Gravity;
                        strength = platform.Jump;
                        if (platform.JumpControl == 1) control = "vk_up";
                        if (platform.JumpControl == 2) control = "vk_shift";
                        if (platform.JumpControl == 3) control = "vk_control";
                    }
                    if (movement.MovementDefinition is ObjectMovementPath path)
                    {
                        movementType = "path";
                        moving = Convert.ToBoolean(moveatstart);
                        var Nodes = path.PathNodes;
                        foreach (var node in Nodes )
                        {
                            //var test = node.
                        }
                        // Unimplemented
                    }
                    createEvFile.code += $"\nMovement{mvmtCount} = {{";

                    createEvFile.code += $"\n\ttype : \"{movementType}\",";
                    createEvFile.code += $"\n\tplayer : {player},";
                    createEvFile.code += $"\n\tmoving : {moving.ToString().ToLower()},";
                    createEvFile.code += $"\n\tspd : {spd},";
                    createEvFile.code += $"\n\taccel : {accel},";
                    createEvFile.code += $"\n\tdecel : {decel},";
                    createEvFile.code += $"\n\tallowedDirs : {allowedDirs},";
                    createEvFile.code += $"\n\tdir : {dir},";
                    createEvFile.code += $"\n\tstickobstacles : {stickobstacles.ToString().ToLower()},";
                    createEvFile.code += $"\n\tnumOfAngles : {numOfAngles},";
                    createEvFile.code += $"\n\trandomizer : {randomizer},";
                    createEvFile.code += $"\n\tsecurity : {security},";
                    createEvFile.code += $"\n\tgrav : {gravity},";
                    createEvFile.code += $"\n\tjumpstrength : {strength},";
                    createEvFile.code += $"\n\tjumpbtn : {control}";

                    createEvFile.code += " };\n";
                    mvmtCount++;
                }
                createEvFile.code += "\nMovements = [";
                for (int i = 0; i < mvmtCount; i++)
                {
                    createEvFile.code += $"Movement{i}";
                    if (i < mvmtCount - 1) createEvFile.code += ",";
                }
                createEvFile.code += "];\n";
                createEvFile.code += $"Movement = Movements[0]";
            }
            catch (Exception ex)
            {
                Logger.LogType(typeof(GMS2Writer), $"Unable to write object movement: {ex}");
            }
        }
        public static void WriteMovement(ObjectCommon common, GMLFile createEvFile)
        {
            try
            {
                int mvmtCount = 0;
                createEvFile.code += "\nmovement = 0;";
                createEvFile.code += "\nmovements = [\n";

                string startSpd = "";
                string startDir = "";
                foreach (ObjectMovement movement in common.ObjectMovements.Movements)
                {
                    //createEvFile.code += "{";

                    int initialdir = movement.StartingDirection;
                    bool moveatstart = Convert.ToBoolean(movement.Move);
                    Logger.LogType(typeof(GMS2Writer), $"Movement: {movement.MovementDefinition.GetType().Name} - Dir@Start={initialdir}");
                    string movementType = "";
                    short player = movement.Player;
                    bool moving = false;

                    int allowedDirs = 0;
                    string dir = "-1";

                    int spd = 0;
                    int accel = 0;
                    int decel = 0;

                    bool stickobstacles = false;

                    var numOfAngles = 32;
                    var randomizer = 20;
                    var security = 60;

                    int gravity = 50;
                    int strength = 50;
                    string control = "undefined";

                    string MvtEntry = "";
                    startSpd = "speed = 0;";
                    startDir = "direction = 0;";
                    if (movement.MovementDefinition is ObjectMovementStatic stopped)
                    {
MvtEntry = @$"{{
mvtName : ""{movement.Name}"",
typeName : ""CMoveStatic"",
dir : {stopped.StartingDirection}
}},";
                        if (mvmtCount == 0)
                        {
                            startDir = "set_direction_int(movements[0].dir);\n";
                        }
                    }
                    if (movement.MovementDefinition is ObjectMovementBall ball)
                    {
                        int[] noa = { 8, 16, 32 };
MvtEntry = @$"{{
mvtName : ""{movement.Name}"",
typeName : ""CMoveBall"",
maxSpd : {ball.Speed},
spd : {ball.Speed} * {moveatstart.ToString().ToLower()},
deceleration : {ball.Decelerate},
dir : {ball.StartingDirection},
numOfAngles : {noa[ball.Angles]},
randomizer : {ball.Bounce},
security : {ball.Security}
}},";
                        if (mvmtCount == 0)
                        {
                            startSpd = "speed = get_pixels(movements[0].spd) * dt();\n";
                            startDir = "set_direction_int(movements[0].dir);\n";
                        }
                    }
                    if (movement.MovementDefinition is ObjectMovementGeneric eightdirs && false)
                    {
                        movementType = "CMoveGeneric";
                        Logger.LogType(typeof(GMS2Writer), $"eightdirs.Directions = {eightdirs.Direction} eightdirs.player = ");
                        moving = Convert.ToBoolean(moveatstart);
                        allowedDirs = eightdirs.Direction;
                        dir = $"{initialdir}";
                        spd = eightdirs.Speed;
                        decel = eightdirs.Deceleration;
                        accel = eightdirs.Acceleration;
MvtEntry = @$"{{
mvtName : ""{movement.Name}"",
typeName : ""CMoveGeneric"",
maxSpd : {eightdirs.Speed},
spd : 0,
deceleration : {eightdirs.Deceleration},
acceleration : {eightdirs.Acceleration},
dir : {eightdirs.StartingDirection},
player: {movement.Player}
}},";
                        if (mvmtCount == 0) startDir = "set_direction_int(movements[0].dir);\n";
                    }
                    if (movement.MovementDefinition is ObjectMovementPlatform platform && false)
                    {
                        movementType = "CMovePlatform";
                        moving = Convert.ToBoolean(moveatstart);
                        dir = $"{initialdir}";
                        spd = platform.Speed;
                        accel = platform.Acceleration;
                        decel = platform.Deceleration;

                        gravity = platform.Gravity;
                        strength = platform.Jump;
                        if (platform.JumpControl == 1) control = "vk_up";
                        if (platform.JumpControl == 2) control = "vk_shift";
                        if (platform.JumpControl == 3) control = "vk_control";
                    }
                    if (movement.MovementDefinition is ObjectMovementPath path)
                    {
                        movementType = "CMovePath";
                        moving = Convert.ToBoolean(moveatstart);
                        string pathNodes = "";
                        foreach (ObjectMovementPathNode node in path.PathNodes)
                        {
                            pathNodes += "{";
                            pathNodes += $"spd:{node.Speed}, ";
                            pathNodes += $"dir:{node.Direction}, ";
                            pathNodes += $"dx:{node.Dx}, ";
                            pathNodes += $"dy:{node.Dy}, ";
                            pathNodes += $"startX:-5552471, ";
                            pathNodes += $"startY:-5552471, ";
                            //pathNodes += $"cos:{node.Cos}, ";
                            //pathNodes += $"sin:{node.Sin}, ";
                            //pathNodes += $"length:{node.Length}, ";
                            pathNodes += $"reached:false, ";
                            
                            pathNodes += $"pause:{node.Pause}, ";
                            pathNodes += $"pauseTimer:{node.Pause},";
                            pathNodes += $"name:\"{node.Name}\"";
                            pathNodes += "},\n";
                        }
MvtEntry = @$"{{
mvtName : ""{movement.Name}"",
typeName : ""CMovePath"",
reverse : false,
reversing : false,
loop : {(int)path.Loop},
repos : {(int)path.Reposition},
posx : x,
posy: y,
node : 0,
nodes : [
{pathNodes}]
}},";
                    }
                    createEvFile.code += MvtEntry; // Append movement entry

                    mvmtCount++;
                }
                
                createEvFile.code += "];\n";
                createEvFile.code += startSpd + "\n";
                createEvFile.code += startDir;

            }
            catch (Exception ex)
            {
                Logger.LogType(typeof(GMS2Writer), $"Unable to write object movement: {ex}");
            }
        }
        public static void WriteAnimations(ObjectCommon common, string ObjectName, GMLFile createEvFile, GMLFile beginstepEvFile)
        {
            try
            {
                createEvFile.code += "\nanimation = 0;\n";


                createEvFile.code += "animations = [\n";
                // Animations
                foreach (var animEntry in common.ObjectAnimations.Animations)
                {
                    var animation = animEntry.Value;
                    var animationID = animEntry.Key;


                    // Directions
                    if (animation.Directions.Count > 0)
                    {
                        createEvFile.code += "\t[\n\t\t[";
                        foreach (ObjectDirection direction in animation.Directions)
                        {
                            var directionID = direction.Index;

                            var imgs = direction.Frames;

                            if (imgs.Length > 0)
                            {
                                createEvFile.code += $"[spr_{ObjectName.Substring(4)}_{imgs[0]}_{animationID}_{directionID}, {directionID}], ";
                            }
                        }
                        createEvFile.code += $"], {animationID}";
                        createEvFile.code += "\n\t],\n";
                    }

                }
                createEvFile.code += "];";

                /*
                for (var i = 0; i < common.Animations.AnimationDict.Count; i++)
                {
                    createEvFile.code += "\t[\n\t\t";

                    for (var j = 0; j < common.Animations.AnimationDict[i].DirectionDict.Count; j++)
                    {
                        var imgs = common.Animations.AnimationDict[i].DirectionDict[j].Frames;
                        if (imgs.Count > 0)
                        {
                            createEvFile.code += "[";
                            createEvFile.code += $"spr_{ObjectName.Substring(4)}_{imgs[0]}_{i}_0"; // Sprite name
                            createEvFile.code += $", {j}]"; // Direction value

                            if (j != common.Animations.AnimationDict[i].DirectionDict.Count - 1) createEvFile.code += ", ";


                        }
                    }

                    createEvFile.code += "\n\t]";
                    if (i != common.Animations.AnimationDict[i].DirectionDict.Count - 1) createEvFile.code += ",";
                    createEvFile.code += "\n";
                }
                createEvFile.code += "];";
                */

                var asmbly = Assembly.GetExecutingAssembly();
                var filePath = $"ZelTranslator_SD.Parsers.GameMaker_Studio_2.Resources.Active_BeginStep.gml";
                using (Stream s = asmbly.GetManifestResourceStream(filePath))
                using (StreamReader sr = new StreamReader(s))
                {
                    beginstepEvFile.code = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Logger.LogType(typeof(GMS2Writer), $"Unable to write object animations: {ex}");
            }
        }
    }
}
