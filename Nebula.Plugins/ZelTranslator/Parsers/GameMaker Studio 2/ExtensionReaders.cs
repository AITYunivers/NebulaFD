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
using static ZelTranslator_SD.Parsers.GameMakerStudio2.ObjectYY;
using System.Drawing;
using Color = System.Drawing.Color;
using Nebula.Core.Data.Chunks.BankChunks.Fonts;
using Font = Nebula.Core.Data.Chunks.BankChunks.Fonts.Font;
using Nebula.Core.Memory;

namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class ExtensionReaders
    {
        public class PFMO
        {
            public int MaxXVelocity;
            public int MaxYVelocity;
            public int XAcceleration;
            public int XDeceleration;
            public int Gravity;
            public int JumpStrength;
            public int JumpHold;
            public int MaxStepUp;
            public int SlopeCorrection;
            public bool ThroughCollisionTop;
            public bool JumpThrough;

            public void Read(ObjectInfo obj, ObjectYY.RootObject newObj, List<GMLFile> GMLFiles)
            {
                var common = obj.Properties as ObjectCommon;
                // Add identifier to object name
                newObj.name = "omfp_" + newObj.name;

                // Read extension data
                var reader = new ByteReader(common.ObjectExtension.ExtensionData);
                reader.Skip(8);
                MaxXVelocity = read_value(reader);
                MaxYVelocity = read_value(reader);
                XAcceleration = read_value(reader);
                XDeceleration = read_value(reader);
                Gravity = read_value(reader);
                JumpStrength = read_value(reader);
                JumpHold = read_value(reader);
                MaxStepUp = read_value(reader);
                SlopeCorrection = read_value(reader);
                ThroughCollisionTop = Convert.ToBoolean(reader.ReadByte());
                JumpThrough = Convert.ToBoolean(reader.ReadByte());

                // Events
                var createEv = new Event() { eventNum = 0, eventType = 0 };
                var stepEv = new Event() { eventNum = 0, eventType = 3 };
                var events = new List<Event>() { createEv, stepEv };
                var createEvFile = new GMLFile();
                var stepEvFile = new GMLFile();

                createEvFile.name = "Create_0";
                createEvFile.path = $"objects\\{newObj.name}";

                stepEvFile.name = "Step_0";
                stepEvFile.path = $"objects\\{newObj.name}";

                // Generate Values, Strings, Flags...
                GMS2Writer.WriteValues(obj, createEvFile);

                // Write PFMO variables
                createEvFile.code += $"\nmaxXVelocity = {MaxXVelocity};\n";
                createEvFile.code += $"maxYVelocity = {MaxYVelocity};\n";
                createEvFile.code += $"xAccel = {XAcceleration};\n";
                createEvFile.code += $"xDecel = {XDeceleration};\n";
                createEvFile.code += $"Gravity = {Gravity};\n";
                createEvFile.code += $"jumpStrength = {JumpStrength};\n";
                createEvFile.code += $"jumpHoldHeight = {JumpHold};\n";
                createEvFile.code += $"stepUp = {MaxStepUp};\n";
                createEvFile.code += $"slopeCorrection = {SlopeCorrection};\n";
                createEvFile.code += "\nthroughCollisionTop = " + $"{ThroughCollisionTop};\n".ToLower();
                createEvFile.code += "jumpThrough = " + $"{JumpThrough};\n".ToLower();
                createEvFile.code += "// set through action\naddXVelocity = 0;\naddYVelocity = 0;\n\n";

                createEvFile.code += "xMoveCount = 0;\nyMoveCount = 0;\n\n";

                createEvFile.code += "instance = noone;\n";
                createEvFile.code += "Paused = false;\n";
                createEvFile.code += "xVelocity = 0;\n";
                createEvFile.code += "yVelocity = 0;\n";
                createEvFile.code += "left = false;\n";
                createEvFile.code += "right = false;\n\n";

                createEvFile.code += "// Event stuff\n";
                createEvFile.code += "obstacle_collision = false;\n";
                createEvFile.code += "platform_collision = false;\n";
                createEvFile.code += "onGround = false;\n";

                //Step GML
                stepEvFile.code = "pmo_update(self);\nobstacle_collision = false;\nplatform_collision = false;";

                // Add events and GML
                newObj.eventList = events.ToArray();
                GMLFiles.Add(createEvFile);
                GMLFiles.Add(stepEvFile);
            }
            private int read_value(ByteReader reader)
            {
                try
                {
                    var val = reader.ReadAscii(16);
                    //Logger.Log($"String: {val}");
                    return int.Parse(val);
                }
                catch
                {
                    return -1;
                }
            }
        }
        public class Joystick2
        {
            public bool BackgroundInput;
            public bool MMFOrder;
            public bool DetectNewDevices;
            public bool PollEveryLoop;
            public bool TestForXbox;

            public void Read(ObjectInfo obj, ObjectYY.RootObject newObj, List<GMLFile> GMLFiles)
            {
                var common = obj.Properties as ObjectCommon;
                // Add identifier to object name
                newObj.name = "2YOJ_" + newObj.name;

                // Read extension data
                var reader = new ByteReader(common.ObjectExtension.ExtensionData);
                BackgroundInput = Convert.ToBoolean(reader.ReadInt32());
                MMFOrder = Convert.ToBoolean(reader.ReadInt32());
                DetectNewDevices = Convert.ToBoolean(reader.ReadInt32());
                PollEveryLoop = Convert.ToBoolean(reader.ReadInt32());
                TestForXbox = Convert.ToBoolean(reader.ReadInt32());

                dump_data(common, newObj.name);


                //Events
                var createEv = new Event() { eventNum = 0, eventType = 0 };
                var stepEv = new Event() { eventNum = 0, eventType = 3 };
                var events = new List<Event>();
                var createEvFile = new GMLFile();
                var stepEvFile = new GMLFile();

                events.Add(createEv);
                events.Add(stepEv);
                createEvFile.name = "Create_0";
                createEvFile.path = $"objects\\{newObj.name}";
                stepEvFile.name = "Step_0";
                stepEvFile.path = $"objects\\{newObj.name}";

                // Generate Values, Strings, Flags...
                GMS2Writer.WriteValues(obj, createEvFile);

                // Write Joystick2 variables 
                createEvFile.code += $"\nBackgroundInput = " + $"{BackgroundInput};\n".ToLower();
                createEvFile.code += $"MMFOrder = " + $"{MMFOrder};\n".ToLower();
                createEvFile.code += $"DetectNewDevices = " + $"{DetectNewDevices};\n".ToLower();
                createEvFile.code += $"PollEveryLoop = " + $"{PollEveryLoop};\n".ToLower();
                createEvFile.code += $"TestForXbox = " + $"{TestForXbox};\n".ToLower();

                // Add events and GML
                newObj.eventList = events.ToArray();
                GMLFiles.Add(createEvFile);
                GMLFiles.Add(stepEvFile);
            }
        }
        public class KcIni
        {
            public string iniName;

            public void Read(ObjectInfo obj, ObjectYY.RootObject newObj, List<GMLFile> GMLFiles)
            {
                var common = obj.Properties as ObjectCommon;
                // Add identifier to object name
                //newObj.name = "OINI_" + newObj.name;

                // Read extension data
                var reader = new ByteReader(common.ObjectExtension.ExtensionData);
                reader.Skip(2);
                iniName = reader.ReadYunicode();
                if (string.IsNullOrEmpty(iniName))
                    iniName = "Default.ini";
                //UTF8 = Convert.ToBoolean(reader.ReadInt32());
                //CreateInMMFApps = Convert.ToBoolean(reader.ReadInt32());
                //Win_ReadChanges = Convert.ToBoolean(reader.ReadInt32());
                //Win_WriteChanges = Convert.ToBoolean(reader.ReadInt32());

                dump_data(common, newObj.name);

                //Events
                var createEv = new Event() { eventNum = 0, eventType = 0 };
                var createEvFile = new GMLFile();

                

                createEvFile.name = "Create_0";
                createEvFile.path = $"objects\\{newObj.name}";

                // Write variables 
                createEvFile.code = $"object_name = \"{obj.Name}\";\niniName = \"{iniName}\";\nKcIni();";

                var stepEv = new Event() { eventNum = 0, eventType = 3 };
                var stepEvFile = new GMLFile();

                stepEvFile.name = "Step_0";
                stepEvFile.path = $"objects\\{newObj.name}";
                stepEvFile.code = $"if (save) saveIni();";

                // Add events and GML
                var events = new List<Event>() { createEv, stepEv };
                newObj.eventList = events.ToArray();
                GMLFiles.Add(createEvFile);
                GMLFiles.Add(stepEvFile);


            }
        }
        private static void dump_data(ObjectCommon common, string name)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Binary\\ExtensionData");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                File.WriteAllBytes($"{path}\\{name}_ExtensionData.bin", common.ObjectExtension.ExtensionData);
            }
            catch
            {

            }
        }
    }
}