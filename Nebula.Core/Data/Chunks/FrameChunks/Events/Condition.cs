using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public class Condition : Chunk
    {
        public BitDict EventFlags = new BitDict( // Flags
            "Repeat",         // Repeat
            "Done",           // Done
            "Default",        // Default
            "DoneBeforeFade", // Done Before Fade In
            "NotDoneInStart", // Not Done In Start
            "Always",         // Always
            "Bad",            // Bad
            "BadObject"       // Bad Object
        );

        public BitDict OtherFlags = new BitDict( // Other Flags
            "Negated", "", "", "", "", // Not
            "NoInterdependence"        // No Object Interdependence
        );

        public short ObjectType;
        public short Num;
        public short ObjectInfo;
        public short ObjectInfoList;
        public Parameter[] Parameters = new Parameter[0];
        public byte DefType;
        public short Identifier;

        public Event? Parent = null;
        public bool DoAdd = true;

        public Condition()
        {
            ChunkName = "Condition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadUShort());

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();
            Identifier = reader.ReadShort();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
            Fix((List<Condition>)extraInfo[0]);
            Parent = (Event)extraInfo[1];
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadUShort());

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            EventFlags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();
            Identifier = reader.ReadShort();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
            Parent = (Event)extraInfo[1];
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter condWriter = new ByteWriter(new MemoryStream());
            condWriter.WriteShort(ObjectType);
            condWriter.WriteShort(Num);
            short oI = ObjectInfo;
            if (oI >> 8 == -128)
            {
                byte qual = (byte)(oI & 0xFF);
                oI = (short)((qual | ((128 - ObjectType) << 8)) & 0xFFFF);
            }
            condWriter.WriteShort(oI);
            condWriter.WriteShort(ObjectInfoList);
            condWriter.WriteByte((byte)EventFlags.Value);
            condWriter.WriteByte((byte)OtherFlags.Value);
            condWriter.WriteByte((byte)Parameters.Length);
            condWriter.WriteByte(DefType);
            condWriter.WriteShort(Identifier);

            foreach (Parameter parameter in Parameters)
                parameter.WriteMFA(condWriter);

            writer.WriteUShort((ushort)(condWriter.Tell() + 2));
            writer.WriteWriter(condWriter);
            condWriter.Flush();
            condWriter.Close();
        }

        private void Fix(List<Condition> evntList)
        {
            switch (ObjectType)
            {
                case -1:
                    switch (Num)
                    {
                        case 0:
                            DoAdd = false;
                            break;
                        case -25:
                            Num = -24;
                            break;
                        case -28:
                        case -31:
                            Num = -8;
                            break;
                        case -43:
                            DoAdd = false;
                            break;
                    }
                    break;
                case >= 0:
                    switch (Num)
                    {
                        case -25:
                            Num = -27;
                            DoAdd = false; // TODO
                            break;
                        case -42:
                            Num = -27;
                            break;
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return (OtherFlags["Negated"] ? "NOT " : "") + GetString();
        }

        public string GetString()
        {
            switch (ObjectType)
            {
                default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType {ObjectType}");
                case -7:
                    switch (Num)
                    {
                        default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType -7, Num {Num}");
                        case -6:
                            return $"Repeat while Player {ObjectInfo + 1} {GetJoystickString(((ParameterShort)Parameters[0].Data).Value)}";
                        case -5:
                            return $"Number of lives of Player {ObjectInfo + 1} reaches 0";
                        case -4:
                            return $"Player {ObjectInfo + 1} {GetJoystickString(((ParameterShort)Parameters[0].Data).Value)}";
                        case -3:
                            return $"Number of lives of Player {ObjectInfo + 1} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -2:
                            return $"Score of Player {ObjectInfo + 1} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                    }
                case -6:
                    switch (Num)
                    {
                        default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType -6, Num {Num}");
                        case -12:
                            return "On mouse wheel down";
                        case -11:
                            return "On mouse wheel up";
                        case -10:
                            return "Mouse cursor is displayed";
                        case -9:
                            return "Upon pressing any key";
                        case -8:
                            string return8 = "Repeat while ";
                            return8 += ((ParameterShort)Parameters[0].Data).Value switch
                            {
                                1 => "left",
                                2 => "right",
                                4 => "middle",
                                _ => "left"
                            };
                            return return8 + " mouse-key is pressed";
                        case -7:
                            ParameterClick click7 = (ParameterClick)Parameters[0].Data;
                            string return7 = $"User {(click7.IsDouble != 0 ? "double-" : "")}clicks with ";
                            return7 += click7.Button switch
                            {
                                0 => "left",
                                1 => "middle",
                                2 => "right",
                                _ => "left"
                            };
                            return return7 + $" button on {Parameters[1]}";
                        case -6:
                            ParameterClick click6 = (ParameterClick)Parameters[0].Data;
                            string return6 = $"User {(click6.IsDouble != 0 ? "double-" : "")}clicks with ";
                            return6 += click6.Button switch
                            {
                                0 => "left",
                                1 => "middle",
                                2 => "right",
                                _ => "left"
                            };
                            return return6 + $" button within zone {Parameters[1]}";
                        case -5:
                            ParameterClick click5 = (ParameterClick)Parameters[0].Data;
                            string return5 = $"User {(click5.IsDouble != 0 ? "double-" : "")}clicks with ";
                            return5 += click5.Button switch
                            {
                                0 => "left",
                                1 => "middle",
                                2 => "right",
                                _ => "left"
                            };
                            return return5 + " button";
                        case -4:
                            return $"Mouse pointer is over {Parameters[0]}";
                        case -3:
                            return $"Mouse pointer lays within zone {Parameters[0]}";
                        case -2:
                            return $"Repeat while \"{GetKeyboardKey(((ParameterShort)Parameters[0].Data).Value)}\" is pressed";
                        case -1:
                            return $"Upon pressing \"{GetKeyboardKey(((ParameterShort)Parameters[0].Data).Value)}\"";
                    }
                case -5:
                    switch (Num)
                    {
                        default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType -5, Num {Num}");
                        case -23:
                            return $"Pick all objects in line ({Parameters[0]},{Parameters[1]}) to ({Parameters[2]},{Parameters[3]})";
                        case -22:
                            return $"Pick objects with Flag {Parameters[0]} off";
                        case -21:
                            return $"Pick objects with Flag {Parameters[0]} on";
                        case -20:
                            return $"Pick objects which {GetAlterableValueName(((ParameterInt)Parameters[0].Data).Value)} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -19:
                            return $"Pick objects with fixed value {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -18:
                            return $"Pick all objects in zone {Parameters[0]}";
                        case -17:
                            return "Pick an object at random";
                        case -16:
                            return $"Pick a random object from zone {Parameters[0]}";
                        case -15:
                            return $"Total number of objects {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -14:
                            return $"Number of objects in zone {Parameters[0]} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -13:
                            return $"No more objects in zone {Parameters[0]}";
                    }
                case -4:
                    switch (Num)
                    {
                        default: return base.ToString();//throw new NotImplementedException($"Could not find ObjectType -4, Num {Num}");
                        case -8:
                            return $"Every {Parameters[0]}";
                        case -7:
                            return $"Timer equals {Parameters[0]}";
                        case -6:
                            return $"On timer event {Parameters[0]}";
                        case -5:
                            return $"User has left the computer for {Parameters[0]}";
                        case -2:
                            return $"Timer is less than {Parameters[0]}";
                        case -1:
                            return $"Timer is greater than {Parameters[0]}";
                    }
                case -3:
                    switch (Num)
                    {
                        default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType -3, Num {Num}");
                        case -10:
                            return "Frame position has just been saved";
                        case -9:
                            return "Frame position has just been loaded";
                        case -8:
                            return "End of pause";
                        case -7:
                            return "V-Sync is enabled";
                        case -6:
                            return $"{Parameters[0]},{Parameters[1]} is a ladder";
                        case -5:
                            return $"{Parameters[0]},{Parameters[1]} is an obstacle";
                        case -4:
                            return "End of Application";
                        case -2:
                            return "End of Frame";
                        case -1:
                            return "Start of Frame";
                    }
                case -2:
                    switch (Num)
                    {
                        default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType -2, Num {Num}");
                        case -9:
                            return $"Is channel {Parameters[0]} paused?";
                        case -8:
                            return $"Is channel {Parameters[0]} not playing?";
                        case -7:
                            return $"Is music paused?";
                        case -6:
                            return $"Is sample {Parameters[0]} paused?";
                        case -4:
                            return "No music is playing";
                        case -3:
                            return "No sample is playing";
                        case -1:
                            return $"{Parameters[0]} is not playing";
                    }
                case -1:
                    switch (Num)
                    {
                        default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType -1, Num {Num}");
                        case -42:
                            return "Never";
                        case -41:
                            return "Is profiling in progress";
                        case -40:
                            if (Parameters[0].Data is ParameterInt)
                            {
                                switch (((ParameterInt)Parameters[0].Data).Value)
                                {
                                    case 0:
                                        return "Is running as Windows application";
                                    case 1:
                                        return "Is running as Mac application";
                                    case 2:
                                        return "Is running as SWF / Flash Player application";
                                    case 3:
                                        return "Is running as Android application";
                                    case 4:
                                        return "Is running as iOS application";
                                    case 5:
                                        return "Is running as Html5 application";
                                    case 6:
                                        return "Is running as XNA application";
                                    case 7:
                                        return "Is running as UWP application";
                                }
                            }
                            return $"Is running as {Parameters[0]}";
                        case -26:
                            return $"{Parameters[0]} chances out of {Parameters[1]} at random";
                        case -25:
                            return "OR (logical)";
                        case -24:
                            return "OR";
                        case -22:
                            return "Text is available in clipboard";
                        case -21:
                            return "Close window has been selected";
                        case -20:
                            return "Menu bar is visible";
                        case -19:
                            return $"Menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)} is enabled";
                        case -18:
                            return $"Menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)} is enabled";
                        case -17:
                            return $"Menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)} is checked";
                        case -16:
                            return $"On loop {Parameters[0]}";
                        case -15:
                            return "Files have been dropped";
                        case -14:
                            return $"Menu option \"{GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}\" selected";
                        case -8:
                            return $"{GetGlobalValueName(Parameters[0].Data)} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -7:
                            return "Only one action when event loops";
                        case -6:
                            return "Run this event once";
                        case -5:
                            return $"Repeat {Parameters[0]} times";
                        case -4:
                            return $"Restrict actions for {Parameters[0]}";
                        case -3:
                            return $"{Parameters[0]} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -2:
                            return "Never";
                        case -1:
                            return "Always";
                    }
                case >= 0:
                    switch (Num)
                    {
                        default:
                            if (ObjectType < 32)
                                switch (Num)
                                {
                                    default: return base.ToString();// throw new NotImplementedException($"Could not find ObjectType {ObjectType}, Num {Num}");
                                    case -84:
                                        if (ObjectType == 9)
                                            return $"Application {GetObjectName()} is paused";
                                        return $"Y scale of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                                    case -83:
                                        if (ObjectType == 4)
                                            return $"{GetObjectName()} : answer number {Parameters[0]} chosen";
                                        if (ObjectType == 9)
                                            return $"Application {GetObjectName()} is visible";
                                        return $"X scale of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                                    case -82:
                                        if (ObjectType == 4)
                                            return $"{GetObjectName()} : bad answer";
                                        if (ObjectType == 9)
                                            return $"Application {GetObjectName()} is finished";
                                        return $"Angle of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                                    case -81:
                                        if (ObjectType == 4)
                                            return $"{GetObjectName()} : correct answer";
                                        if (ObjectType == 9)
                                            return $"{GetObjectName()}: Frame has changed";
                                        return $"{GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";

                                }
                            switch (ObjectType)
                            {
                                default:
                                    string output = $"{GetObjectName()}: Event ID {Num}, {Parameters.Length} Parameters";
                                    for (int i = 0; i < Parameters.Length; i++)
                                        output += (i == 0 ? " { " : "") + Parameters[i] + (i + 1 < Parameters.Length ? ", " : " }");
                                    return output;
                            }
                        case -51:
                            return $"Would {GetObjectName()} at X={Parameters[0]}, Y={Parameters[1]} overlap a backdrop?";
                        case -50:
                            return $"Would {GetObjectName()} at X={Parameters[1]}, Y={Parameters[2]} overlap {Parameters[0]}?";
                        case -49:
                            return $"Instance value of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -48:
                            return $"Pick \"{GetObjectName()}\" objects with a maximum value of {Parameters[0]}";
                        case -47:
                            return $"Pick \"{GetObjectName()}\" objects with a minimum value of {Parameters[0]}";
                        case -46:
                            return $"Layer of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -45:
                            return $"{GetObjectName()}: {Parameters[0]} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -44:
                            return $"Pick closest \"{GetObjectName()}\" objects from \"{Parameters[0]}\"";
                        case -41:
                            return $"On each of {GetObjectName()}, loop name {Parameters[0]}";
                        case -40:
                            return $"{GetObjectName()}: font is strikeout";
                        case -39:
                            return $"{GetObjectName()}: font is underlined";
                        case -38:
                            return $"{GetObjectName()}: font is italic";
                        case -37:
                            return $"{GetObjectName()}: font is bold";
                        case -36:
                            return $"{GetObjectAlterableStringName(((ParameterShort)Parameters[0].Data).Value)} of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -35:
                            return $"Path movement of {GetObjectName()} has reached node {Parameters[1]}";
                        case -34:
                            return $"Pick one of {GetObjectName()}";
                        case -33:
                            return $"Last {GetObjectName()} has been destroyed";
                        case -32:
                            return $"Number of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -31:
                            return $"No more {GetObjectName()} in zone {Parameters[0]}";
                        case -30:
                            return $"Number of {GetObjectName()} in zone {Parameters[0]} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -29:
                            return $"{GetObjectName()} is visible";
                        case -28:
                            return $"{GetObjectName()} is invisible";
                        case -27:
                            return $"{GetObjectAlterableValueName(((ParameterShort)Parameters[0].Data).Value)} of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                        case -26:
                            return $"Fixed value of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -25:
                            return $"{GetObjectName()}: Flag {Parameters[0]} is on";
                        case -24:
                            return $"{GetObjectName()}: Flag {Parameters[0]} is off";
                        case -23:
                            return $"{GetObjectName()} is overlapping a backdrop";
                        case -22:
                            return $"{GetObjectName()} is getting closer than {Parameters[0]} pixels from window's edge";
                        case -21:
                            return $"{GetObjectName()} has reached the end in its path";
                        case -20:
                            return $"{GetObjectName()} has reached a node in the path";
                        case -19:
                            return $"Acceleration of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -18:
                            return $"Deceleration of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -17:
                            return $"X position of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -16:
                            return $"Y position of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -15:
                            return $"Speed of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                        case -14:
                            return $"Collision between {GetObjectName()} and {Parameters[0]}";
                        case -13:
                            return $"{GetObjectName()} collides with the background";
                        case -12:
                            return $"{GetObjectName()} leaves the play area{GetPlayAreaString(((ParameterShort)Parameters[0].Data).Value)}";
                        case -11:
                            return $"{GetObjectName()} enters the play area{GetPlayAreaString(((ParameterShort)Parameters[0].Data).Value)}";
                        case -10:
                            return $"{GetObjectName()} is out of the play area";
                        case -9:
                            return $"{GetObjectName()} is in the play area";
                        case -8:
                            return $"{GetObjectName()} is facing a direction {GetDirection(Parameters[0].Data)}";
                        case -7:
                            return $"{GetObjectName()} is stopped";
                        case -6:
                            return $"{GetObjectName()} is bouncing";
                        case -4:
                            return $"{GetObjectName()} is overlapping {Parameters[0]}";
                        case -3:
                            return $"{GetObjectName()} animation {GetObjectAnimation(((ParameterShort)Parameters[0].Data).Value)} is playing";
                        case -2:
                            return $"{GetObjectName()} animation {GetObjectAnimation(((ParameterShort)Parameters[0].Data).Value)} is over";
                        case -1:
                            return $"Current frame of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                    }
            }
        }

        public ObjectInfo GetObject()
        {
            if (Parent!.Parent.EventObjects.Count > 0)
                return NebulaCore.PackageData.FrameItems.Items[(int)Parent.Parent.EventObjects[ObjectInfo].ItemHandle];
            else return NebulaCore.PackageData.FrameItems.Items[ObjectInfo];
        }

        public string GetGlobalValueName(ParameterChunk param)
        {
            if (param is ParameterInt p)
            {
                int id = p.Value;
                if (NebulaCore.PackageData.GlobalValueNames.Names.Length <= id || string.IsNullOrEmpty(NebulaCore.PackageData.GlobalValueNames.Names[id]))
                {
                    string output = "Global Value ";
                    if (id > 26)
                        output += (char)('A' + Math.Floor(id / 27d));
                    output += (char)('A' + id % 27);
                    return output;
                }
                else return NebulaCore.PackageData.GlobalValueNames.Names[id];
            }
            else return $"Global Value({param})";
        }

        public string GetGlobalStringName(ParameterChunk param)
        {
            if (param is ParameterShort p)
            {
                int id = p.Value;
                if (NebulaCore.PackageData.GlobalStringNames.Names.Length <= id || string.IsNullOrEmpty(NebulaCore.PackageData.GlobalStringNames.Names[id]))
                {
                    string output = "Global String ";
                    if (id > 26)
                        output += (char)('A' + Math.Floor(id / 27d));
                    output += (char)('A' + id % 27);
                    return output;
                }
                else return NebulaCore.PackageData.GlobalStringNames.Names[id];
            }
            else return $"Global String({param})";
        }

        public string GetAlterableValueName(int id)
        {
            string output = "Alterable Value ";
            if (id > 26)
                output += (char)('A' + Math.Floor(id / 27d));
            output += (char)('A' + id % 27);
            return output;
        }

        public string GetAlterableStringName(int id)
        {
            string output = "Alterable String ";
            if (id > 26)
                output += (char)('A' + Math.Floor(id / 27d));
            output += (char)('A' + id % 27);
            return output;
        }

        public string GetObjectAlterableValueName(int id)
        {
            ObjectCommon oC = (ObjectCommon)GetObject().Properties;
            if (oC.ObjectAlterableValues.Names.Length <= id || string.IsNullOrEmpty(oC.ObjectAlterableValues.Names[id]))
            {
                string output = "Alterable Value ";
                if (id > 26)
                    output += (char)('A' + Math.Floor(id / 27d));
                output += (char)('A' + id % 27);
                return output;
            }
            else return oC.ObjectAlterableValues.Names[id];
        }

        public string GetObjectAlterableStringName(int id)
        {
            ObjectCommon oC = (ObjectCommon)GetObject().Properties;
            if (oC.ObjectAlterableStrings.Names.Length <= id || string.IsNullOrEmpty(oC.ObjectAlterableStrings.Names[id]))
            {
                string output = "Alterable String ";
                if (id > 26)
                    output += (char)('A' + Math.Floor(id / 27d));
                output += (char)('A' + id % 27);
                return output;
            }
            else return oC.ObjectAlterableStrings.Names[id];
        }

        public string GetComparison(int id) => id switch
        {
            0 => "=",
            1 => "<>",
            2 => "<=",
            3 => "<",
            4 => ">=",
            5 => ">",
            _ => "="
        };

        public string GetMenuItemName(int id)
        {
            MenuItem? item = null;
            foreach (MenuItem menu in NebulaCore.PackageData.MenuBar.Items)
            {
                if (menu.ID == id)
                {
                    item = menu;
                    break;
                }
                if (menu.Flags["Parent"])
                {
                    MenuItem menuItem = GetMenuItem(id, menu.Items.ToArray());
                    if (menuItem != null)
                    {
                        item = menuItem;
                        break;
                    }
                }
            }
            return item != null ? item.Name.Split('\t')[0] : "";
        }

        public MenuItem GetMenuItem(int id, MenuItem[] parent)
        {
            foreach (MenuItem menu in parent)
            {
                if (menu.ID == id)
                    return menu;
                if (menu.Flags["Parent"])
                    return GetMenuItem(id, menu.Items.ToArray());
            }
            return null;
        }

        public string GetKeyboardKey(short id) => id switch
        {
            0x08 => "Backspace",
            0x09 => "Tab",
            0x0D => "Enter",
            0x10 => "Shift",
            0x11 => "Control",
            0x13 => "", // Pause
            0x14 => "", // Caps Lock
            0x1B => "Escape",
            0x20 => "Space bar",
            0x21 => "Page Up",
            0x22 => "Page Down",
            0x23 => "End",
            0x24 => "Home",
            0x25 => "Left Arrow",
            0x26 => "Up Arrow",
            0x27 => "Right Arrow",
            0x28 => "Down Arrow",
            0x2D => "Ins",
            0x2E => "Del",
            0x30 => "0",
            0x31 => "1",
            0x32 => "2",
            0x33 => "3",
            0x34 => "4",
            0x35 => "5",
            0x36 => "6",
            0x37 => "7",
            0x38 => "8",
            0x39 => "9",
            0x41 => "A",
            0x42 => "B",
            0x43 => "C",
            0x44 => "D",
            0x45 => "E",
            0x46 => "F",
            0x47 => "G",
            0x48 => "H",
            0x49 => "I",
            0x4A => "J",
            0x4B => "K",
            0x4C => "L",
            0x4D => "M",
            0x4E => "N",
            0x4F => "O",
            0x50 => "P",
            0x51 => "Q",
            0x52 => "R",
            0x53 => "S",
            0x54 => "T",
            0x55 => "U",
            0x56 => "V",
            0x57 => "W",
            0x58 => "X",
            0x59 => "Y",
            0x5A => "Z",
            0x5B => "", // Windows Key
            0x5C => "", // Menu
            0x60 => "0 (Num. keypad)",
            0x61 => "1 (Num. keypad)",
            0x62 => "2 (Num. keypad)",
            0x63 => "3 (Num. keypad)",
            0x64 => "4 (Num. keypad)",
            0x65 => "5 (Num. keypad)",
            0x66 => "6 (Num. keypad)",
            0x67 => "7 (Num. keypad)",
            0x68 => "8 (Num. keypad)",
            0x69 => "9 (Num. keypad)",
            0x6A => "*",
            0x6B => "+",
            0x6D => "-",
            0x6F => "/",
            0x70 => "F1",
            0x71 => "F2",
            0x72 => "F3",
            0x73 => "F4",
            0x74 => "F5",
            0x75 => "F6",
            0x76 => "F7",
            0x77 => "F8",
            0x78 => "F9",
            0x79 => "F10",
            0x7A => "F11",
            0x7B => "F12",
            0x90 => "", // Num Lock
            0x91 => "", // Scroll Lock
            0xBA => ";",
            0xBB => "=",
            0xBC => ",",
            0xBD => "-",
            0xBE => ".",
            0xBF => "/",
            0xC0 => "`",
            0xDB => "[",
            0xDC => "\\",
            0xDD => "]",
            0xDE => "'",
            _ => throw new Exception($"Unknown Keyboard Key {id}")
        };

        public string GetJoystickString(short flags)
        {
            string output = string.Empty;
            BitDict dict = new BitDict("Up", "Down", "Left", "Right", "Fire1", "Fire2", "Fire3", "Fire4");
            dict.Value = (ushort)flags;
            if (dict["Fire1"] || dict["Fire2"] || dict["Fire3"] || dict["Fire4"])
            {
                output += "Pressed ";
                bool add = false;
                if (dict["Fire1"])
                {
                    output += "fire 1";
                    add = true;
                }
                if (dict["Fire2"])
                {
                    if (add)
                        output += "+";
                    output += "fire 2";
                    add = true;
                }
                if (dict["Fire3"])
                {
                    if (add)
                        output += "+";
                    output += "fire 3";
                    add = true;
                }
                if (dict["Fire4"])
                {
                    if (add)
                        output += "+";
                    output += "fire 4";
                }
                output += ", ";
            }
            output += "Moved ";
            if (dict["Up"] && dict["Right"])
                output += "up+right";
            else if (dict["Up"] && dict["Left"])
                output += "up+left";
            else if (dict["Down"] && dict["Right"])
                output += "down+right";
            else if (dict["Down"] && dict["Left"])
                output += "down+left";
            else if (dict["Up"])
                output += "top";
            else if (dict["Down"])
                output += "down";
            else if (dict["Left"])
                output += "left";
            else if (dict["Right"])
                output += "right";
            return output;
        }

        public string GetObjectName() => GetObject().Name;

        public string GetPlayAreaString(short flags)
        {
            string output = string.Empty;
            BitDict dict = new BitDict("Left", "Right", "Top", "Bottom");
            dict.Value = (ushort)flags;
            if (!(dict["Left"] && dict["Right"] && dict["Top"] && dict["Bottom"]))
            {
                output += " on the ";
                bool add = false;
                if (dict["Bottom"])
                {
                    output += "bottom";
                    add = true;
                }
                if (dict["Top"])
                {
                    if (add)
                        output += ", ";
                    output += "top";
                    add = true;
                }
                if (dict["Left"])
                {
                    if (add)
                        output += ", ";
                    output += "left";
                    add = true;
                }
                if (dict["Right"])
                {
                    if (add)
                        output += ", ";
                    output += "right";
                }
                output.ReplaceLast(", ", " or ");
            }
            return output;
        }

        public string GetDirection(ParameterChunk param)
        {
            if (param is ParameterInt p)
            {
                string output = "...........";
                BitDict dict = new BitDict();
                dict.Value = (uint)p.Value;
                if (dict.Value == uint.MaxValue)
                    output += ".";
                else if (dict["31"])
                    output += "..........";
                else if (dict["30"])
                    output += ".........";
                else if (dict["29"] || dict["28"] || dict["27"])
                    output += "........";
                else if (dict["26"] || dict["25"] || dict["24"])
                    output += ".......";
                else if (dict["20"] || dict["21"] || dict["22"] || dict["23"])
                    output += "......";
                else if (dict["17"] || dict["18"] || dict["19"])
                    output += ".....";
                else if (dict["14"] || dict["15"] || dict["16"])
                    output += "....";
                else if (dict["10"] || dict["11"] || dict["12"] || dict["13"])
                    output += "...";
                else if (dict["7"] || dict["8"] || dict["9"])
                    output += "..";
                else if (dict["4"] || dict["5"] || dict["6"])
                    output += ".";
                return output;
            }
            else return param.ToString();
        }

        public string GetObjectAnimation(short id)
        {
            ObjectCommon oC = (ObjectCommon)GetObject().Properties;
            if (string.IsNullOrEmpty(oC.ObjectAnimations.Animations[id].Name))
            {
                return id switch
                {
                    0 => "Stopped",
                    1 => "Walking",
                    2 => "Running",
                    3 => "Appearing",
                    4 => "Disappearing",
                    5 => "Bouncing",
                    6 => "Launching",
                    7 => "Jumping",
                    8 => "Falling",
                    9 => "Climbing",
                    10 => "Crouch down",
                    11 => "Stand up",
                    _ => "Animation " + id
                };
            }
            else return oC.ObjectAnimations.Animations[id].Name;
        }
    }
}
