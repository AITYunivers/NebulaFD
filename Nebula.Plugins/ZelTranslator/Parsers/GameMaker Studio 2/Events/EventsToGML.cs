using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ZelTranslator_SD.Parsers.GameMakerStudio2.ObjectYY;
using ZelTranslator_SD.Parsers.GameMakerStudio2;
using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Action = Nebula.Core.Data.Chunks.FrameChunks.Events.Action;
using Nebula.Core.Data.Chunks.ObjectChunks;
using System.ComponentModel;
using Nebula.Core.Data.Chunks.FrameChunks;

namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class EventsToGML
    {
        public static string GetOperator(short Comparison)
        {
            switch (Comparison)
            {
                case 0: return "==";
                case 1: return "!=";
                case 2: return "<=";
                case 3: return "<";
                case 4: return ">=";
                case 5: return ">";
                default: return "err";
            }
        }
        public static void Write(List<GMLFile> gmlFiles, List<ProjectYYP.Resource> projResources, List<ObjectYY.RootObject> objects, PackageData gameData, List<RoomYY.RootObject> rooms, ref List<string> missing_code)
        {
            /*
             * NotFollowFrame parent object
             */
            var FollowFrame = new ObjectYY.RootObject();
            FollowFrame.name = "NotFollowFrame";
            FollowFrame.visible = false;

            objects.Add(FollowFrame);

            var FollowFrameRes = new ProjectYYP.Resource();
            var FollowFrameResID = new ProjectYYP.ResourceID();

            FollowFrameResID.name = FollowFrame.name;
            FollowFrameResID.path = $"objects/{FollowFrame.name}/{FollowFrame.name}.yy";
            FollowFrameRes.id = FollowFrameResID;
            FollowFrameRes.order = GMS2Writer.ObjectOrder;
            GMS2Writer.ObjectOrder++;
            projResources.Add(FollowFrameRes);



            /*
             * Backdrop obstacles parent object
             */
            var BackdropObstacle = new ObjectYY.RootObject();
            BackdropObstacle.name = "Backdrop_Obstacle";
            BackdropObstacle.visible = false;

            objects.Add(BackdropObstacle);

            var bdObstacleRes = new ProjectYYP.Resource();
            var bdObstacleResID = new ProjectYYP.ResourceID();

            bdObstacleResID.name = BackdropObstacle.name;
            bdObstacleResID.path = $"objects/{BackdropObstacle.name}/{BackdropObstacle.name}.yy";
            bdObstacleRes.id = bdObstacleResID;
            bdObstacleRes.order = GMS2Writer.ObjectOrder;
            GMS2Writer.ObjectOrder++;
            projResources.Add(bdObstacleRes);


            /*
             * Frames parent object
             */
            var FrameParent = new ObjectYY.RootObject();
            FrameParent.name = "Frame";
            FrameParent.visible = false;

            objects.Add(FrameParent);

            var FrameParentRes = new ProjectYYP.Resource();
            var FrameParentResID = new ProjectYYP.ResourceID();

            FrameParentResID.name = FrameParent.name;
            FrameParentResID.path = $"objects/{FrameParent.name}/{FrameParent.name}.yy";
            FrameParentRes.id = FrameParentResID;
            FrameParentRes.order = GMS2Writer.ObjectOrder;
            GMS2Writer.ObjectOrder++;
            projResources.Add(FrameParentRes);


            /*
             * Frame Camera object
             */
            var frameCam = new ObjectYY.RootObject();
            frameCam.name = "frameCamera";
            frameCam.visible = true;

            var frameCamEvts = new List<ObjectYY.Event>();

            var frameCamCreate = new ObjectYY.Event();
            frameCamCreate.eventNum = 0;
            frameCamCreate.eventType = 0;
            frameCamEvts.Add(frameCamCreate);
            var frameCamEndStep = new ObjectYY.Event();
            frameCamEndStep.eventNum = 2;
            frameCamEndStep.eventType = 3;
            frameCamEvts.Add(frameCamEndStep);

            frameCam.eventList = frameCamEvts.ToArray();
            objects.Add(frameCam);

            var frameCamRes = new ProjectYYP.Resource();
            var frameCamResID = new ProjectYYP.ResourceID();

            frameCamResID.name = frameCam.name;
            frameCamResID.path = $"objects/{frameCam.name}/{frameCam.name}.yy";
            frameCamRes.id = frameCamResID;
            frameCamRes.order = GMS2Writer.ObjectOrder;
            GMS2Writer.ObjectOrder++;
            projResources.Add(frameCamRes);

            var camCreateGML = new GMLFile();
            camCreateGML.name = "Create_0";
            camCreateGML.path = $"objects\\{frameCam.name}";
            var createAsmbly = Assembly.GetExecutingAssembly();
            var createFilePath = "ZelTranslator_SD.Parsers.GameMaker_Studio_2.Resources.Camera_Create.gml";
            using (Stream s = createAsmbly.GetManifestResourceStream(createFilePath))
            using (StreamReader sr = new StreamReader(s))
            {
                camCreateGML.code = (sr.ReadToEnd()).Replace("<CAMWIDTH>", $"{gameData.AppHeader.AppWidth}").Replace("<CAMHEIGHT>", $"{gameData.AppHeader.AppHeight}");
            }

            var camEndStepGML = new GMLFile();
            camEndStepGML.name = "Step_2";
            camEndStepGML.path = $"objects\\{frameCam.name}";
            var esAsmbly = Assembly.GetExecutingAssembly();
            var esFilePath = "ZelTranslator_SD.Parsers.GameMaker_Studio_2.Resources.Camera_EndStep.gml";
            using (Stream s = esAsmbly.GetManifestResourceStream(esFilePath))
            using (StreamReader sr = new StreamReader(s))
            {
                camEndStepGML.code = sr.ReadToEnd();
            }

            gmlFiles.Add(camCreateGML);
            gmlFiles.Add(camEndStepGML);
            //

            /*
             * Kotfile Ini object -- by nkrapivin
            
            var kotfile = new ObjectYY.RootObject();
            kotfile.name = "objKotfile";
            kotfile.visible = false;
            kotfile.persistent = true;

            var kotfileEvts = new List<ObjectYY.Event>();

            var kotfileAsync = new ObjectYY.Event();
            kotfileAsync.eventNum = 72;
            kotfileAsync.eventType = 7;
            kotfileEvts.Add(kotfileAsync);

            var kotfileStart = new ObjectYY.Event();
            kotfileStart.eventNum = 2;
            kotfileStart.eventType = 7;
            kotfileEvts.Add(kotfileStart);

            kotfile.eventList = kotfileEvts.ToArray();
            objects.Add(kotfile);

            var kotfileRes = new ProjectYYP.Resource();
            var kotfileResID = new ProjectYYP.ResourceID();

            kotfileResID.name = kotfile.name;
            kotfileResID.path = $"objects/{kotfile.name}/{kotfile.name}.yy";
            kotfileRes.id = kotfileResID;
            kotfileRes.order = GMS2Writer.ObjectOrder;
            GMS2Writer.ObjectOrder++;
            projResources.Add(kotfileRes);

            var kotfileAsyncGML = new GMLFile();
            kotfileAsyncGML.name = "Other_72";
            kotfileAsyncGML.path = $"objects\\{kotfile.name}";
            kotfileAsyncGML.code = "/// @description Handle Save Load events through Kotfile.\nKotfileAsync();";
            gmlFiles.Add(kotfileAsyncGML);

            var kotfileStartGML = new GMLFile();
            kotfileStartGML.name = "Other_2";
            kotfileStartGML.path = $"objects\\{kotfile.name}";
            kotfileStartGML.code = "/// @description Initialize Kotfile\n__kfDummyVariable = KotfileInit();\nglobal.ini_file = \"\";\nglobal.ini_group = \"\";\nglobal.ini_item = \"\";\nglobal.ini_string = \"\";";
            gmlFiles.Add(kotfileStartGML);
            */
            //

            //Global Scripts
            List<string> globalScripts = new List<string>() {
                    "ZelCTF"//, "Kotfile", "IniSaveLoad", "TextFileWrite"
                };
            int scriptOrder = 0;
            foreach (var scrName in globalScripts)
            {
                ProjectYYP.Resource newScriptRes = new ProjectYYP.Resource();
                ProjectYYP.ResourceID newScriptResID = new ProjectYYP.ResourceID();

                newScriptResID.name = scrName;
                newScriptResID.path = $"scripts/{scrName}/{scrName}.yy";
                newScriptRes.id = newScriptResID;
                newScriptRes.order = scriptOrder;
                scriptOrder++;
                projResources.Add(newScriptRes);
                GMLFile zelCtfGML = new GMLFile(); // GMLFile
                zelCtfGML.name = scrName;
                zelCtfGML.path = $"scripts\\{zelCtfGML.name}";

                var asmbly = Assembly.GetExecutingAssembly();
                var filePath = $"ZelTranslator_SD.Parsers.GameMaker_Studio_2.Resources.{scrName}.gml";
                using (Stream s = asmbly.GetManifestResourceStream(filePath))
                using (StreamReader sr = new StreamReader(s))
                {
                    zelCtfGML.code = sr.ReadToEnd();
                }
                gmlFiles.Add(zelCtfGML);

            }


            Logger.LogType(typeof(EventsToGML), "Parsing extensions");
            Dictionary<string, int> ExtCodes = new Dictionary<string, int>();
            foreach (var obj in gameData.FrameItems.Items.Values)
            {
                if (obj.Properties is ObjectCommon common)
                {
                    if (common.Identifier != "SPRI" && common.Identifier != "SP" && common.Identifier != "CNTR" && common.Identifier != "CN" && common.Identifier != "TEXT" && common.Identifier != "TE")
                    {

                        try
                        {
                            if (!ExtCodes.ContainsKey(common.Identifier))
                            {
                                ExtCodes.Add(common.Identifier, obj.Header.Type);
                                Logger.LogType(typeof(EventsToGML), $"ID:{common.Identifier}-ObjType:{obj.Header.Type}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogType(typeof(EventsToGML), $"Error parsing extension: {ex}");
                        }
                    }
                }
            }

            int frameCount = 0;
            foreach (var frame in gameData.Frames)
            {
                // Add the built-in Fusion objects (i.e. Timer, Storyboard Controls, etc.)
                //string NewObjName = "";
                List<int> CTF_objectIDs = new List<int>();
                /*
                for (var i = 0; i < 10; i++)
                {
                    switch (i)
                    {
                        case 0:
                            NewObjName = "Special";
                            break;
                        case 1:
                            NewObjName = "Sound";
                            break;
                        case 2:
                            NewObjName = "Storyboard";
                            break;
                        case 3:
                            NewObjName = "Timer";
                            break;
                        case 4:
                            NewObjName = "Create";
                            break;
                        case 5:
                            NewObjName = "KeyboardMouse";
                            break;
                        case 6:
                            NewObjName = "Player1";
                            break;
                        case 7:
                            NewObjName = "Player2";
                            break;
                        case 8:
                            NewObjName = "Player3";
                            break;
                        case 9:
                            NewObjName = "Player4";
                            break;


                    }
                */
                var newObj = new ObjectYY.RootObject();
                newObj.name = $"CTF_Events_{frameCount}";
                newObj.visible = false;

                var frameParent = new ParentObjectID();
                frameParent.name = "Frame";
                frameParent.path = "objects/Frame/Frame.yy";
                newObj.parentObjectId = frameParent;

                var evtList = new List<ObjectYY.Event>();

                /*
                 * Initialize GMS2 events
                 */

                //Create
                ObjectYY.Event createEvent = new ObjectYY.Event(); // Event
                createEvent.eventNum = 0;
                createEvent.eventType = 0;
                evtList.Add(createEvent);
                GMLFile createGML = new GMLFile(); // GMLFile
                createGML.name = "Create_0";
                createGML.path = $"objects\\{newObj.name}";
                createGML.code = $"ZelCTF();\nglobal.eventInstLists = array_create_zel({frame.FrameEvents.Events.Count}+1); // Create event instance lists\nnotAlways = array_notalways({frame.FrameEvents.Events.Count}+1); // Create notAlways and runOnce lists\nrunOnce = notAlways;\ngroups = array_notalways(<GROUPCOUNT>);\nOnlyOneActionWhenEventLoops = true; // Dummy/Aesthetic variable\n";
                createGML.code += $"MoveTimer = {frame.FrameMoveTimer}\n";
                // Step
                ObjectYY.Event stepEvent = new ObjectYY.Event(); // Event
                stepEvent.eventNum = 0;
                stepEvent.eventType = 3;
                evtList.Add(stepEvent);
                GMLFile stepGML = new GMLFile(); // GMLFile
                stepGML.name = "Step_0";
                stepGML.path = $"objects\\{newObj.name}";
                stepGML.code = $"timer += delta_time / 1000000.0; // Count up game timer\nglobal.eventInstLists = array_create_zel({frame.FrameEvents.Events.Count}+1); // Clear/refresh event instance lists\n\n" + $"for (var i = 0; i < array_length(everyTimers); i++) {{\n\teveryTimers[i][1] = max(everyTimers[i][1] - delta_time/1000000.0, 0); // Count down everyTimers\n}}\n\n";
                stepGML.code += $"for (var i = 0; i < array_length(equalsTimers); i++) {{\n\tif (equalsTimers[i][1] > -1) then equalsTimers[i][1] = max(equalsTimers[i][1] - delta_time/1000000.0, 0); // Count down equalsTimers\n}}\n\n/*\n\tEvent List\n*/\n\n";

                // Room Start
                ObjectYY.Event roomstartEvent = new ObjectYY.Event(); // Event
                roomstartEvent.eventNum = 4;
                roomstartEvent.eventType = 7;
                evtList.Add(roomstartEvent);
                GMLFile roomStartGML = new GMLFile(); // GMLFile
                roomStartGML.name = "Other_4";
                roomStartGML.path = $"objects\\{newObj.name}";
                roomStartGML.code = "// Pre-load all sprites in the room\nvar arr = [];\nwith(all){\n\tif (sprite_index != -1) array_push(arr,sprite_index);\n}\nsprite_prefetch_multi(arr);\n\n";

                // Room End
                ObjectYY.Event roomendEvent = new ObjectYY.Event(); // Event
                roomendEvent.eventNum = 5;
                roomendEvent.eventType = 7;
                evtList.Add(roomendEvent);
                GMLFile roomendGML = new GMLFile(); // GMLFile
                roomendGML.name = "Other_5";
                roomendGML.path = $"objects\\{newObj.name}";
                roomendGML.code = "";

                // Game End
                ObjectYY.Event gameendEvent = new ObjectYY.Event(); // Event
                gameendEvent.eventNum = 3;
                gameendEvent.eventType = 7;
                evtList.Add(gameendEvent);
                GMLFile gameendGML = new GMLFile(); // GMLFile
                gameendGML.name = "Other_3";
                gameendGML.path = $"objects\\{newObj.name}";
                gameendGML.code = "";

                // Game Start
                ObjectYY.Event gamestartEvent = new ObjectYY.Event();
                gamestartEvent.eventNum = 2;
                gamestartEvent.eventType = 7;
                evtList.Add(gamestartEvent);
                GMLFile gamestartGML = new GMLFile(); // GMLFile
                gamestartGML.name = "Other_2";
                gamestartGML.path = $"objects//{newObj.name}";
                gamestartGML.code = "";
                if (frameCount == 0) gamestartGML.code = "// Init INI variables\nglobal.ini_file = \"\";\nglobal.ini_group = \"\";\nglobal.ini_item = \"\";\nglobal.ini_string = \"\";\n\n";

                var createVariables = new List<string>() { $"{Throw.missingCond()} = false; // In case of missing conditions",
                                                            "timer = 0;",
                                                            "for (var i = 1; i < 48+1; i++) variable_global_set(\"channel\" + string(i), audio_emitter_create()); // Init all 48 sample channels"};
                string createCode = "";
                var everyTimers = new List<string[]>();
                var equalsTimers = new List<string[]>();
                int evntCount = 1;
                int groupCount = 0;
                int indents = 0;

                var notAlwaysEvts = new List<int>();
                var runOnceEvts = new List<int>();
                foreach (var evnt in frame.FrameEvents.Events)
                {
                    foreach (var cmt in frame.FrameEvents.Comments)
                    {
                        string cmtLog = $"COMMENT in FRAME {frameCount}: {cmt.Handle} {cmt.Value}";
                        Logger.LogType(typeof(EventsToGML), cmtLog);
                        missing_code.Add(cmtLog);
                    }
                    string evntIfStatement = string.Concat(Enumerable.Repeat("\t", indents)) + $"/*ev{evntCount}*/ if (";
                    int condCount = 1;

                    bool startOfFrame = false; // If event contains "Start of Frame" condition
                    bool endOfFrame = false; // If event contains "End of Frame" condition
                    bool endOfApp = false; // If event contains "End of Application" condition
                    bool notAlways = false; // If event contains "Only one action when event loops" condition
                    bool runOnce = false; // If event contains "Run this event once" condition
                    bool groupStart = false; // If group start is declared
                    bool groupEnd = false;

                    ParameterGroup group = new ParameterGroup();
                    foreach (var cond in evnt.Conditions)
                    {
                        try
                        {
                            if (condCount > 1)
                                if (!evntIfStatement.EndsWith(") || (")) evntIfStatement += " && ";
                            if (cond.OtherFlags["Negated"]) evntIfStatement += "!";
                            switch (cond.ObjectType)
                            {
                                case -7: // Player 1
                                    {
                                        switch (cond.Num)
                                        {
                                            //case -4: // Pressed button (notAlways) (FIX/REWORK add this)
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code);
                                                break;
                                        }
                                        break;
                                    }
                                case -6: // Keyboard/Mouse Input
                                    {
                                        switch (cond.Num)
                                        {
                                            case -9: // Upon pressing any key
                                                {
                                                    evntIfStatement += $"keyboard_check_pressed(vk_anykey)";
                                                    break;
                                                }
                                            case -8: // Repeat while left mouse-key is pressed
                                                {
                                                    evntIfStatement += $"mouse_check_button(mb_left)";
                                                    break;
                                                }
                                            case -7: // User clicks with left button on object -- (FIX/REWORK to work with EACH instance)
                                                {
                                                    var clickedObj = gameData.FrameItems.Items[(cond.Parameters[1].Data as ParameterObject).ObjectInfo];
                                                    var clickedObjName = GMS2Writer.CleanString(clickedObj.Name).Replace(" ", "_") + "_" + clickedObj.Header.Handle;
                                                    clickedObjName = $"object({GMS2Writer.ObjectName(clickedObj)})";
                                                    evntIfStatement += $"ClickedObj({clickedObjName}, mb_left, true, true, {evntCount})";
                                                    break;
                                                }
                                            case -5: // User clicks with with left button
                                                {
                                                    evntIfStatement += $"mouse_check_button_pressed(mb_left)";
                                                    break;
                                                }
                                            case -4: // Mouse pointer is over object -- (FIX/REWORK to work with EACH instance)
                                                {
                                                    var hoverObj = gameData.FrameItems.Items[(cond.Parameters[0].Data as ParameterObject).ObjectInfo];
                                                    var hoverObjName = GMS2Writer.CleanString(hoverObj.Name).Replace(" ", "_") + "_" + hoverObj.Header.Handle;
                                                    hoverObjName = $"object({GMS2Writer.ObjectName(hoverObj)})";
                                                    evntIfStatement += $"MouseOver({hoverObjName}, true, true, {evntCount})";
                                                    break;
                                                }
                                            case -2: // Repeat while pressing key
                                                {
                                                    var pressingKey = cond.Parameters[0].Data as ParameterShort;
                                                    evntIfStatement += $"keyboard_check({pressingKey.Value})";
                                                    break;
                                                }
                                            case -1: // Upon pressing key
                                                {
                                                    var pressedKey = cond.Parameters[0].Data as ParameterShort;
                                                    evntIfStatement += $"keyboard_check_pressed({pressedKey.Value})";
                                                    break;
                                                }
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                    }
                                    break;
                                case -5: // Create
                                    {
                                        switch (cond.Num)
                                        {
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                        break;
                                    }
                                case -4: // Timer -- (FIX/REWORK all of these to ensure that they work with expressions)
                                    {
                                        switch (cond.Num)
                                        {
                                            case -8: // Every XX"-XX (aka Every2?)
                                            case -4: // Every XX"-XX
                                                {
                                                    string interval = "";
                                                    if (cond.Parameters[0].Data is ParameterTimer time) interval = (time.Timer / 1000.0).ToString();
                                                    else if (cond.Parameters[0].Data is ParameterEvery every) interval = (every.Delay / 1000.0).ToString();
                                                    else if (cond.Parameters[0].Data is ParameterExpressions expParam)
                                                    {
                                                        interval = $"{GMS2Expressions.Evaluate(expParam, evntCount, gameData, ExtCodes, ref missing_code)} / 1000.0";
                                                    }
                                                    everyTimers.Add(new string[] { $"everyTimer{everyTimers.Count}", $"{interval}", $"{interval}" }); // Format: [ name, interval, currentVal ]
                                                    evntIfStatement += $"everyTimer{everyTimers.Count - 1}[1] == 0";
                                                    break;
                                                }
                                            case -7: // Timer equals XX"-XX
                                                {
                                                    string seconds = "";
                                                    if (cond.Parameters[0].Data is ParameterTimer time) seconds = (time.Timer / 1000.0).ToString();
                                                    else if (cond.Parameters[0].Data is ParameterExpressions expParam) seconds = $"{GMS2Expressions.Evaluate(expParam, evntCount, gameData, ExtCodes, ref missing_code)} / 1000.0";
                                                    equalsTimers.Add(new string[] { $"equalsTimer{equalsTimers.Count}", $"{seconds}", $"{seconds}" }); // Format: [ name, interval, currentVal ]
                                                    evntIfStatement += $"equalsTimer{equalsTimers.Count - 1}[1] == 0";
                                                    break;
                                                }
                                            /*case -8:
                                            case -4: // Every XX"-XX
                                                {
                                                    var Parameter1 = cond.Parameters[0].Data as Every;
                                                    var interval = (Parameter1.Delay / 1000.0).ToString();
                                                    everyTimers.Add(new string[] { $"everyTimer{everyTimers.Count}", $"{interval}", $"{interval}" }); // Format: [ name, interval, currentVal ]
                                                    evntIfStatement += $"everyTimer{everyTimers.Count - 1}[1] == 0";
                                                    break;
                                                }*/
                                            case -2: // Timer is less than XX"-XX
                                                {
                                                    string seconds = "";
                                                    if (cond.Parameters[0].Data is ParameterTimer time) seconds = (time.Timer / 1000.0).ToString();
                                                    else if (cond.Parameters[0].Data is ParameterExpressions expParam) seconds = $"{GMS2Expressions.Evaluate(expParam, evntCount, gameData, ExtCodes, ref missing_code)} / 1000.0";
                                                    evntIfStatement += $"timer < {seconds}";
                                                    break;
                                                }
                                            case -1: // Timer is greater than XX"-XX
                                                {
                                                    string seconds = "";
                                                    if (cond.Parameters[0].Data is ParameterTimer time) seconds = (time.Timer / 1000.0).ToString();
                                                    else if (cond.Parameters[0].Data is ParameterExpressions expParam) seconds = $"{GMS2Expressions.Evaluate(expParam, evntCount, gameData, ExtCodes, ref missing_code)} / 1000.0";
                                                    evntIfStatement += $"timer > {seconds}";
                                                    break;
                                                }
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                        break;

                                    }
                                case -3: // Storyboard
                                    {
                                        switch (cond.Num)
                                        {
                                            case -4: // End of Application
                                                {
                                                    endOfApp = true;
                                                    evntIfStatement += "event_number == ev_game_end";
                                                    break;
                                                }
                                            case -2: // End of Frame
                                                {
                                                    endOfFrame = true;
                                                    evntIfStatement += "event_number == ev_room_end";
                                                    break;
                                                }
                                            case -1: // Start of Frame
                                                {
                                                    startOfFrame = true;
                                                    evntIfStatement += "event_number == ev_room_start";
                                                    break;
                                                }
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                        break;
                                    }
                                case -2: // Sound
                                    {
                                        if (GMS2Writer.Args["-nosnds"]) throw new Exception("Ignoring sound condition");
                                        switch (cond.Num)
                                        {
                                            case -1: // Sample is not playing
                                                {
                                                    var SampleName = (cond.Parameters[0].Data as ParameterSample).Name;
                                                    evntIfStatement += $"!audio_is_playing(snd_{GMS2Writer.CleanStringFull(SampleName)})";
                                                    break;
                                                }
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code);
                                                break;
                                        }
                                        break;
                                    }
                                case -1: // Special
                                    {
                                        switch (cond.Num)
                                        {
                                            case -25: // OR (logical) (fix/rework in case this doesn't actually function properly)
                                                {
                                                    if (evntIfStatement.EndsWith(" && "))
                                                        evntIfStatement = evntIfStatement[..^4];
                                                    evntIfStatement += ") || (";
                                                    break;
                                                }
                                            case -24: // OR (filtered) (fix/rework in case this doesn't actually function properly)
                                                {
                                                    if (evntIfStatement.EndsWith(" && "))
                                                        evntIfStatement = evntIfStatement[..^4];
                                                    evntIfStatement += ") || (";
                                                    break;
                                                }
                                            //case -23: notAlways = true; // On group activation (targets group that the condition is within) (FIX/REWORK add this)
                                            case -12: // Group ____ is activated
                                                {
                                                    var g = cond.Parameters[0].Data as ParameterGroupPointer;
                                                    evntIfStatement += $"groups[{g.ID}] == true";
                                                    break;
                                                }
                                            case -11: // GROUP END/Group Footer
                                                //evntIfStatement += "}\n#endregion";
                                                evntIfStatement = "";
                                                groupEnd = true;
                                                indents--;
                                                break;
                                            case -10: // GROUP START/Group header
                                                {
                                                    group = cond.Parameters[0].Data as ParameterGroup;
                                                    evntIfStatement = evntIfStatement.Insert(evntIfStatement.IndexOf("/*"), $"#region ").Replace("*/ ", $"*/ {group.Name}\n");
                                                    evntIfStatement += $"groups[{group.ID}]";
                                                    groupStart = true;
                                                    groupCount++;
                                                    indents++;
                                                    break;
                                                }
                                            case -7: // Only one action when event loops
                                                {
                                                    notAlwaysEvts.Add(evntCount);
                                                    evntIfStatement += "OnlyOneActionWhenEventLoops";
                                                    notAlways = true;
                                                    break;
                                                }
                                            case -6: // Run this event once
                                                {
                                                    runOnceEvts.Add(evntCount);
                                                    evntIfStatement += $"runOnce[{evntCount}]";
                                                    runOnce = true;
                                                    break;
                                                }
                                            case -3: // Compare two general values
                                                {
                                                    var exp1str = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                    var exp2 = cond.Parameters[1].Data as ParameterExpressions;
                                                    var exp2str = GMS2Expressions.Evaluate(exp2, evntCount, gameData, ExtCodes, ref missing_code);
                                                    evntIfStatement += $"{exp1str} {GetOperator(exp2.Comparison)} {exp2str}";
                                                    break;
                                                }
                                            case -2: // Never (LMAO)
                                                {
                                                    evntIfStatement += "false";
                                                    break;
                                                }
                                            case -1: // Always (lol)
                                                {
                                                    evntIfStatement += "true";
                                                    break;
                                                }
                                            // NOTE/FIX/REWORK: Add "Compare two general values"
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                        break;
                                    }
                                case 2: // Active/Object -- NOTE: every condition here MUST be accomodated to work with EACH instance of the object individually
                                    {
                                        switch (cond.Num)
                                        {
                                            case -34: // Pick one random instance of Object
                                                {
                                                    var RandomObject = gameData.FrameItems.Items[cond.ObjectInfo];
                                                    var ObjectName = GMS2Writer.CleanString(RandomObject.Name).Replace(" ", "_") + "_" + RandomObject.Header.Handle;
                                                    ObjectName = $"object({GMS2Writer.ObjectName(RandomObject)})";
                                                    evntIfStatement += $"PickOneOf({ObjectName}, {evntCount})";
                                                    break;
                                                }
                                            case -32: // Number of objects
                                                {
                                                    var CountObject = gameData.FrameItems.Items[cond.ObjectInfo];
                                                    var ObjectName = GMS2Writer.CleanString(CountObject.Name).Replace(" ", "_") + "_" + CountObject.Header.Handle;
                                                    ObjectName = $"object({GMS2Writer.ObjectName(CountObject)})";
                                                    var expression = cond.Parameters[0].Data as ParameterExpressions;
                                                    evntIfStatement += $"instance_number({ObjectName}) {GetOperator(expression.Comparison)} {GMS2Expressions.Evaluate(expression, evntCount, gameData, ExtCodes, ref missing_code)}";
                                                    break;
                                                }
                                            case -30: // Number of (object) in a zone
                                                {
                                                    ParameterZone zone = cond.Parameters[0].Data as ParameterZone;
                                                    string ObjectName = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[cond.ObjectInfo])})";
                                                    evntIfStatement += $"NumInZone({ObjectName}, {zone.X1}, {zone.Y1}, {zone.X2}, {zone.Y2}, {evntCount})";
                                                    break;
                                                }
                                            case -29: // Object is visible
                                            case -28: // Object is invisible
                                                evntIfStatement += GMS2Conditions.IsVisible(cond, evntCount, gameData);
                                                break;
                                            case -27: // Compare Alterable Value
                                                {
                                                    var whichValue = cond.Parameters[0].Data as ParameterShort;
                                                    var expression = cond.Parameters[1].Data as ParameterExpressions;
                                                    var CompareObject = gameData.FrameItems.Items[cond.ObjectInfo];
                                                    var ObjectName = GMS2Writer.CleanString(CompareObject.Name).Replace(" ", "_") + "_" + CompareObject.Header.Handle;
                                                    ObjectName = $"object({GMS2Writer.ObjectName(CompareObject)})";
                                                    evntIfStatement += $"CompareAltValue({ObjectName}, {whichValue.Value}, \"{GetOperator(expression.Comparison)}\", {GMS2Expressions.Evaluate(expression, evntCount, gameData, ExtCodes, ref missing_code)}, {evntCount})";
                                                    break;
                                                }
                                            case -23: // Is object overlapping a backdrop
                                                {
                                                    evntIfStatement += GMS2Conditions.OverlapBackdrop(cond, evntCount, gameData);
                                                    break;
                                                }
                                            case -22: // Object is getting closer than X pixels from window's edge
                                                {
                                                    var objName = GMS2Writer.CleanString(gameData.FrameItems.Items[cond.ObjectInfo].Name).Replace(" ", "_") + "_" + gameData.FrameItems.Items[cond.ObjectInfo].Header.Handle;
                                                    objName = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[cond.ObjectInfo])})";
                                                    var pixels = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                    evntIfStatement += $"CloseToWindowEdge({objName}, {pixels}, {evntCount})";
                                                    break;
                                                }
                                            //case -21: // Object has reached the end of its path (FIX/REWORK add this)
                                            //case -20: // Object has reached a node in the path (FIX/REWORK add this)
                                            // Path movement of Object has reached node "name" (FIX/REWORK add this)
                                            case -17: // Compare XPos
                                            case -16: // Compare YPos
                                                {
                                                    evntIfStatement += GMS2Conditions.CompareXorYPos(cond, evntCount, gameData, ExtCodes, ref missing_code);
                                                    break;
                                                }
                                            //case -15: // Compare Speed (FIX/REWORK add this)
                                            case -14: // Collides with another object  (OnlyOneActionWhenEventLoops)
                                                {
                                                    notAlways = true;
                                                    evntIfStatement += GMS2Conditions.OverlapObject(cond, evntCount, gameData);
                                                    break;
                                                }
                                            case -13: // Collides with a backdrop (OnlyOneActionWhenEventLoops)
                                                {
                                                    notAlways = true;
                                                    evntIfStatement += GMS2Conditions.OverlapBackdrop(cond, evntCount, gameData);
                                                    break;
                                                }
                                            case -12: // Leaves the play area (OnlyOneActionWhenEventLoops)
                                                {
                                                    notAlways = true;
                                                    var objName = GMS2Writer.CleanString(gameData.FrameItems.Items[cond.ObjectInfo].Name).Replace(" ", "_") + "_" + gameData.FrameItems.Items[cond.ObjectInfo].Header.Handle;
                                                    objName = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[cond.ObjectInfo])})";
                                                    evntIfStatement += $"CloseToWindowEdge({objName}, -1, {evntCount})";
                                                    break;
                                                }
                                            case -8: // Object is facing direction
                                                {
                                                    string direction = "";
                                                    string objName = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[cond.ObjectInfo])})";
                                                    if (cond.Parameters[0].Data is ParameterExpressions dirExp) direction = GMS2Expressions.Evaluate(dirExp, evntCount, gameData, ExtCodes, ref missing_code);
                                                    else if (cond.Parameters[0].Data is ParameterInt dirInt) direction = dirInt.Value.ToString();
                                                    evntIfStatement += $"{objName}.direction == ({direction})*11.25";
                                                    break;
                                                }
                                            case -7: // Is movement stopped?
                                                {
                                                    var objName = GMS2Writer.CleanString(gameData.FrameItems.Items[cond.ObjectInfo].Name).Replace(" ", "_") + "_" + gameData.FrameItems.Items[cond.ObjectInfo].Header.Handle;
                                                    objName = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[cond.ObjectInfo])})";
                                                    evntIfStatement += $"IsStopped({objName}, {evntCount})";
                                                    break;
                                                }
                                            case -4: // Is object overlapping another object (Fix/rework IN GML to target individual obj2 instances)
                                                {
                                                    evntIfStatement += GMS2Conditions.OverlapObject(cond, evntCount, gameData);
                                                    break;
                                                }
                                            case -3: // Is an animation playing?
                                                {
                                                    var Object = gameData.FrameItems.Items[cond.ObjectInfo];
                                                    var ObjectName = GMS2Writer.CleanString(Object.Name).Replace(" ", "_") + "_" + Object.Header.Handle;
                                                    ObjectName = $"object({GMS2Writer.ObjectName(Object)})";
                                                    var changeAnim = "";
                                                    if (cond.Parameters[0].Data is ParameterExpressions) changeAnim = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                    else changeAnim = (cond.Parameters[0].Data as ParameterShort).Value.ToString();
                                                    evntIfStatement += $"IsAnimPlaying({ObjectName}, {changeAnim}, {evntCount})";
                                                    break;
                                                }
                                            case -2: // Has an animation finished? (FIX/REWORK NOTE: might be buggy at last frame)
                                                {
                                                    var Object = gameData.FrameItems.Items[cond.ObjectInfo];
                                                    var ObjectName = GMS2Writer.CleanString(Object.Name).Replace(" ", "_") + "_" + Object.Header.Handle;
                                                    ObjectName = $"object({GMS2Writer.ObjectName(Object)})";
                                                    var finAnim = "";
                                                    if (cond.Parameters[0].Data is ParameterExpressions) finAnim = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                    else finAnim = (cond.Parameters[0].Data as ParameterShort).Value.ToString();
                                                    evntIfStatement += $"HasAnimFinished({ObjectName}, {finAnim}, {evntCount})";
                                                    break;
                                                }
                                            case -1: // Compare to Animation Frame
                                                {
                                                    var Object = gameData.FrameItems.Items[cond.ObjectInfo];
                                                    var ObjectName = GMS2Writer.CleanString(Object.Name).Replace(" ", "_") + "_" + Object.Header.Handle;
                                                    ObjectName = $"object({GMS2Writer.ObjectName(Object)})";
                                                    var compareFrame = "";
                                                    if (cond.Parameters[0].Data is ParameterExpressions) compareFrame = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                    else compareFrame = (cond.Parameters[0].Data as ParameterShort).Value.ToString();
                                                    evntIfStatement += $"CompareAnimFrame({ObjectName}, {compareFrame}, {evntCount})";
                                                    break;
                                                }

                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                        break;
                                    }
                                case 7: // Counters -- NOTE: every condition here MUST be accomodated to work with EACH instance of the counter individually
                                    {
                                        // set counter name
                                        string CounterName = "ZEL_UNKNOWN_OBJECT";
                                        var Counter = gameData.FrameItems.Items[cond.ObjectInfo];
                                        if (cond.ObjectInfo <= gameData.FrameItems.Items.Count)
                                            CounterName = GMS2Writer.CleanString(Counter.Name).Replace(" ", "_") + "_" + Counter.Header.Handle;
                                        CounterName = $"object({GMS2Writer.ObjectName(Counter)})";

                                        switch (cond.Num)
                                        {
                                            case -81: // Compare counter to value
                                                {
                                                    var compareCounterParam = cond.Parameters[0].Data as ParameterExpressions;
                                                    string compareValue = GMS2Expressions.Evaluate(compareCounterParam, evntCount, gameData, ExtCodes, ref missing_code);
                                                    //if (compareCounterParam.Items[0].Num == 0) compareValue = (compareCounterParam.Items[0].Data as LongExp).Value.ToString(); // If expression value is Long
                                                    //if (compareCounterParam.Items[0].Num == 23) compareValue = (compareCounterParam.Items[0].Data as DoubleExp).FloatValue.ToString(); // If expression value is Double
                                                    evntIfStatement += $"CompareCounter({CounterName}, \"{GetOperator(compareCounterParam.Comparison)}\", {compareValue}, {evntCount})";
                                                    break;
                                                }
                                            case -7: // Is stopped
                                                evntIfStatement += $"IsStopped({CounterName}, {evntCount})";
                                                break;
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        #region ExtensionConditions
                                        if (cond.ObjectType == GMS2Expressions.try_val("2YOJ", ExtCodes)) // Joystick 2 Object
                                        {
                                            switch (cond.Num)
                                            {
                                                case -88: // Any Button on Joystick _ is pressed
                                                    {
                                                        var device = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"gamepad_button_check_pressed(({device}) - 1, gp_anybutton(({device}) - 1))";
                                                        break;
                                                    }
                                                case -85: // Joystick _ Button _ is pressed
                                                    {
                                                        var device = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var buttonIndex = GMS2Expressions.Evaluate(cond.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"gamepad_button_check_pressed(({device}) - 1, gp_const({buttonIndex}))";
                                                        break;
                                                    }
                                                case -81: // Repeat While Button Pressed
                                                    {
                                                        var device = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var buttonIndex = GMS2Expressions.Evaluate(cond.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"gamepad_button_check(({device}) - 1, gp_const({buttonIndex}))";
                                                        break;
                                                    }
                                                default:
                                                    Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                    break;
                                            }
                                            break;
                                        }
                                        if (cond.ObjectType == GMS2Expressions.try_val("GMS2", ExtCodes)) // GMS2 Object
                                        {
                                            switch (cond.Num)
                                            {
                                                case -81: // Is runtime running as...
                                                    var osType = GMS2Expressions.Evaluate(cond.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code).Replace("\"", "");
                                                    evntIfStatement += $"os_type == {osType}";
                                                    break;
                                                default:
                                                    Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                    break;
                                            }
                                            break;
                                        }
                                        #endregion
                                        switch (cond.Num)
                                        {
                                            default:
                                                Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                                                break;

                                        }

                                    }

                                    break;

                            }
                            condCount++;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogType(typeof(EventsToGML), $"Problem reading condition ({frameCount}-{evntCount}): {ex}");
                            Throw.unimplemented(cond, ref evntIfStatement, gameData, ref missing_code); ;
                            condCount++;
                        }

                    }
                    // Close if-statement, begin actions
                    if (!groupEnd) evntIfStatement += ") {\n\t";
                    int actionCount = 1;

                    if (notAlways) evntIfStatement += $"if (notAlways[{evntCount}]) == true {{\n\t\t";
                    foreach (var action in evnt.Actions)
                    {
                        try
                        {
                            switch (action.ObjectType)
                            {
                                case -6: // Keyboard/Mouse
                                    switch (action.Num)
                                    {
                                        case 0: // Hide Windows mouse pointer (FIX/REWORK add this)
                                        case 1: // Show Windows mouse pointer (FIX/REWORK add this)
                                            {
                                                string[] cr = { "cr_none", "cr_default" };
                                                evntIfStatement += $"window_set_cursor({cr[action.Num]});";
                                                break;
                                            }
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code);
                                            break;
                                    }
                                    break;
                                case -5: // Create
                                    switch (action.Num)
                                    {
                                        case 0: // Create object (normal) -- (FIX/REWORK to work with EACH instance)
                                            var create = action.Parameters[0].Data as ParameterCreate;
                                            var createdObject = gameData.FrameItems.Items[create.ObjectInfo];
                                            var objectName = GMS2Writer.CleanString(createdObject.Name).Replace(" ", "_") + "_" + createdObject.Header.Handle;
                                            objectName = GMS2Writer.ObjectName(createdObject);
                                            evntIfStatement += $"CreateObject({objectName}, {create.X}, {create.Y}, {evntCount}, ";
                                            if (create.TypeParent == 2)
                                            {
                                                var parentObj = gameData.FrameItems.Items[create.ObjectInfoParent];
                                                var parentName = GMS2Writer.CleanString(parentObj.Name).Replace(" ", "_") + "_" + parentObj.Header.Handle;
                                                parentName = $"object({GMS2Writer.ObjectName(parentObj)})";
                                                evntIfStatement += $"-1, 2, {parentName});";
                                            }
                                            else
                                            {
                                                evntIfStatement += $"{create.Layer});";
                                            }
                                            evntIfStatement += $" // Create obj (incomplete)";
                                            break;
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                            break;
                                    }
                                    break;

                                case -4: // Timer
                                    switch (action.Num)
                                    {
                                        case 0: // Set timer to XX"-XX
                                            var setTimer = action.Parameters[0].Data as ParameterTimer;
                                            var setTimerVal = (setTimer.Timer / 1000.0);
                                            evntIfStatement += $"//Set timer\n\ttimer = {setTimerVal};\n\t";
                                            evntIfStatement += $"for(var i = 0; i < array_length(equalsTimers); i++) {{\n\t\tequalsTimers[i][1] = clamp(equalsTimers[i][0] - {setTimerVal}, 0, equalsTimers[i][0]);\n\t}}";
                                            break;
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                            break;
                                    }
                                    break;
                                case -3: // Storyboard controls
                                    switch (action.Num)
                                    {
                                        case 0: // Next frame
                                            if (frameCount == gameData.Frames.Count - 1)
                                            {
                                                evntIfStatement += "game_end(); // room_goto_next(); Cannot goto next room if final room";
                                            }
                                            else
                                            {
                                                evntIfStatement += "room_goto_next();";
                                            }
                                            break;
                                        case 2: // Jump to Frame (!!!FIX/REWORK add compatibility for Expressions here)
                                            try
                                            {
                                                int toFrame = (action.Parameters[0].Data as ParameterShort).Value + 1;
                                                int frameId = -1;
                                                foreach (Frame f in gameData.Frames)
                                                {
                                                    if (f.Handle == toFrame) frameId = f.Handle; break;
                                                }
                                                evntIfStatement += $"room_goto(rm{toFrame}_{GMS2Writer.CleanStringFull(gameData.Frames[toFrame].FrameName)}); // Jump to Frame ({toFrame})";
                                                break;
                                            }
                                            catch // If frame is missing or errored somehow
                                            {
                                                evntIfStatement += $"// Jump to frame (missing frame)";
                                                break;
                                            }
                                        case 4: // End Application
                                            evntIfStatement += "game_end();";
                                            break;
                                        case 5: // Restart Application
                                            evntIfStatement += "game_restart();";
                                            break;
                                        case 7: // Center Display at Object Position (FIX/REWORK to work with EACH instance)
                                        case 8: // Center Display at X=
                                            {
                                                evntIfStatement += GMS2Actions.CenterDisplay(action, evntCount, gameData, ExtCodes, ref missing_code);
                                                break;
                                            }
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                            break;
                                    }
                                    break;
                                case -2: // Sound
                                    if (GMS2Writer.Args["-nosnds"]) throw new Exception("Ignoring sound action");
                                    switch (action.Num)
                                    {
                                        case 0: // Play Sample
                                            {
                                                var SampleName = (action.Parameters[0].Data as ParameterSample).Name;
                                                evntIfStatement += $"audio_play_sound(snd_{GMS2Writer.CleanStringFull(SampleName)}, 1, false);";
                                                Convert.ToInt16("5");
                                                break;
                                            }
                                        case 1: // Stop Any Sample Playing
                                            evntIfStatement += $"audio_stop_all();";
                                            break;
                                        case 4: // Play and loop sample X times (fix/rework to play the exact number of times)
                                            {
                                                var SampleName = (action.Parameters[0].Data as ParameterSample).Name;
                                                evntIfStatement += $"audio_play_sound(snd_{GMS2Writer.CleanStringFull(SampleName)}, 1, true);";
                                                break;
                                            }
                                        case 6: // Stop sample
                                            {
                                                var SampleName = (action.Parameters[0].Data as ParameterSample).Name;
                                                evntIfStatement += $"audio_stop_sound(snd_{GMS2Writer.CleanStringFull(SampleName)});";
                                                break;
                                            }
                                        case 11: // Play Sound on Channel
                                            {
                                                var SampleName = (action.Parameters[0].Data as ParameterSample).Name;
                                                var channel = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                evntIfStatement += $"audio_play_sound_on(variable_global_get(\"channel\"+string({channel})), snd_{GMS2Writer.CleanStringFull(SampleName)}, false, 10);";
                                                break;
                                            }
                                        case 12: // Play and Loop sound on Channel (fix/rework to play the exact number of times)
                                            {
                                                var SampleName = (action.Parameters[0].Data as ParameterSample).Name;
                                                var channel = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                evntIfStatement += $"audio_play_sound_on(variable_global_get(\"channel\"+string({channel})), snd_{GMS2Writer.CleanStringFull(SampleName)}, true, 10);";
                                                break;
                                            }
                                        case 17: // Set Volume of Channel
                                            {
                                                var channel = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                var vol = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                evntIfStatement += $"audio_emitter_gain(variable_global_get(\"channel\"+string({channel})), ({vol})/100.0);";
                                                break;
                                            }
                                        case 24: // Pause all sounds
                                            evntIfStatement += $"audio_pause_all();";
                                            break;
                                        case 25: // Resume all sounds
                                            evntIfStatement += $"audio_resume_all();";
                                            break;
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code);
                                            break;
                                    }
                                    break;
                                case -1: // Special
                                    {
                                        switch (action.Num)
                                        {
                                            case 1: // Skip (idk when this appears, found it on the FRL Trello)
                                            case 0: // Skip (appears at groups)
                                                break;
                                            case 6: // Activate group _____
                                            case 7: // Deactivate group _____
                                                {
                                                    var g = action.Parameters[0].Data as ParameterGroupPointer;
                                                    evntIfStatement += $"groups[{g.ID}] = ";
                                                    evntIfStatement += $"{(!Convert.ToBoolean(action.Num - 6)).ToString().ToLower()};";
                                                    break;
                                                }
                                            default:
                                                Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code);
                                                break;
                                        }
                                        break;
                                    }
                                case 2: // Object -- NOTE: every action here MUST be accomodated to work with EACH instance of the object individually
                                    //string ObjectName = $"Qualifier {action.ObjectInfo}";
                                    string ObjectName = "ZEL_UNKNOWN_OBJECT";
                                    var Object = gameData.FrameItems.Items[action.ObjectInfo];
                                    if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                                        ObjectName = GMS2Writer.CleanString(Object.Name).Replace(" ", "_") + "_" + Object.Header.Handle;
                                    ObjectName = $"object({GMS2Writer.ObjectName(Object)})";

                                    switch (action.Num)
                                    {
                                        case 1: // Set Position at (X,X)
                                        case 2: // Set X Position
                                        case 3: // Set Y Position
                                            evntIfStatement += GMS2Actions.SetPosition(action, evntCount, gameData, ExtCodes, ref missing_code);
                                            break;
                                        case 4: // Stop movement
                                            evntIfStatement += $"SetMoving({ObjectName}, false, {evntCount});";
                                            break;
                                        case 5: // Start movement
                                            evntIfStatement += $"SetMoving({ObjectName}, true, {evntCount});";
                                            break;
                                        case 6: // Set movement speed
                                            var speed = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                            evntIfStatement += $"SetSpd({ObjectName}, {speed}, {evntCount});";
                                            break;
                                        //case 9: // Bounce (FIX/REWORK add this)
                                        //case 11: // Next movement (FIX/REWORK add this)
                                        case 14: // Look at (x,y) / Look at (x,y) from object -- FIX/REWORK IN GML to target individual obj2 instances
                                            var Object2 = action.Parameters[0].Data as ParameterPosition;
                                            if (Object2.TypeParent == 2) // Look at Position relative to an object
                                            {
                                                var Object2Name = GMS2Writer.CleanString(gameData.FrameItems.Items[Object2.ObjectInfoParent].Name).Replace(" ", "_") + "_" + gameData.FrameItems.Items[Object2.ObjectInfoParent].Header.Handle;
                                                Object2Name = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[Object2.ObjectInfoParent])})";
                                                evntIfStatement += $"LookAt({ObjectName}, {Object2Name}.x + {Object2.X}, {Object2Name}.y + {Object2.Y}, {evntCount});";
                                            }
                                            else // Look at exact X and Y position
                                            {
                                                evntIfStatement += $"LookAt({ObjectName}, {Object2.X}, {Object2.Y}, {evntCount});";
                                            }
                                            break;
                                        case 17: // Change animation sequence
                                            string changeAnim = "";
                                            if (action.Parameters[0].Data is ParameterExpressions) changeAnim = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                            else changeAnim = (action.Parameters[0].Data as ParameterShort).Value.ToString();
                                            evntIfStatement += $"ChangeAnimSequence({ObjectName}, {changeAnim}, {evntCount});";
                                            break;
                                        case 23: // Set direction
                                            string direction = "";
                                            if (action.Parameters[0].Data is ParameterExpressions dirExp) direction = GMS2Expressions.Evaluate(dirExp, evntCount, gameData, ExtCodes, ref missing_code);
                                            else if (action.Parameters[0].Data is ParameterInt dirInt) direction = dirInt.Value.ToString();
                                            //evntIfStatement += $"{ObjectName}.direction = ({direction})*11.25";
                                            evntIfStatement += $"SetGeneralValue({ObjectName}, \"direction\", \"=\", round(({direction})*11.25), {evntCount});";
                                            break;
                                        case 24: // Destroy object(s)
                                            evntIfStatement += GMS2Actions.DestroyObject(action, evntCount, gameData);
                                            break;
                                        case 26: // Make invisible
                                        case 27: // Make reappear
                                            evntIfStatement += GMS2Actions.SetVisibility(action, evntCount, gameData);
                                            break;
                                        case 28: // Flash during XX"-XX
                                            evntIfStatement += GMS2Actions.FlashObject(action, evntCount, gameData, ExtCodes, ref missing_code);
                                            break;
                                        //case 29: Launch object in direction with speed _ (FIX/REWORK add this)
                                        //case 30: Launch object1 towards object2 with speed _ (FIX/REWORK add this)
                                        case 31: // Set Alterable Value
                                        case 32: // Add to Alterable Value
                                        case 33: // Subtract from Alterable Value
                                            evntIfStatement += GMS2Actions.SetAltValue(action, evntCount, gameData, ExtCodes, ref missing_code);
                                            break;
                                        case 57: // Bring to front
                                            evntIfStatement += GMS2Actions.BringToFront(action, evntCount, gameData);
                                            break;
                                        case 61: // Move to layer _
                                            string layer = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                            evntIfStatement += $"MoveToLayer({ObjectName}, {layer}, {evntCount});";
                                            break;
                                        //case 63: // Set effect to _ (FIX/REWORK add this)
                                        case 39: // Set semi-transparency (compatibility)
                                        case 65: // Set alpha-blending Coefficient
                                            evntIfStatement += GMS2Actions.SetAlpha(action, evntCount, gameData, ExtCodes, ref missing_code);
                                            break;
                                        //case 80: // Paste into background (not an obstalce) (FIX/REWORK add this and also research it properly for the different backdrop modes)
                                        case 88: // Set Angle to _
                                            string angle = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                            evntIfStatement += $"SetAngle({ObjectName}, {angle}, {evntCount})";
                                            break;
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                            break;
                                    }
                                    break;
                                case 3: // String -- NOTE: every action here MUST be accomodated to work with EACH instance of the string individually
                                    {
                                        string StringName = "ZEL_UNKNOWN_OBJECT";
                                        var String = gameData.FrameItems.Items[action.ObjectInfo];
                                        if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                                            StringName = GMS2Writer.CleanString(String.Name).Replace(" ", "_") + "_" + String.Header.Handle;
                                        StringName = $"object({GMS2Writer.ObjectName(String)})";
                                        switch (action.Num)
                                        {
                                            case 24: // Destroy
                                                evntIfStatement += GMS2Actions.DestroyObject(action, evntCount, gameData);
                                                break;
                                            case 26: // Make invisible
                                            case 27: // Make reappear
                                                evntIfStatement += GMS2Actions.SetVisibility(action, evntCount, gameData);
                                                break;
                                            case 28: // Flash during XX"-XX
                                                evntIfStatement += GMS2Actions.FlashObject(action, evntCount, gameData, ExtCodes, ref missing_code);
                                                break;
                                            case 57: // Bring to front
                                                evntIfStatement += GMS2Actions.BringToFront(action, evntCount, gameData);
                                                break;
                                            case 84: // Set paragraph to _
                                            case 85: // Display previous paragraph
                                            case 86: // Display next paragraph
                                                evntIfStatement += GMS2Actions.SetParagraph(action, evntCount, gameData, ExtCodes, ref missing_code);
                                                break;
                                            case 87: // Display alterable string (is this broken in real Fusion? lol it just doesn't work for me somehow)
                                                evntIfStatement += $"DisplayAlterableString({StringName}, {evntCount});";
                                                break;
                                            case 88: // Change alterable string
                                                string strText = "";
                                                strText = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                evntIfStatement += $"SetAlterableString({StringName}, {strText}, {evntCount});";
                                                break;
                                            default:
                                                Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code);
                                                break;
                                        }

                                        break;
                                    }
                                case 7: // Counter -- NOTE: every action here MUST be accomodated to work with EACH instance of the counter individually
                                    string CounterName = "ZEL_UNKNOWN_OBJECT";
                                    var Counter = gameData.FrameItems.Items[action.ObjectInfo];
                                    if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                                        CounterName = GMS2Writer.CleanString(Counter.Name).Replace(" ", "_") + "_" + Counter.Header.Handle;
                                    CounterName = $"object({GMS2Writer.ObjectName(Counter)})";

                                    switch (action.Num)
                                    {
                                        case 24: // Destroy
                                            evntIfStatement += $"DestroyObject({CounterName}, {evntCount});";
                                            break;
                                        case 26: // Make invisible
                                        case 27: // Reappear
                                            evntIfStatement += GMS2Actions.SetVisibility(action, evntCount, gameData);
                                            break;
                                        case 28: // Flash during XX"-XX
                                            evntIfStatement += GMS2Actions.FlashObject(action, evntCount, gameData, ExtCodes, ref missing_code);
                                            break;
                                        case 57: // Bring to front
                                            evntIfStatement += GMS2Actions.BringToFront(action, evntCount, gameData);
                                            break;
                                        /*
                                        case 80: // Set counter to value
                                            var setCounterParam = action.Parameters[0].Data as ParameterExpressions;
                                            evntIfStatement += $"{CounterName}.cntrvalue = clamp({GMS2Expressions.Evaluate(setCounterParam, evntCount, gameData, ExtCodes, ref missing_code)}, {CounterName}.minval, {CounterName}.maxval);";
                                            break;
                                        case 81: // Add value to counter
                                            var addCounterParam = action.Parameters[0].Data as ParameterExpressions;
                                            evntIfStatement += $"{CounterName}.cntrvalue = clamp({CounterName}.cntrvalue + {GMS2Expressions.Evaluate(addCounterParam, evntCount, gameData, ExtCodes, ref missing_code)}, {CounterName}.minval, {CounterName}.maxval);";
                                            break;
                                        case 82: // Subtract from value
                                            var subCounterParam = action.Parameters[0].Data as ParameterExpressions;
                                            evntIfStatement += $"{CounterName}.cntrvalue = clamp({CounterName}.cntrvalue - {GMS2Expressions.Evaluate(subCounterParam, evntCount, gameData, ExtCodes, ref missing_code)}, {CounterName}.minval, {CounterName}.maxval);";
                                            break;
                                        */
                                        case 80: // Set counter to value
                                        case 81: // Add value to counter
                                        case 82: // Subtract from value
                                            string setVal = "";
                                            if (action.Num > 80) setVal = $"CounterValue({CounterName},{evntCount}) + ";
                                            if (action.Num == 82) setVal = setVal[..^2] + "- ";
                                            setVal += GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                            evntIfStatement += $"SetCounter({CounterName}, {setVal}, {evntCount});";
                                            break;
                                        default:
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                            break;
                                    }
                                    break;
                                default:
                                    {
                                        #region ExtensionActions
                                        if (action.ObjectType == GMS2Expressions.try_val("0I", ExtCodes) || action.ObjectType == GMS2Expressions.try_val("0INI", ExtCodes)) // Ini object
                                        {
                                            var ini_name = GMS2Writer.ObjectName(gameData.FrameItems.Items[action.ObjectInfo]);
                                            string pref = $"with({ini_name}){{"; // Target specific INI object
                                            switch (action.Num)
                                            {
                                                case 80: // Set current group to "GroupName"
                                                    {
                                                        var GroupName = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetCurrentGroup({GroupName})";
                                                        break;
                                                    }
                                                case 81: // Set current item to "ItemName"
                                                    {
                                                        var ItemName = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetCurrentItem({ItemName})";
                                                        break;
                                                    }
                                                case 82: // Set value
                                                    {
                                                        var Value = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetValue({Value})";
                                                        break;
                                                    }
                                                case 83: // Save position of object
                                                    {
                                                        var ObjName = GMS2Writer.ObjectName(gameData.FrameItems.Items[(action.Parameters[0].Data as ParameterObject).ObjectInfo]);
                                                        evntIfStatement += $"{pref}SavePosition({ObjName})";
                                                        break;
                                                    }
                                                case 84: // Load position of object
                                                    {
                                                        var ObjName = GMS2Writer.ObjectName(gameData.FrameItems.Items[(action.Parameters[0].Data as ParameterObject).ObjectInfo]);
                                                        evntIfStatement += $"{pref}LoadPosition({ObjName})";
                                                        break;
                                                    }
                                                case 85: // Set string
                                                    {
                                                        var String = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetString({String})";
                                                        break;
                                                    }
                                                case 86: // Set current file to "FileName"
                                                    {
                                                        var FileName = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetCurrentFile({FileName})";
                                                        break;
                                                    }
                                                case 87: // Set value (item)
                                                    {
                                                        var Item = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var Value = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetValueItem({Item}, {Value})";
                                                        break;
                                                    }
                                                case 88: // Set value (group - item)
                                                    {
                                                        var Group = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var Item = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var Value = GMS2Expressions.Evaluate(action.Parameters[2].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetValueGroupItem({Group}, {Item}, {Value})";
                                                        break;
                                                    }
                                                case 89: // Set string (item)
                                                    {
                                                        var Item = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var String = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetStringItem({Item}, {String})";
                                                        break;
                                                    }
                                                case 90: // Set string (group - item);
                                                    {
                                                        var Group = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var Item = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var String = GMS2Expressions.Evaluate(action.Parameters[2].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}SetStringGroupItem({Group}, {Item}, {String})";
                                                        break;
                                                    }
                                                case 91: // Delete item
                                                    {
                                                        var ItemName = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}DeleteItem({ItemName})";
                                                        break;
                                                    }
                                                case 92: // Delete item (group)
                                                    {
                                                        var GroupName = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        var ItemName = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}DeleteGroupItem({GroupName}, {ItemName})";
                                                        break;
                                                    }
                                                case 93: // Delete group
                                                    {
                                                        var GroupName = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evntCount, gameData, ExtCodes, ref missing_code);
                                                        evntIfStatement += $"{pref}DeleteGroup({GroupName})";
                                                        break;
                                                    }

                                                default:
                                                    Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                                    break;
                                            }
                                            evntIfStatement += "};";
                                        }
                                        #endregion
                                        else
                                        {
                                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;
                                        }
                                    }
                                    break;
                            }

                            if (!(action.ObjectType == -1 && action.Num > 1)) evntIfStatement += "\n\t";
                            if (notAlways) evntIfStatement += "\t";
                        }
                        catch (Exception ex)
                        {
                            Logger.LogType(typeof(EventsToGML), $"Problem reading action ({frameCount}-{evntCount}): {ex}");
                            Throw.unimplemented(action, ref evntIfStatement, gameData, ref missing_code); ;

                            if (!(action.ObjectType == -1 && action.Num > 1)) evntIfStatement += "\n\t";
                            if (notAlways) evntIfStatement += "\t";
                        }
                        actionCount++;
                    }
                    // End entire line
                    if (notAlways) evntIfStatement += $"\n\t\tnotAlways[{evntCount}] = false;\n\t}}";
                    if (runOnce) evntIfStatement += $"\n\trunOnce[{evntCount}] = false;";
                    if (!groupStart) evntIfStatement += "\n}";
                    if (!groupEnd) evntIfStatement += "\n";
                    if (notAlways) evntIfStatement += $"else {{\n\tnotAlways[{evntCount}] = true;\n}}";
                    evntIfStatement = evntIfStatement.Replace("\n", "\n" + string.Concat(Enumerable.Repeat("\t", indents)));
                    if (groupEnd) evntIfStatement += $"#endregion /*ev{evntCount}*/\n";

                    evntIfStatement += "\n";

                    // Append GML
                    if (startOfFrame)
                    {
                        roomStartGML.code += evntIfStatement;
                        stepGML.code += $"/*ev{evntCount}*/ // Start of Frame... in Room Start Event\n\n";
                    }
                    else if (endOfFrame)
                    {
                        roomendGML.code += evntIfStatement;
                        stepGML.code += $"/*ev{evntCount}*/ // End of Frame... in Room End Event\n\n";
                    }
                    else if (endOfApp)
                    {
                        gameendGML.code += evntIfStatement;
                        stepGML.code += $"/*ev{evntCount}*/ // End of Application... in Game End Event\n\n";
                    }
                    else
                    {
                        stepGML.code += evntIfStatement;
                    }
                    evntCount++;
                }
                // Create Variables in Create GML
                createGML.code = createGML.code.Replace("<GROUPCOUNT>", groupCount.ToString());
                foreach (var item in everyTimers)
                {
                    createVariables.Add($"{item[0]} = [{item[1]}, {item[2]}];");
                }
                foreach (var item in equalsTimers)
                {
                    createVariables.Add($"{item[0]} = [{item[1]}, {item[2]}];");
                }
                foreach (var item in createVariables)
                {
                    createGML.code += $"{item}\n";
                }
                createGML.code += $"everyTimers = [";
                int i = 0;
                foreach (var item in everyTimers)
                {
                    createGML.code += $"\n\t{item[0]}";
                    if (i < everyTimers.Count - 1)
                    {
                        createGML.code += ",";
                    }
                    i++;
                }
                createGML.code += "\n];\n";

                createGML.code += $"equalsTimers = [";
                int j = 0;
                foreach (var item in equalsTimers)
                {
                    createGML.code += $"\n\t{item[0]}";
                    if (j < equalsTimers.Count - 1)
                    {
                        createGML.code += ",";
                    }
                    j++;
                }
                createGML.code += "\n];\n";

                createGML.code += $"eventInstLists = [];";


                createGML.code += "\n\n" + createCode;
                if (!gameData.AppHeader.NewFlags["PlaySoundsOverFrames"]) // Play Samples Over Frames game setting
                {
                    createGML.code += "\n// Don't play samples over frames\naudio_stop_all();";
                }

                // Reset everyTimers in Step GML
                stepGML.code += $"\n\n// Reset everyTimers\nfor (var i = 0; i < array_length(everyTimers); i++) {{\n\tif (everyTimers[i][1] == 0) then everyTimers[i][1] = everyTimers[i][0];\n}}\n";

                // Stop equalsTimers in Step GML
                stepGML.code += $"\n\n// Stop equalsTimers\nfor (var i = 0; i < array_length(equalsTimers); i++) {{\n\tif (equalsTimers[i][1] == 0) then equalsTimers[i][1] = -1;\n}}\n";


                newObj.eventList = evtList.ToArray();
                gmlFiles.Add(createGML);
                gmlFiles.Add(stepGML);
                gmlFiles.Add(roomStartGML);
                gmlFiles.Add(roomendGML);
                gmlFiles.Add(gameendGML);
                gmlFiles.Add(gamestartGML);


                objects.Add(newObj);
                CTF_objectIDs.Add(projResources.Count); // Save object ID

                var newObjectRes = new ProjectYYP.Resource();
                var newObjectResID = new ProjectYYP.ResourceID();

                newObjectResID.name = newObj.name;
                newObjectResID.path = $"objects/{newObj.name}/{newObj.name}.yy";
                newObjectRes.id = newObjectResID;
                newObjectRes.order = GMS2Writer.ObjectOrder;
                GMS2Writer.ObjectOrder++;
                projResources.Add(newObjectRes);




                List<RoomYY.Instance> instancesList = rooms[frameCount].layers[0].instances.ToList<RoomYY.Instance>();
                List<RoomYY.InstanceCreationOrder> instancesOrderList = rooms[frameCount].instanceCreationOrder.ToList<RoomYY.InstanceCreationOrder>();

                var newInstance = new RoomYY.Instance();
                newInstance.name = $"inst_{GMS2Writer.NewInstanceID()}";
                newInstance.x = 0;
                newInstance.y = -64;
                newInstance.colour = 4294967295;

                var objectID = new RoomYY.ObjectID();
                objectID.name = newObj.name;
                objectID.path = newObjectResID.path;
                newInstance.objectId = objectID;
                instancesList.Add(newInstance);

                var instanceCreation = new RoomYY.InstanceCreationOrder();
                instanceCreation.name = newInstance.name;
                instanceCreation.path = $"rooms/{rooms[frameCount].name}/{rooms[frameCount].name}.yy";
                instancesOrderList.Insert(0, instanceCreation);


                var camInstance = new RoomYY.Instance();
                camInstance.name = $"inst_{GMS2Writer.NewInstanceID()}";
                camInstance.x = 32;
                camInstance.y = -64;
                camInstance.colour = 4294967295;

                var camObjectID = new RoomYY.ObjectID();
                camObjectID.name = frameCam.name;
                camObjectID.path = frameCamResID.path;
                camInstance.objectId = camObjectID;
                instancesList.Add(camInstance);

                var camInstanceCreation = new RoomYY.InstanceCreationOrder();
                camInstanceCreation.name = camInstance.name;
                camInstanceCreation.path = $"rooms/{rooms[frameCount].name}/{rooms[frameCount].name}.yy";
                instancesOrderList.Insert(1, camInstanceCreation);


                /*if (frameCount == 0) // Add Kotfile to first room/frame
                {
                    var kotfileInstance = new RoomYY.Instance();
                    kotfileInstance.name = $"inst_{GMS2Writer.NewInstanceID()}";
                    kotfileInstance.x = 64;
                    kotfileInstance.y = -64;
                    kotfileInstance.colour = 4294967295;

                    var kotfileObjectID = new RoomYY.ObjectID();
                    kotfileObjectID.name = kotfile.name;
                    kotfileObjectID.path = kotfileResID.path;
                    kotfileInstance.objectId = kotfileObjectID;
                    instancesList.Add(kotfileInstance);

                    var kotfileinstanceCreation = new RoomYY.InstanceCreationOrder();
                    kotfileinstanceCreation.name = kotfileinstanceCreation.name;
                    kotfileinstanceCreation.path = $"rooms/{rooms[frameCount].name}/{rooms[frameCount].name}.yy";
                    instancesOrderList.Add(kotfileinstanceCreation);
                }*/


                rooms[frameCount].layers[0].instances = instancesList.ToArray();
                rooms[frameCount].instanceCreationOrder = instancesOrderList.ToArray();




                // Count up current frame
                frameCount++;
            }

        }

        public class Throw
        {
            // https://youtu.be/OcOCx1MxYA8
            public static string missingCond(int ObjType = 5552471, int condNum = 5552471, string ID = "noID")
            {
                if (ObjType == 5552471 && condNum == 5552471)
                {
                    return "UNIMPLEMENTED_CONDITION";
                }
                else
                {
                    //Logger.Log(typeof(EventsToGML), $"UNIMPLEMENTED_CONDITION/*Obj {ObjType} ~ Cond {condNum}*/");
                    return $"UNIMPLEMENTED_CONDITION/*{ID} Obj {ObjType} ~ Cnd {condNum}*/";
                }
            }
            public static string missingAction(int ObjType = 5552471, int actNum = 5552471, string ID = "noID")
            {
                if (ObjType == 5552471 && actNum == 5552471)
                {
                    return "// UNIMPLEMENTED ACTION";
                }
                else
                {
                    //Logger.Log(typeof(EventsToGML), $"// UNIMPLEMENTED ACTION: Obj {ObjType} ~ Act {actNum}");
                    return $"// UNIMPLEMENTED ACTION:{ID} Obj {ObjType} ~ Act {actNum}";
                }
            }
            /* BACKUP Missing Condition

            evntIfStatement += Throw.missingCond(cond.ObjectType, cond.Num) + "/* ";
            foreach (var conditionItems in cond.Items)
            {
                evntIfStatement += $"[Loader:{conditionItems.Loader} ";
                if (conditionItems.Loader is ParameterExpressions ParameterExpressions)
                {
                    foreach (var expParam in ParameterExpressions.Items)
                    {
                        evntIfStatement += $"(ExpressParamLdr:{expParam.Loader} - {expParam.Num} {expParam.ObjectInfo} {expParam.ObjectInfoList} {expParam.ObjectType} {expParam.Unk1} {expParam.Unk2}) | ";

                    }

                }
                evntIfStatement += "], ";
            }
            evntIfStatement += "*/

            /* BACKUP Missing Action
            evntIfStatement += Throw.missingAction(action.ObjectType, action.Num) + " ";
            foreach (var actionItems in action.Items)
            {
                evntIfStatement += $"[Loader:{actionItems.Loader} ";
                if (actionItems.Loader is ParameterExpressions ExpressParam)
                {
                    foreach (var expressionParam in ExpressParam.Items)
                    {
                        evntIfStatement += $"(ExpressParamLdr:{expressionParam.Loader} - {expressionParam.Num} {expressionParam.ObjectInfo} {expressionParam.ObjectInfoList} {expressionParam.ObjectType} {expressionParam.Unk1} {expressionParam.Unk2}) | ";
                    }
                }
                evntIfStatement += "], ";
            }
            */

            public static void unimplemented(Condition cond, ref string evntIfStatement, PackageData gameData, ref List<string> missing_code)
            {

                var ItemsList = cond.Parameters;
                var ID = getID(cond.ObjectInfo, cond.ObjectType, gameData);
                string errStr = Throw.missingCond(cond.ObjectType, cond.Num, ID) + "/* ";
                //Logger.Log(typeof(EventsToGML), "69420 UNIMPLEMENTED CONDITION");
                writeitems(ItemsList, ref errStr, gameData);
                missing_code.Add(errStr);
                evntIfStatement += errStr;
                evntIfStatement += "*/";
            }
            public static void unimplemented(Action action, ref string evntIfStatement, PackageData gameData, ref List<string> missing_code)
            {

                var ItemsList = action.Parameters;
                var ID = getID(action.ObjectInfo, action.ObjectType, gameData);
                string errStr = Throw.missingAction(action.ObjectType, action.Num, ID) + " ";
                //Logger.Log(typeof(EventsToGML), "1337 UNIMPLEMENTED ACTION");
                writeitems(ItemsList, ref errStr, gameData);
                missing_code.Add(errStr);
                evntIfStatement += errStr;

            }
            public static void writeitems(Parameter[] ItemsList, ref string evntIfStatement, PackageData gameData)
            {
                foreach (var item in ItemsList)
                {
                    evntIfStatement += $"[Loader:{item.Data.GetType().Name} ";
                    if (item.Data is ParameterExpressions ExpressParam)
                    {
                        foreach (var expressionParam in ExpressParam.Expressions)
                        {
                            var ID = getID(expressionParam.ObjectInfo, expressionParam.ObjectType, gameData);
                            evntIfStatement += $"(ExpressParamLdr:{expressionParam.Expression.GetType().Name}{ID} - Obj_{expressionParam.ObjectType} Prm_{expressionParam.Num} OI_{expressionParam.ObjectInfo} OIL_{expressionParam.ObjectInfoList} Size_{expressionParam.Size}) | ";
                        }
                    }
                    evntIfStatement += "], ";
                }
            }
            public static string getID(ushort ObjInf, short ObjectType, PackageData gameData)
            {
                var ID = "";
                ObjectInfo Object = new ObjectInfo();
                try
                {
                    Object = gameData.FrameItems.Items[ObjInf];
                }
                catch{}
                if (Object.Properties is ObjectCommon common && ObjectType > 7) ID = $" \"{common.Identifier}\"";
                return ID;
            }
        }
    }
}
