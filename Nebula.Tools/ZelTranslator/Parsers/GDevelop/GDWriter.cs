using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Nebula.Tools.ZelTranslator_SD.GDevelop
{
    public class GDWriter //: INebulaTool // Comment ': INebulaTool' out to disable
    {
        public static string AltCharacter(int index)
        {
            if (index >= 26)
                return ((char)('A' + ((index - index % 26) / 26 - 1))).ToString() + ((char)('A' + index % 26)).ToString();
            else
                return ((char)('A' + index)).ToString();
        }

        public static Dictionary<string, int> extnames = new()
        {
            { "Every", 0 }
        };

        public List<int> extensions = new();

        public string Name => "GDevelop Translator";

        public void Execute()
        {
            PackageData gameData = NebulaCore.PackageData;
            var outPath = gameData.AppName ?? "Unknown Game";
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            outPath = rgx.Replace(outPath, "").Trim(' ');
            outPath = $"Dumps\\{outPath}\\GDevelop";
            Directory.CreateDirectory(outPath);
            extensions = new();

            var JSONtoWrite = new GDJSON.Rootobject();
            JSONtoWrite.firstLayout = gameData.Frames[0].FrameName;

            var Properties = new GDJSON.Properties();
            Properties.packageName = $"com.CTFAK.{rgx.Replace(gameData.AppName, "").Replace(" ", "")}";
            Properties.name = gameData.AppName;
            Properties.author = gameData.Author;
            Properties.windowWidth = gameData.AppHeader.AppWidth;
            Properties.windowHeight = gameData.AppHeader.AppHeight;
            Properties.maxFPS = gameData.AppHeader.FrameRate;
            Properties.verticalSync = gameData.AppHeader.NewFlags["VSync"];
            Properties.platforms = new GDJSON.Platform[1];
            Properties.platforms[0] = new();

            var Resources = new GDJSON.Resources();
            var ListResources = new List<GDJSON.Resource>();
            if (gameData.ImageBank.Images != null)
            {
                Task[] tasks = new Task[gameData.ImageBank.Images.Count];
                int i = 0;
                foreach (Image img in gameData.ImageBank.Images.Values)
                {
                    var newTask = new Task(() =>
                    {
                        var bmp = img.GetBitmap();
                        bmp.Save($"{outPath}\\img{img.Handle}.png");

                        var res = new GDJSON.Resource();
                        res.alwaysLoaded = false;
                        res.file = $"img{img.Handle}.png";
                        res.kind = "image";
                        res.metadata = "";
                        res.name = $"img{img.Handle}.png";
                        res.smoothed = true;
                        res.userAdded = true;
                        ListResources.Add(res);
                    });
                    tasks[i] = newTask;
                    newTask.Start();
                    i++;
                }
                Task.WaitAll(tasks);
            }
            if (gameData.SoundBank != null)
            {
                Task[] tasks = new Task[gameData.SoundBank.Sounds.Count];
                int i = 0;
                foreach (Sound sound in gameData.SoundBank.Sounds.Values)
                {
                    var newTask = new Task(() =>
                    {
                        var snddata = sound.Data;
                        var sndext = ".wav";
                        if (snddata[0] == 0xff || snddata[0] == 0x49)
                            sndext = ".mp3";
                        File.WriteAllBytes($"{outPath}\\{sound.Name}{sndext}", snddata);

                        var res = new GDJSON.Resource();
                        res.file = sound.Name + sndext;
                        res.kind = "audio";
                        res.metadata = "";
                        res.name = sound.Name + sndext;
                        res.preloadAsMusic = false;
                        res.preloadAsSound = true;
                        res.preloadInCache = false;
                        res.userAdded = true;
                        ListResources.Add(res);
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
            Resources.resources = ListResources.ToArray();

            var Scenes = new List<GDJSON.Layout>();
            foreach (var frame in gameData.Frames)
            {
                var newScene = new GDJSON.Layout();
                if (frame.FrameName == "" || frame.FrameName == null) continue;
                newScene.mangledName = rgx.Replace(frame.FrameName, "").Replace(" ", "");
                newScene.name = frame.FrameName;
                var sceneLayers = new List<GDJSON.Layer>();
                // Base Layer
                var BaseLayer = new GDJSON.Layer();
                BaseLayer.ambientLightColorB = 0;
                BaseLayer.ambientLightColorG = 7988432;
                BaseLayer.ambientLightColorR = 16;
                BaseLayer.followBaseLayerCamera = false;
                BaseLayer.name = "";
                BaseLayer.visibility = true;
                BaseLayer.cameras = new GDJSON.Camera[1];
                BaseLayer.cameras[0] = new();
                sceneLayers.Add(BaseLayer);

                // Frame Layers
                foreach (var layer in frame.FrameLayers.Layers)
                {
                    var newLayer = new GDJSON.Layer();
                    newLayer.followBaseLayerCamera = layer.XCoefficient != 0 && layer.YCoefficient != 0;
                    newLayer.name = layer.Name;
                    newLayer.visibility = !layer.LayerFlags["HiddenAtStart"];
                    newLayer.cameras = new GDJSON.Camera[0];
                    sceneLayers.Add(newLayer);
                }
                var sceneObjects = new List<GDJSON.Object>();
                var sceneInstances = new List<GDJSON.Instance>();
                int z = 0;
                foreach (var obj in frame.FrameInstances.Instances)
                {
                    var newObj = new GDJSON.Object();
                    var objItem = gameData.FrameItems.Items[(int)obj.ObjectInfo];
                    bool alreadyIn = false;
                    foreach (var oldObj in sceneObjects)
                        if (objItem.Name == oldObj.name)
                            alreadyIn = true;
                    if (!alreadyIn)
                    {
                        if (objItem.Properties is ObjectCommon objCommon)
                        {
                            //Actives
                            if (objItem.Header.Type == 2)
                            {
                                if (objCommon.ObjectAnimations == null ||
                                    objCommon.ObjectAnimations.Animations == null ||
                                    objCommon.ObjectAnimations.Animations[0].Directions == null) continue;
                                newObj.name = objItem.Name;
                                newObj.type = "Sprite";
                                var Animations = new List<GDJSON.Animation>();
                                foreach (var anim in objCommon.ObjectAnimations.Animations)
                                {
                                    if (anim.Value.Directions == null) continue;
                                    var newAnim = new GDJSON.Animation();
                                    newAnim.name = $"Animation {anim.Key}";
                                    newAnim.useMultipleDirections = anim.Value.Directions.Count <= 0;
                                    var Directions = new List<GDJSON.Direction>();
                                    foreach (var dir in anim.Value.Directions)
                                    {
                                        if (dir.Frames.Length == 0) continue;
                                        var newDir = new GDJSON.Direction();
                                        newDir.looping = dir.Repeat == -1;
                                        newDir.timeBetweenFrames = 1 / (60 * ((float)dir.MaximumSpeed / 100));
                                        var Images = new List<GDJSON.Sprite>();
                                        foreach (var img in dir.Frames)
                                        {
                                            var newImg = new GDJSON.Sprite();
                                            newImg.image = $"img{img}.png";
                                            var newHotspot = new GDJSON.Originpoint();
                                            newHotspot.x = gameData.ImageBank.Images[img].HotspotX;
                                            newHotspot.y = gameData.ImageBank.Images[img].HotspotY;
                                            newImg.originPoint = newHotspot;
                                            Images.Add(newImg);
                                        }
                                        newDir.sprites = Images.ToArray();
                                        Directions.Add(newDir);
                                    }
                                    newAnim.directions = Directions.ToArray();
                                    Animations.Add(newAnim);
                                }

                                var Variables = new List<GDJSON.ObjectVariable>();
                                var AltFlags = new List<GDJSON.ObjectVariable>();
                                if (objCommon.ObjectAlterableValues != null)
                                {
                                    for (int j = 0; j < objCommon.ObjectAlterableValues.AlterableValues.Length; j++)
                                    {
                                        var newValue = new GDJSON.ObjectVariable();
                                        newValue.name = $"AlterableValue{AltCharacter(j)}";
                                        newValue.value = objCommon.ObjectAlterableValues.AlterableValues[j];
                                        newValue.type = "number";
                                        Variables.Add(newValue);
                                    }

                                    for (int j = 0; j < 32; j++)
                                    {
                                        var newValue = new GDJSON.ObjectVariable();
                                        newValue.name = $"AlterableFlag{AltCharacter(j)}";
                                        newValue.value = objCommon.ObjectAlterableValues.AlterableFlags == j;
                                        newValue.type = "boolean";
                                        AltFlags.Add(newValue);
                                    }

                                    for (int j = 31; j >= 0; j--)
                                        if (AltFlags[j].value is bool b && b == false)
                                            AltFlags.Remove(AltFlags[j]);
                                        else break;
                                }

                                if (objCommon.ObjectAlterableStrings != null)
                                {
                                    for (int j = 0; j < objCommon.ObjectAlterableStrings.AlterableStrings.Length; j++)
                                    {
                                        var newValue = new GDJSON.ObjectVariable();
                                        newValue.name = $"AlterableString{AltCharacter(j)}";
                                        newValue.value = objCommon.ObjectAlterableStrings.AlterableStrings[j];
                                        newValue.type = "string";
                                        Variables.Add(newValue);
                                    }
                                }

                                foreach (var value in AltFlags)
                                    Variables.Add(value);

                                var Effects = new List<GDJSON.ObjectEffect>();

                                var newAlpha = new GDJSON.ObjectEffect();
                                newAlpha.effectType = "BlendingMode";
                                newAlpha.name = "Alpha Blending Coefficient";
                                var newParameter = new GDJSON.Doubleparameters();
                                newParameter.blendmode = 0;
                                newParameter.opacity = 1 - objItem.Header.BlendCoeff / 255;
                                newAlpha.doubleParameters = newParameter;
                                newAlpha.booleanParameters = new();
                                newAlpha.stringParameters = new();
                                Effects.Add(newAlpha);

                                newObj.animations = Animations.ToArray();
                                newObj.variables = Variables.ToArray();
                                newObj.effects = Effects.ToArray();
                            }
                            //Counters
                            else if (objItem.Header.Type == 7)
                            {
                                if (objCommon.ObjectCounter == null ||
                                    objCommon.ObjectCounter.Frames == null ||
                                    objCommon.ObjectCounter.Frames.Length == 0) continue;

                                GDBMFWriter.CounterToBMF(gameData, objCommon);
                                continue;
                                /*newObj.name = objItem.name;
                                newObj.type = "Sprite";
                                var Animations = new List<GDJSON.Animation>();
                                foreach (var anim in objCommon.Animations.AnimationDict)
                                {
                                    if (anim.Value.DirectionDict == null) continue;
                                    var newAnim = new GDJSON.Animation();
                                    newAnim.name = $"Animation {anim.Key}";
                                    newAnim.useMultipleDirections = anim.Value.DirectionDict.Count <= 0;
                                    var Directions = new List<GDJSON.Direction>();
                                    foreach (var dir in anim.Value.DirectionDict.Values)
                                    {
                                        if (dir.Frames.Count == 0) continue;
                                        var newDir = new GDJSON.Direction();
                                        newDir.looping = dir.Repeat == -1;
                                        newDir.timeBetweenFrames = 1 / (60 * ((float)dir.MaxSpeed / 100));
                                        var Images = new List<GDJSON.Sprite>();
                                        foreach (var img in dir.Frames)
                                        {
                                            var newImg = new GDJSON.Sprite();
                                            newImg.image = $"img{img}.png";
                                            var newHotspot = new GDJSON.Originpoint();
                                            newHotspot.x = gameData.Images.Items[img].HotspotX;
                                            newHotspot.y = gameData.Images.Items[img].HotspotY;
                                            newImg.originPoint = newHotspot;
                                            Images.Add(newImg);
                                        }
                                        newDir.sprites = Images.ToArray();
                                        Directions.Add(newDir);
                                    }
                                    newAnim.directions = Directions.ToArray();
                                    Animations.Add(newAnim);
                                }

                                var Variables = new List<GDJSON.ObjectVariable>();
                                var AltFlags = new List<GDJSON.ObjectVariable>();
                                if (objCommon.Values != null)
                                {
                                    for (int j = 0; j < objCommon.Values.Items.Count; j++)
                                    {
                                        var newValue = new GDJSON.ObjectVariable();
                                        newValue.name = $"AlterableValue{AltCharacter(j)}";
                                        newValue.value = objCommon.Values.Items[j];
                                        newValue.type = "number";
                                        Variables.Add(newValue);
                                    }

                                    for (int j = 0; j < 32; j++)
                                    {
                                        var newValue = new GDJSON.ObjectVariable();
                                        newValue.name = $"AlterableFlag{AltCharacter(j)}";
                                        newValue.value = ByteFlag.GetFlag((uint)objCommon.Values.Flags, j);
                                        newValue.type = "boolean";
                                        AltFlags.Add(newValue);
                                    }

                                    for (int j = 31; j >= 0; j--)
                                        if (AltFlags[j].value is bool b && b == false)
                                            AltFlags.Remove(AltFlags[j]);
                                        else break;
                                }

                                if (objCommon.Strings != null)
                                {
                                    for (int j = 0; j < objCommon.Strings.Items.Count; j++)
                                    {
                                        var newValue = new GDJSON.ObjectVariable();
                                        newValue.name = $"AlterableString{AltCharacter(j)}";
                                        newValue.value = objCommon.Strings.Items[j];
                                        newValue.type = "string";
                                        Variables.Add(newValue);
                                    }
                                }

                                foreach (var value in AltFlags)
                                    Variables.Add(value);

                                var Effects = new List<GDJSON.ObjectEffect>();

                                var newAlpha = new GDJSON.ObjectEffect();
                                newAlpha.effectType = "BlendingMode";
                                newAlpha.name = "Alpha Blending Coefficient";
                                var newParameter = new GDJSON.Doubleparameters();
                                newParameter.blendmode = 0;
                                newParameter.opacity = 1 - objItem.blend / 255;
                                newAlpha.doubleParameters = newParameter;
                                newAlpha.booleanParameters = new();
                                newAlpha.stringParameters = new();
                                Effects.Add(newAlpha);

                                newObj.animations = Animations.ToArray();
                                newObj.variables = Variables.ToArray();
                                newObj.effects = Effects.ToArray();*/
                            }
                        }
                        //Backdrops
                        else if (objItem.Properties is ObjectBackdrop objBackdrop)
                        {
                            newObj.name = objItem.Name;
                            newObj.type = "Sprite";

                            newObj.animations = new GDJSON.Animation[1];
                            newObj.animations[0] = new GDJSON.Animation();
                            newObj.animations[0].directions = new GDJSON.Direction[1];
                            newObj.animations[0].directions[0] = new GDJSON.Direction();
                            newObj.animations[0].directions[0].sprites = new GDJSON.Sprite[1];
                            newObj.animations[0].directions[0].sprites[0] = new GDJSON.Sprite();
                            newObj.animations[0].directions[0].sprites[0].originPoint = new GDJSON.Originpoint();

                            newObj.animations[0].name = "Backdrop";
                            newObj.animations[0].useMultipleDirections = false;

                            newObj.animations[0].directions[0].looping = false;
                            newObj.animations[0].directions[0].timeBetweenFrames = 0.033f;

                            newObj.animations[0].directions[0].sprites[0].image = $"img{objBackdrop.Image}.png";
                            newObj.animations[0].directions[0].sprites[0].originPoint.x = 0;
                            newObj.animations[0].directions[0].sprites[0].originPoint.y = 0;
                        }
                        else continue;
                    }

                    var newInstance = new GDJSON.Instance();
                    newInstance.layer = frame.FrameLayers.Layers[obj.Layer].Name;
                    newInstance.name = objItem.Name;
                    newInstance.x = obj.PositionX;
                    newInstance.y = obj.PositionY;
                    newInstance.zOrder = z;
                    z++;

                    sceneInstances.Add(newInstance);
                    sceneObjects.Add(newObj);
                }

                var Events = new List<GDJSON.FrameEvents>();
                foreach (var evnt in frame.FrameEvents.Events)
                {
                    var newEvnt = new GDJSON.FrameEvents();
                    var Conditions = new List<GDJSON.Condition>();
                    foreach (var cond in evnt.Conditions)
                    {
                        GDJSON.Condition newCond = null;
                        /*if (CTFAKCore.parameters.Contains("-dumpevents"))
                        {
                            int p = 0;
                            foreach (var parameter in cond.Items)
                            {
                                var paramreader = new ByteWriter(new MemoryStream());
                                parameter.Write(paramreader);
                                File.WriteAllBytes($"Events\\Condition {cond.ObjectType} ~ {cond.Num} ~ Parameter{p}.bin", paramreader.GetBuffer());
                                p++;
                            }
                        }*/
                        switch (cond.ObjectType)
                        {
                            /* To Add:
                                Counter !=<>
                                Animation is Over
                                Overlapping
                                Mouse Pointer Over
                                Compare X Position
                                User Clicks on Object
                                Compare Two General Values
                                Repeat while key is pressed
                                Animation is playing
                                Timer is greater than
                                Run this event once
                                Current Frame !=<>
                                Is Visible
                            */
                            case -6:
                                switch (cond.Num)
                                {
                                    case -1: //Upon pressing key
                                        newCond = GDConditions.KeyPressed(cond, gameData);
                                        break;
                                }
                                break;
                            case -4:
                                switch (cond.Num)
                                {
                                    case -8: //Every Timer
                                        newCond = GDConditions.EveryTimer(cond, gameData, Events.Count);
                                        if (!extensions.Contains(extnames["Every"]))
                                            extensions.Add(extnames["Every"]);
                                        break;
                                }
                                break;
                            case -3:
                                switch (cond.Num)
                                {
                                    case -1: //Start of Frame
                                        newCond = GDConditions.DefaultType(cond, gameData, false, false, "DepartScene");
                                        break;
                                }
                                break;
                            case -1:
                                switch (cond.Num)
                                {
                                    case -1: //Always
                                        newCond = GDConditions.DefaultType(cond, gameData, false, false, "BuiltinCommonInstructions::Always");
                                        break;
                                    case -2: //Never
                                        newCond = GDConditions.DefaultType(cond, gameData, false, true, "BuiltinCommonInstructions::Always");
                                        break;
                                    case -7: //Only One Action
                                        newCond = GDConditions.DefaultType(cond, gameData, false, false, "BuiltinCommonInstructions::Once");
                                        break;
                                }
                                break;
                            case 2:
                                switch (cond.Num)
                                {
                                    case -27: //Compare to Alterable Value
                                        newCond = GDConditions.CompareAltVal(cond, gameData);
                                        break;
                                }
                                break;
                        }
                        if (newCond == null)
                        {
                            this.Log($"Unknown Condition: {cond.ObjectType} ~ {cond.Num}");
                            int p = 0;
                            foreach (var parameter in cond.Parameters)
                            {
                                this.Log($"Parameter {p} - {parameter.Code}");
                                if (parameter.Data is ParameterExpressions loader)
                                    for (int l = 0; l < loader.Expressions.Count; l++)
                                        this.Log($"Expression {l} - {loader.Expressions[l].ObjectType} ~ {loader.Expressions[l].Num}");
                                p++;
                            }
                        }

                        Conditions.Add(newCond);
                    }
                    var Actions = new List<GDJSON.Action>();
                    foreach (var action in evnt.Actions)
                    {
                        GDJSON.Action newAction = null;
                        /*if (CTFAKCore.parameters.Contains("-dumpevents"))
                        {
                            int p = 0;
                            foreach (var parameter in action.Items)
                            {
                                var paramreader = new ByteWriter(new MemoryStream());
                                parameter.Write(paramreader);
                                File.WriteAllBytes($"Events\\Action {action.ObjectType} ~ {action.Num} ~ Parameter{p}.bin", paramreader.GetBuffer());
                                p++;
                            }
                        }*/
                        switch (action.ObjectType)
                        {
                            /* To Add:
                                Set/Add to/Subtract from; Counter
                                Set Ini File
                                Set Ini Group
                                Set Ini Value
                            */
                            case -5:
                                switch (action.Num)
                                {
                                    case 0: //Create at Position
                                        newAction = GDActions.CreateAt(action, gameData, Scenes.Count);
                                        break;
                                }
                                break;
                            case -3:
                                switch (action.Num)
                                {
                                    case 0: //Next Frame
                                        newAction = GDActions.ToFrame(action, gameData, Scenes.Count);
                                        break;
                                    case 2: //Jump to Frame
                                        newAction = GDActions.ToFrame(action, gameData, -1);
                                        break;
                                    case 4: //End Application
                                        newAction = GDActions.DefaultType(action, gameData, false, "Quit");
                                        break;
                                    case 8: //Center Display at X
                                        newAction = GDActions.SetCameraXorY(action, gameData, "X");
                                        break;
                                }
                                break;
                            case -2:
                                switch (action.Num)
                                {
                                    case 1: //Stop Any Sample
                                        newAction = GDActions.DefaultType(action, gameData, false, "UnloadAllAudio");
                                        break;
                                    case 11: //Play Sound on Channel
                                        newAction = GDActions.PlaySoundChannel(action, gameData, false);
                                        break;
                                    case 12: //Loop Sound on Channel
                                        newAction = GDActions.PlaySoundChannel(action, gameData, true);
                                        break;
                                    case 17: //Set Volume of Channel
                                        newAction = GDActions.SetChannelVolume(action, gameData);
                                        break;
                                }
                                break;
                            case 2:
                                switch (action.Num)
                                {
                                    case 1: //Set Position
                                        newAction = GDActions.SetPosition(action, gameData);
                                        break;
                                    case 2: //Set X
                                        newAction = GDActions.SetXorY(action, gameData, "X");
                                        break;
                                    case 3: //Set Y
                                        newAction = GDActions.SetXorY(action, gameData, "Y");
                                        break;
                                    case 17: //Set Animation
                                        newAction = GDActions.SetAnimation(action, gameData);
                                        break;
                                    case 24: //Destroy
                                        newAction = GDActions.DefaultType(action, gameData, true, "Delete");
                                        break;
                                    case 26: //Show
                                        newAction = GDActions.DefaultType(action, gameData, true, "Cache");
                                        break;
                                    case 27: //Hide
                                        newAction = GDActions.DefaultType(action, gameData, true, "Montre");
                                        break;
                                    case 31: //Set Alterable Value
                                        newAction = GDActions.SetAltVal(action, gameData, "=");
                                        break;
                                    case 32: //Add to Alterable Value
                                        newAction = GDActions.SetAltVal(action, gameData, "+");
                                        break;
                                    case 33: //Subtract from Alterable Value
                                        newAction = GDActions.SetAltVal(action, gameData, "-");
                                        break;
                                    case 65: //Set Alpha Blending Coefficient
                                        newAction = GDActions.SetOpacity(action, gameData);
                                        break;
                                }
                                break;
                        }
                        if (newAction == null)
                        {
                            this.Log($"Unknown Action: {action.ObjectType} ~ {action.Num}");
                            int p = 0;
                            foreach (var parameter in action.Parameters)
                            {
                                this.Log($"Parameter{p} - {parameter.Code}");
                                if (parameter.Data is ParameterExpressions loader)
                                    for (int l = 0; l < loader.Expressions.Count; l++)
                                        this.Log($"Expression {l} - {loader.Expressions[l].ObjectType} ~ {loader.Expressions[l].Num}");
                                p++;
                            }
                        }

                        Actions.Add(newAction);
                    }
                    if (Conditions.Count == 0 && Actions.Count == 0) continue;

                    newEvnt.conditions = Conditions.ToArray();
                    newEvnt.actions = Actions.ToArray();
                    Events.Add(newEvnt);
                }
                newScene.layers = sceneLayers.ToArray();
                newScene.objects = sceneObjects.ToArray();
                newScene.instances = sceneInstances.ToArray();
                newScene.events = Events.ToArray();
                Scenes.Add(newScene);
            }

            JSONtoWrite.properties = Properties;
            JSONtoWrite.resources = Resources;
            JSONtoWrite.layouts = Scenes.ToArray();

            var JSON = JsonConvert.SerializeObject(JSONtoWrite, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            string exts = "\"eventsFunctionsExtensions\": [";
            foreach (int ext in extensions)
            {
                if (exts[exts.Length - 1] == '}')
                    exts += ",";
                exts += GDExtensions.extensions[ext];
            }
            exts += "],";

            JSON = JSON.Replace(":null", ":[]").Replace("\"eventsFunctionsExtensions\":[],", exts);
            File.WriteAllText($"{outPath}\\game.json", JSON);
        }
    }
}
