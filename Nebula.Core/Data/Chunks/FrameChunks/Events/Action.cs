using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public class Action : ACEventBase
    {
        public Action()
        {
            ChunkName = "Action";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            if (NebulaCore.Fusion == 1.5f)
            {
                ObjectType = reader.ReadSByte();
                Num = reader.ReadSByte();
            }
            else
            {
                ObjectType = reader.ReadShort();
                Num = reader.ReadShort();
            }
            ObjectInfo = reader.ReadUShort();
            ObjectInfoList = reader.ReadShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
                Parameters[i].FrameEvents = Parent.Parent;
            }

            reader.Seek(endPosition);
            Fix((List<Action>)extraInfo[0]);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadUShort();
            ObjectInfoList = reader.ReadShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
                Parameters[i].FrameEvents = Parent.Parent;
            }

            reader.Seek(endPosition);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            if (FrameEvents.QualifierJumptable.ContainsKey(Tuple.Create(ObjectInfo, ObjectType)))
                ObjectInfo = FrameEvents.QualifierJumptable[Tuple.Create(ObjectInfo, ObjectType)];

            ByteWriter actWriter = new ByteWriter(new MemoryStream());
            actWriter.WriteShort(ObjectType);
            actWriter.WriteShort(Num);
            actWriter.WriteUShort(ObjectInfo);
            actWriter.WriteShort(ObjectInfoList);
            actWriter.WriteByte((byte)EventFlags.Value);
            actWriter.WriteByte((byte)OtherFlags.Value);
            actWriter.WriteByte((byte)Parameters.Length);
            actWriter.WriteByte(DefType);

            long startPosition = (long)extraInfo[0] + writer.Tell() + actWriter.Tell() + 4;
            foreach (Parameter parameter in Parameters)
                parameter.WriteMFA(actWriter, startPosition);

            writer.WriteUShort((ushort)(actWriter.Tell() + 2));
            writer.WriteWriter(actWriter);
            actWriter.Flush();
            actWriter.Close();
        }

        private void Fix(List<Action> evntList)
        {
            short oldNum = Num;
            bool ignoreOptimization = false;

            if (NebulaCore.Fusion == 1.5f && ObjectType > 1 && Num >= 48)
            {
                // MMF1.5 EXTBASE = 48
                // MMF2   EXTBASE = 80
                Num += 80 - 48;
                ignoreOptimization = true;
            }

            switch (ObjectType)
            {
                case -1:
                    switch (Num)
                    {
                        case -33:
                            Num = -8;
                            ignoreOptimization = true;
                            break;
                        case -24:
                            Num = -25;
                            ignoreOptimization = true;
                            break;
                        case 0: // Skip
                        case 44: // Skip
                            // DoAdd = false;
                            break;
                        case 27: // Set Global Integer
                        case 28: // Set Global
                        case 29: // Set Global Double
                        case 30: // Set Global
                            Num = 3; // Set Global
                            ignoreOptimization = true;
                            break;
                        case 31: // Add Global Integer
                        case 32: // Add Global
                        case 33: // Add Global Double
                        case 34: // Add Global
                            Num = 5; // Add Global
                            ignoreOptimization = true;
                            break;
                        case 35: // Subtract Global Integer
                        case 36: // Subtract Global
                        case 37: // Subtract Global Double
                        case 38: // Subtract Global
                            Num = 4; // Subtract Global
                            ignoreOptimization = true;
                            break;
                        case 43: // Execute Child Events
                            DoAdd = false;
                            ignoreOptimization = true;
                            break;
                    }
                    break;
                case >= 0:
                    switch (Num)
                    {
                        case 2: // Set X
                        case 3: // Set Y
                            if (Parameters.Length > 1)
                            {
                                Action act = Copy();
                                act.Parameters = new Parameter[1];
                                act.Parameters[0] = Parameters[0];
                                evntList.Add(act);

                                Num = (short)(Num == 2 ? 3 : 2);
                                Parameter param = Parameters[1];
                                Parameters = new Parameter[1];
                                Parameters[0] = param;
                            }
                            break;
                        case 13: // Set Movement
                            if (Parameters[0].Data is ParameterMovement && (ObjectInfo & 0x7FFF) == ObjectInfo)
                            {
                                ParameterMovement param = (ParameterMovement)Parameters[0].Data;
                                ObjectCommon oC = (ObjectCommon)NebulaCore.PackageData.FrameItems.Items[ObjectInfo].Properties;
                                if (oC.ObjectMovements.Movements.Length <= param.ID)
                                    break;
                                string name = oC.ObjectMovements.Movements[param.ID].Name;
                                param.Name = string.IsNullOrEmpty(name) ? "Movement #" + param.ID : name;
                            }
                            break;
                    }
                    break;
            }

            FrameEvents.OptimizedEvents |= ((Num != oldNum) || (DoAdd == false)) && !ignoreOptimization;
        }

        public override string ToString()
        {
            string Header = "";
            switch (ObjectType)
            {
                default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                case -7:
                    Header = $"Player {ObjectInfo + 1} : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return Header + $"Set Score to {Parameters[0]}";
                        case 1:
                            return Header + $"Set Number of Lives to {Parameters[0]}";
                        case 2:
                            return Header + "Ignore Control";
                        case 3:
                            return Header + "Restore Control";
                        case 4:
                            return Header + $"Add {Parameters[0]} to Score";
                        case 5:
                            return Header + $"Add {Parameters[0]} to Number of Lives";
                        case 6:
                            return Header + $"Subtract {Parameters[0]} from Score";
                        case 7:
                            return Header + $"Subtract {Parameters[0]} from Number of Lives";
                        case 8:
                            return Header + $"Set input device to {Parameters[0]}";
                        case 9:
                            return Header + $"Set key {Parameters[0]} to \"{GetKeyboardKey(((ParameterShort)Parameters[1].Data).Value)}\"";
                        case 10:
                            return Header + $"Set player name to {Parameters[0]}";
                    }
                case -6:
                    Header = "The mouse pointer and keyboard : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return Header + "Hide Windows mouse pointer";
                        case 1:
                            return Header + "Show Windows mouse pointer";
                        case 2:
                            return Header + $"Reset input between frames: {(int.TryParse(Parameters[0].ToString(), out int param) ? (param == 0 ? "Disabled" : "Enabled") : Parameters[0])}";
                    }
                case -5:
                    Header = "New Objects : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return Header + $"Create {Parameters[0]}";
                        case 1:
                            return Header + $"Create {Parameters[0]} at {Parameters[1]}";
                        case 2:
                        case 3:
                            return Header + $"Create {Parameters[0]} at X={Parameters[1]}, Y={Parameters[2]}, Layer={Parameters[3]}";
                    }
                case -4:
                    Header = "the timer : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return Header + $"Set timer to {Parameters[0]}";
                        case 1:
                            return Header + $"Fire event {Parameters[1]} after {Parameters[0]}";
                        case 2:
                            return Header + $"Fire event {Parameters[3]} {Parameters[1]} times every {Parameters[2]} after {Parameters[0]}";
                    }
                case -3:
                    Header = "storyboard controls : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return Header + "Next frame";
                        case 1:
                            return Header + "Previous frame";
                        case 2:
                            return Header + $"Jump to frame {((Parameters[0].Data is ParameterExpressions) ? Parameters[0] : GetFrameName(((ParameterShort)Parameters[0].Data).Value))}";
                        case 4:
                            return Header + "End the application";
                        case 5:
                            return Header + "Restart the application";
                        case 6:
                            return Header + "Restart current frame";
                        case 7:
                            return Header + $"Center display at {Parameters[0].ToString().ReplaceLast(" layer 1", "")}";
                        case 8:
                            return Header + $"Center display at X={Parameters[0]}";
                        case 9:
                            return Header + $"Center display at Y={Parameters[0]}";
                        case 12:
                            return Header + $"Clear screen in color {Parameters[0]}";
                        case 13:
                            return Header + $"Clear zone zone {Parameters[0]} in color {Parameters[1]}";
                        case 14:
                            return Header + "Full Screen Mode";
                        case 15:
                            return Header + "Windowed Mode";
                        case 16:
                            return Header + $"Set frame rate to {Parameters[0]}";
                        case 17:
                            return Header + $"Pause application and resume when key \"{GetKeyboardKey(((ParameterShort)Parameters[0].Data).Value)}\" is pressed";
                        case 18:
                            return Header + "Pause application and resume when any key is pressed";
                        case 19:
                            return Header + "Set V-Sync On";
                        case 20:
                            return Header + "Set V-Sync Off";
                        case 21:
                            return Header + $"Set Virtual Width to {Parameters[0]}";
                        case 22:
                            return Header + $"Set Virtual Height to {Parameters[0]}";
                        case 23:
                            return Header + $"Set Background Color to {Parameters[0]}";
                        case 24:
                            return Header + $"Delete Created Backdrops At {Parameters[1]},{Parameters[2]} in layer {Parameters[0]} (fine detection: {Parameters[3]})";
                        case 25:
                            return Header + $"Delete All Created Backdrops in layer {Parameters[0]}";
                        case 26:
                            return Header + $"Set Frame Width to {Parameters[0]}";
                        case 27:
                            return Header + $"Set Frame Height to {Parameters[0]}";
                        case 28:
                            return Header + $"Save frame position to {Parameters[0]}, version {Parameters[1]}";
                        case 29:
                            return Header + $"Load frame position {Parameters[0]}, version {Parameters[1]}";
                        case 30:
                            return Header + $"Load application position {Parameters[0]}, version {Parameters[1]}";
                        case 31:
                            return Header + $"Play demo file {Parameters[0]}";
                        case 32:
                            return Header + $"Set effect to {Parameters[0].ToString().ReplaceLast(".fx", "")}";
                        case 33:
                            return Header + $"Set effect parameter {Parameters[0]} to {Parameters[1]}";
                        case 34:
                            return Header + $"Set effect image parameter {Parameters[0]} to {Parameters[1]}";
                        case 35:
                            return Header + $"Set alpha-blending coefficient to {Parameters[0]}";
                        case 36:
                            return Header + $"Set RGB coefficient to {Parameters[0]}";
                        case 37:
                            return Header + $"Set anti-aliasing when resizing to {Parameters[0]}";
                    }
                case -2:
                    Header = "Sound : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return Header + $"Play sample {Parameters[0]}";
                        case 1:
                            return Header + "Stop any sample";
                        case 3:
                            return Header + "Stop any music";
                        case 4:
                            return Header + $"Play sample {Parameters[0]} {Parameters[1]} times";
                        case 6:
                            return Header + $"Stop sample {Parameters[0]}";
                        case 7:
                            return Header + $"Pause sample {Parameters[0]}";
                        case 8:
                            return Header + $"Resume sample {Parameters[0]}";
                        case 9:
                            return Header + "Pause music";
                        case 10:
                            return Header + "Resume music";
                        case 11:
                            return Header + $"Play sample {Parameters[0]} on channel #{Parameters[1]}";
                        case 12:
                            return Header + $"Play sample {Parameters[0]} on channel #{Parameters[1]}, {Parameters[2]} times";
                        case 13:
                            return Header + $"Pause channel #{Parameters[0]}";
                        case 14:
                            return Header + $"Resume channel #{Parameters[0]}";
                        case 15:
                            return Header + $"Stop channel #{Parameters[0]}";
                        case 16:
                            return Header + $"Set position of channel #{Parameters[0]} to {Parameters[1]}";
                        case 17:
                            return Header + $"Set volume of channel #{Parameters[0]} to {Parameters[1]}";
                        case 18:
                            return Header + $"Set pan of channel #{Parameters[0]} to {Parameters[1]}";
                        case 19:
                            return Header + $"Set position of sample {Parameters[0]} to {Parameters[1]}";
                        case 20:
                            return Header + $"Set main volume to {Parameters[0]}";
                        case 21:
                            return Header + $"Set volume of sample {Parameters[0]} to {Parameters[1]}";
                        case 22:
                            return Header + $"Set main pan to {Parameters[0]}";
                        case 23:
                            return Header + $"Set pan of sample {Parameters[0]} to {Parameters[1]}";
                        case 24:
                            return Header + "Pause all sounds";
                        case 25:
                            return Header + "Resume all sounds";
                        case 26:
                            return Header + $"Play music file {Parameters[0]}";
                        case 27:
                            return Header + $"Play and loop music file {Parameters[0]}, {Parameters[1]} times";
                        case 28:
                            return Header + $"Play sample file {Parameters[0]} on channel #{Parameters[1]}";
                        case 29:
                            return Header + $"Play sample file {Parameters[0]} on channel #{Parameters[1]}, {Parameters[2]} times";
                        case 30:
                            return Header + $"Lock channel {Parameters[0]}";
                        case 31:
                            return Header + $"Unlock channel {Parameters[0]}";
                        case 32:
                            return Header + $"Set frequency of channel #{Parameters[0]} to {Parameters[1]}";
                        case 33:
                            return Header + $"Set frequency of sample {Parameters[0]} to {Parameters[1]}";
                        case 34:
                            return Header + $"Preload sample file {Parameters[0]}";
                        case 35:
                            return Header + $"Discard sample file {Parameters[0]}";
                        case 36:
                            return Header + $"Play sample {Parameters[0]}, {Parameters[2]} times, on channel #{Parameters[1]}, volume {Parameters[3]}, pan {Parameters[4]}, frequency {Parameters[5]}";
                    }
                case -1:
                    Header = "Special : ";
                    switch (Num)
                    {
                        default:
                            return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 2:
                            ParameterFile file2 = (ParameterFile)Parameters[0].Data;
                            return Header + $"Execute external program {file2.FileName} {file2.Command}{(file2.FileFlags["WaitForEnd"] && file2.FileFlags["HideApplication"] ? " (wait,hide)" : file2.FileFlags["WaitForEnd"] ? " (wait)" : file2.FileFlags["HideApplication"] ? " (hide)" : "")}";
                        case 3:
                            return Header + $"Set {GetGlobalValueName(Parameters[0].Data)} to {Parameters[1]}";
                        case 4:
                            return Header + $"Subtract {Parameters[1]} from {GetGlobalValueName(Parameters[0].Data)}";
                        case 5:
                            return Header + $"Add {Parameters[1]} to {GetGlobalValueName(Parameters[0].Data)}";
                        case 6:
                            return Header + $"Activate Group \"{Parameters[0]}\"";
                        case 7:
                            return Header + $"Deactivate Group \"{Parameters[0]}\"";
                        case 8:
                            return Header + $"Enable menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}";
                        case 9:
                            return Header + $"Disable menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}";
                        case 10:
                            return Header + $"Check menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}";
                        case 11:
                            return Header + $"Uncheck menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}";
                        case 12:
                            return Header + "Show menu bar";
                        case 13:
                            return Header + "Hide menu bar";
                        case 14:
                            return Header + $"Start loop {Parameters[0]} {Parameters[1]} times";
                        case 15:
                            return Header + $"Stop loop {Parameters[0]}";
                        case 16:
                            return Header + $"Set loop {Parameters[0]} index to {Parameters[1]}";
                        case 17:
                            return Header + $"Randomize {Parameters[0]}";
                        case 18:
                            return Header + $"Send Menu Command {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}";
                        case 19:
                            return Header + $"Set {GetGlobalStringName(Parameters[0].Data)} to {Parameters[1]}";
                        case 20:
                            return Header + $"Send {Parameters[0]} to clipboard";
                        case 21:
                            return Header + "Clear clipboard";
                        case 22:
                            short flags22 = ((ParameterShort)Parameters[2].Data).Value;
                            return Header + $"Execute external program {Parameters[0]} {Parameters[1]}{(flags22 == 3 ? " (wait,hide)" : flags22 == 1 ? " (wait)" : flags22 == 2 ? " (hide)" : "")}";
                        case 23:
                            return Header + "Open debugger";
                        case 24:
                            return Header + "Pause debugger";
                        case 25:
                            return Header + $"Extract binary file {Parameters[0]}";
                        case 26:
                            return Header + $"Release binary file {Parameters[0]}";
                        case 39:
                            return Header + "Start profiling";
                        case 40:
                            return Header + "Stop profiling";
                        case 41:
                            return Header + "Clear output window";
                        case 42:
                            return Header + $"Send {Parameters[0]} to output window";
                        case 44:
                            return Header + "Break";
                    }
                case >= 0:
                    Header = GetObjectName() + " : ";
                    switch (Num)
                    {
                        default:
                            if (ObjectType < 32)
                                switch (Num)
                                {
                                    default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                    case 80:
                                        if (ObjectType == 3)
                                            return Header + "Erase";
                                        if (ObjectType == 4)
                                            return Header + "Ask question";
                                        if (ObjectType == 7)
                                            return Header + $"Set Counter to {Parameters[0]}";
                                        if (ObjectType == 8)
                                            return Header + $"Set X Position to {Parameters[0]}";
                                        if (ObjectType == 9)
                                            return Header + "Restart application";
                                        return Header + $"Paste into background{GetDirectionSettings(((ParameterShort)Parameters[0].Data).Value)}";
                                    case 81:
                                        if (ObjectType == 2)
                                            return Header + "Bring to front";
                                        if (ObjectType == 7)
                                            return Header + $"Add {Parameters[0]} to Counter";
                                        if (ObjectType == 8)
                                            return Header + $"Set Y Position to {Parameters[0]}";
                                        if (ObjectType == 9)
                                            return Header + "Restart current frame";
                                        return Header + $"Display paragraph {((ParameterShort)Parameters[1].Data).Value + 1} at {Parameters[0].ToString().ReplaceLast(" layer 1", "")}";
                                    case 82:
                                        if (ObjectType == 7)
                                            return Header + $"Subtract {Parameters[0]} from Counter";
                                        if (ObjectType == 8)
                                            return Header + $"Set Zoom Factor to {Parameters[0]}";
                                        if (ObjectType == 9)
                                            return Header + "Next Frame";
                                        return Header + $"Flash paragraph {((ParameterShort)Parameters[1].Data).Value + 1} at {Parameters[0].ToString().ReplaceLast(" layer 1", "")} for {Parameters[2]}";
                                    case 83:
                                        if (ObjectType == 3)
                                            return Header + $"Set color of {Parameters[0]}";
                                        if (ObjectType == 7)
                                            return Header + $"Set minimum value to {Parameters[0]}";
                                        if (ObjectType == 8)
                                            return Header + "Clear Selected";
                                        if (ObjectType == 9)
                                            return Header + "Previous Frame";
                                        return Header + $"Add backdrop{GetDirectionSettings(((ParameterShort)Parameters[0].Data).Value)}";
                                    case 84:
                                        if (ObjectType == 3)
                                        {
                                            if (Parameters.Length > 1 && Parameters[1].Data is ParameterShort)
                                                return Header + $"Display paragraph {((ParameterShort)Parameters[1].Data).Value + 1}";
                                            else
                                                return Header + $"Display paragraph {Parameters[0].Data}";
                                        }
                                        if (ObjectType == 7)
                                            return Header + $"Set maximum value to {Parameters[0]}";
                                        if (ObjectType == 8)
                                            return Header + $"Search and select {Parameters[0]}";
                                        if (ObjectType == 9)
                                            return Header + "End application";
                                        return Header + $"Replace color {Parameters[0]} by {Parameters[1]}";
                                    case 85:
                                        if (ObjectType == 3)
                                            return Header + "Display previous paragraph";
                                        if (ObjectType == 7)
                                            return Header + $"Set color to {Parameters[0]}";
                                        if (ObjectType == 8)
                                            return Header + "Search and select next";
                                        if (ObjectType == 9)
                                            return Header + $"Set application {Parameters[0]}";
                                        return Header + $"Set scale to {Parameters[0]} (Quality = {Parameters[1]})";
                                    case 86:
                                        if (ObjectType == 3)
                                            return Header + "Display next paragraph";
                                        if (ObjectType == 7)
                                            return Header + $"Set color #2 to {Parameters[0]}";
                                        if (ObjectType == 8)
                                            return Header + $"Search All and select{Parameters[0]}";
                                        if (ObjectType == 9)
                                            return Header + $"Jump to Frame {Parameters[0]}";
                                        return Header + $"Set X scale to {Parameters[0]} (Quality = {Parameters[1]})";
                                    case 87:
                                        if (ObjectType == 3)
                                            return Header + "Display alterable string";
                                        if (ObjectType == 8)
                                            return Header + $"Select word num {Parameters[0]}";
                                        if (ObjectType == 9)
                                            return Header + $"Set global value {Parameters[0]} to {Parameters[1]}";
                                        return Header + $"Set Y scale to {Parameters[0]} (Quality = {Parameters[1]})";
                                    case 88:
                                        if (ObjectType == 3)
                                            return Header + $"Set alterable string to {Parameters[0]}";
                                        if (ObjectType == 8)
                                            return Header + $"Select Line num {Parameters[0]}";
                                        return Header + $"Set angle to {Parameters[0]} (Quality = {Parameters[1]})";
                                    case 89:
                                        if (ObjectType == 8)
                                            return Header + $"Select Paragraph num {Parameters[0]}";
                                        if (Parameters[1].Data is ParameterShort)
                                            return Header + $"Load {Parameters[0]} into Animation {GetObjectAnimation(((ParameterShort)Parameters[1].Data).Value)}, Direction {GetDirection(Parameters[2].Data)}, Frame #{Parameters[3]}, HotSpot({Parameters[4]},{Parameters[5]}, Action Point({Parameters[6]},{Parameters[7]}), Transparent Color {Parameters[8]}";
                                        return Header + $"Load {Parameters[0]} into Animation {Parameters[1]}, Direction {GetDirection(Parameters[2].Data)}, Frame #{Parameters[3]}, HotSpot({Parameters[4]},{Parameters[5]}, Action Point({Parameters[6]},{Parameters[7]}), Transparent Color {Parameters[8]}";
                                    case 90:
                                        if (ObjectType == 9)
                                            return Header + $"Set global string {Parameters[0]} to {Parameters[1]}";
                                        return Header + $"Load {Parameters[0]} as image, HotSpot({Parameters[1]},{Parameters[2]}, Action Point({Parameters[3]},{Parameters[4]}), Transparent Color {Parameters[5]}";
                                    case 91:
                                        if (ObjectType == 8)
                                            return Header + "Select All";
                                        if (ObjectType == 9)
                                            return Header + "Pause application";
                                        return Header + $"Load animations from file {Parameters[0]}";
                                    case 92:
                                        if (ObjectType == 9)
                                            return Header + "Resume application";
                                        return Header + $"Select from Word num {Parameters[0]} to {Parameters[1]}";
                                    case 93:
                                        return Header + $"Set width to {Parameters[0]}";
                                    case 94:
                                        if (ObjectType == 9)
                                            return Header + $"Set height to {Parameters[0]}";
                                        return Header + $"Set Focus to Word num {Parameters[0]}";
                                    case 95:
                                        return Header + "Remove highlight";
                                    case 96:
                                        return Header + $"Set Highlighted text color {Parameters[0]}";
                                    case 97:
                                        return Header + $"Set Highlighted text bold";
                                    case 98:
                                        return Header + $"Set Highlighted text italic";
                                    case 99:
                                        return Header + $"Set Highlighted text underlined";
                                    case 100:
                                        return Header + $"Set Highlighted text outline color {Parameters[0]}, {Parameters[1]} pixels";
                                    case 101:
                                        return Header + $"Set Highlighted background color {Parameters[0]}";
                                    case 103:
                                        return Header + "Set Highlighted background as Marker";
                                    case 104:
                                        return Header + $"Set Highlighted background as Hatched with color {Parameters[0]}";
                                    case 105:
                                        return Header + "Set Highlighted background as Inverted";
                                    case 106:
                                        return Header + "Display the text";
                                    case 107:
                                        return Header + "Set Focus to previous word";
                                    case 108:
                                        return Header + "Set Focus to next word";
                                    case 109:
                                        return Header + "Remove the Focus";
                                    case 112:
                                        return Header + $"Insert {Parameters[0]}";
                                    case 113:
                                        return Header + $"Load text {Parameters[0]}";
                                    case 114:
                                        return Header + $"Insert text {Parameters[0]}";
                                }
                            switch (ObjectType)
                            {
                                default:
                                    string output = Header + $"Event ID {Num}, {Parameters.Length} Parameters";
                                    for (int i = 0; i < Parameters.Length; i++)
                                        output += (i == 0 ? " { " : "") + Parameters[i] + (i + 1 < Parameters.Length ? ", " : " }");
                                    return output;
                            }
                        case 0:
                            return Header + $"Set Flag {Parameters[1]} to {Parameters[2]}";
                        case 1:
                            return Header + $"Set position at {Parameters[0].ToString().ReplaceLast(" layer 1", "")}";
                        case 2:
                            return Header + $"Set X position to {Parameters[0]}";
                        case 3:
                            return Header + $"Set Y position to {Parameters[0]}";
                        case 4:
                            return Header + "Stop";
                        case 5:
                            return Header + "Start";
                        case 6:
                            return Header + $"Set speed to {Parameters[0]}";
                        case 7:
                            return Header + $"Set maximum speed to {Parameters[0]}";
                        case 8:
                            return Header + "Wrap";
                        case 9:
                            return Header + "Bounce";
                        case 10:
                            return Header + "Reverse";
                        case 11:
                            return Header + "Next movement";
                        case 12:
                            return Header + "Previous movement";
                        case 13:
                            return Header + $"Select movement {Parameters[0]}";
                        case 14:
                            return Header + $"Look at {Parameters[0].ToString().ReplaceLast(" layer 1", "")}";
                        case 15:
                            return Header + "Stop animation";
                        case 16:
                            return Header + "Start animation";
                        case 17:
                            if (Parameters[0].Data is ParameterShort animS)
                                return Header + $"Change animation sequence to {GetObjectAnimation(animS.Value)}";
                            else
                                return Header + $"Change animation sequence to {Parameters[0].Data}";
                        case 18:
                            return Header + $"Change animation direction to {GetDirection(Parameters[0].Data)}";
                        case 19:
                            return Header + $"Change speed of animation to {Parameters[0]}";
                        case 20:
                            return Header + "Restore animation sequence";
                        case 21:
                            return Header + "Restore animation direction";
                        case 22:
                            return Header + "Restore speed of animation";
                        case 23:
                            return Header + $"Set direction to {GetDirection(Parameters[0].Data)}";
                        case 24:
                            return Header + "Destroy";
                        case 25:
                            return Header + "Swap position with another object";
                        case 26:
                            return Header + "Make invisible";
                        case 27:
                            return Header + "Reappear";
                        case 28:
                            return Header + $"Flash during {Parameters[0]}";
                        case 29:
                            return Header + $"Launch {Parameters[0]}";
                        case 30:
                            return Header + $"Launch {Parameters[0]} toward {Parameters[1].ToString().ReplaceLast(" layer 1", "")}";
                        case 31:
                            return Header + $"Set {GetObjectAlterableValueName(Parameters[0].Data)} to {Parameters[1]}";
                        case 32:
                            return Header + $"Add {Parameters[1]} to {GetObjectAlterableValueName(Parameters[0].Data)}";
                        case 33:
                            return Header + $"Subtract {Parameters[1]} from {GetObjectAlterableValueName(Parameters[0].Data)}";
                        case 34:
                            return Header + $"Spread value {Parameters[1]} in {GetObjectAlterableValueName(Parameters[0].Data)}";
                        case 35:
                            return Header + $"Set Flag {Parameters[0]} on";
                        case 36:
                            return Header + $"Set Flag {Parameters[0]} off";
                        case 37:
                            return Header + $"Toggle Flag {Parameters[0]}";
                        case 38:
                            return Header + $"Set ink effect to {GetInkEffect((ParameterDoubleShort)Parameters[0].Data)}";
                        case 39:
                            return Header + $"Set semi-transparency to {Parameters[0]}";
                        case 40:
                            return Header + $"Force animation frame to {Parameters[0]}";
                        case 41:
                            return Header + "Restore animation frame";
                        case 42:
                            return Header + $"Set acceleration to {Parameters[0]}";
                        case 43:
                            return Header + $"Set deceleration to {Parameters[0]}";
                        case 44:
                            return Header + $"Set rotating speed to {Parameters[0]}";
                        case 45:
                            return Header + $"Set authorized directions to {GetDirection(Parameters[0].Data)}";
                        case 46:
                            return Header + $"Branch node {Parameters[0]}";
                        case 47:
                            return Header + $"Set gravity to {Parameters[0]}";
                        case 48:
                            return Header + $"Goto node {Parameters[0]}";
                        case 49:
                            return Header + $"Set {GetObjectAlterableStringName(Parameters[0].Data)} to {Parameters[1]}";
                        case 50:
                            return Header + $"Set font name to {Parameters[0]}";
                        case 51:
                            return Header + $"Set font size to {Parameters[0]}, border {Parameters[1]}";
                        case 52:
                            return Header + $"Set bold {Parameters[0]}";
                        case 53:
                            return Header + $"Set italic {Parameters[0]}";
                        case 54:
                            return Header + $"Set underline {Parameters[0]}";
                        case 55:
                            return Header + $"Set strikeout {Parameters[0]}";
                        case 56:
                            return Header + $"Set font color to {Parameters[0]}";
                        case 57:
                            return Header + "Bring to front";
                        case 58:
                            return Header + "Bring to back";
                        case 59:
                            return Header + $"Move behind object {Parameters[0]}";
                        case 60:
                            return Header + $"Move in front of object {Parameters[0]}";
                        case 61:
                            return Header + $"Move to layer {Parameters[0]}";
                        case 62:
                            return Header + "Add to debugger";
                        case 63:
                            return Header + $"Set effect to {Parameters[0].ToString().ReplaceLast(".fx", "")}";
                        case 64:
                            return Header + $"Set effect parameter {Parameters[0]} to {Parameters[1]}";
                        case 65:
                            return Header + $"Set alpha-blending coefficient to {Parameters[0]}";
                        case 66:
                            return Header + $"Set RGB coefficient to {Parameters[0]}";
                        case 67:
                            return Header + $"Set effect image parameter {Parameters[0]} to {Parameters[1]}";
                        case 68:
                            return Header + $"Set friction to {Parameters[0]}";
                        case 69:
                            return Header + $"Set elasticity to {Parameters[0]}";
                        case 70:
                            return Header + $"Apply impulse, force {Parameters[0]}, angle {Parameters[1]} degrees";
                        case 71:
                            return Header + $"Apply angular impulse, torque {Parameters[0]}";
                        case 72:
                            return Header + $"Apply force {Parameters[0]}, angle {Parameters[1]} degrees";
                        case 73:
                            return Header + $"Apply torque {Parameters[0]}";
                        case 74:
                            return Header + $"Set linear velocity to {Parameters[0]}, angle {Parameters[1]} degrees";
                        case 75:
                            return Header + $"Set angular velocity to {Parameters[0]}";
                        case 76:
                            return $"Start loop for each one of {GetObjectName()}, loop name {Parameters[0]}";
                        case 77:
                            return $"Start loop for each one of {GetObjectName()} and {Parameters[0]}, loop name {Parameters[1]}";
                        case 78:
                            return Header + "Stop force";
                        case 79:
                            return Header + "Stop torque";
                    }
                }
        }

        public Action Copy()
        {
            Action act = new Action();
            act.ObjectType = ObjectType;
            act.Num = Num;
            act.ObjectInfo = ObjectInfo;
            act.ObjectInfoList = ObjectInfoList;
            act.EventFlags.Value = EventFlags.Value;
            act.OtherFlags.Value = OtherFlags.Value;
            act.DefType = DefType;
            act.Parent = Parent;
            act.DoAdd = DoAdd;
            return act;
        }
    }
}
