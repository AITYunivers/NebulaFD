using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public class ACEventBase : Chunk
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
        public ushort ObjectInfo;
        public short ObjectInfoList;
        public Parameter[] Parameters = new Parameter[0];
        public byte DefType;

        public Event Parent = null;
        public bool DoAdd = true;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            throw new NotImplementedException();
        }

        public ObjectInfo? GetObject()
        {
            if (Parent?.Parent.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo).Any() == true)
                return null;
            else if (NebulaCore.MFA && Parent?.Parent.EventObjects.Count > 0)
                return NebulaCore.PackageData.FrameItems.Items[(int)Parent.Parent.EventObjects[ObjectInfo].ItemHandle];
            else
                return NebulaCore.PackageData.FrameItems.Items[ObjectInfo];
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
                        output += (char)('A' + Math.Floor(id / 27.0));
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
                        output += (char)('A' + Math.Floor(id / 27.0));
                    output += (char)('A' + id % 27);
                    return output;
                }
                else return NebulaCore.PackageData.GlobalStringNames.Names[id];
            }
            else return $"Global String({param})";
        }

        public string GetObjectAlterableValueName(ParameterChunk parameter)
        {
            if (parameter is ParameterShort idData)
            {
                short id = idData.Value;
                ObjectCommon? oC = (ObjectCommon)GetObject()?.Properties;
                if (oC == null || oC.ObjectAlterableValues.Names.Length <= id || string.IsNullOrEmpty(oC.ObjectAlterableValues.Names[id]))
                {
                    string output = "Alterable Value ";
                    if (id > 26)
                        output += (char)('A' + Math.Floor(id / 27.0));
                    output += (char)('A' + id % 27);
                    return output;
                }
                else return oC.ObjectAlterableValues.Names[id];
            }
            else return "Alterable Value(" + parameter.ToString() + ")";
        }

        public string GetObjectAlterableStringName(ParameterChunk parameter)
        {
            if (parameter is ParameterShort idData)
            {
                short id = idData.Value;
                ObjectCommon? oC = (ObjectCommon)GetObject()?.Properties;
                if (oC == null || oC.ObjectAlterableStrings.Names.Length <= id || string.IsNullOrEmpty(oC.ObjectAlterableStrings.Names[id]))
                {
                    string output = "Alterable String ";
                    if (id > 26)
                        output += (char)('A' + Math.Floor(id / 27.0));
                    output += (char)('A' + id % 27);
                    return output;
                }
                else return oC.ObjectAlterableStrings.Names[id];
            }
            else return "Alterable String(" + parameter.ToString() + ")";
        }

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
            0x6E => ".",
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

        public string GetObjectName()
        {
            ObjectInfo? objectInfo = GetObject();
            if (objectInfo != null)
                return objectInfo.Name;
            Qualifier[] qualifier = Parent?.Parent.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo && x.Type == ObjectType).ToArray()!;
            if (qualifier.Length > 0)
                return GetQualifierName(qualifier.First());
            return "Unknown Object";
        }

        public string GetQualifierName(Qualifier qualifier)
        {
            string qualifierName = "Group." + (qualifier.ObjectInfo & 0x7FFF) switch
            {
                0 => "Player",
                1 => "Good",
                2 => "Neutral",
                3 => "Bad",
                4 => "Enemies",
                5 => "Friends",
                6 => "Bullets",
                7 => "Arms",
                8 => "Bonus",
                9 => "Collectables",
                10 => "Traps",
                11 => "Doors",
                12 => "Keys",
                13 => "Texts",
                14 => "0",
                15 => "1",
                16 => "2",
                17 => "3",
                18 => "4",
                19 => "5",
                20 => "6",
                21 => "7",
                22 => "8",
                23 => "9",
                24 => "Parents",
                25 => "Children",
                26 => "Data",
                27 => "Timed",
                28 => "Engine",
                29 => "Areas",
                30 => "Reference Points",
                31 => "Radar Enemies",
                32 => "Radar Friends",
                33 => "Radar Neutrals",
                34 => "Music",
                35 => "Sound",
                36 => "Waveform",
                37 => "Background Scenery",
                38 => "Foreground Scenery",
                39 => "Decorations",
                40 => "Water",
                41 => "Clouds",
                42 => "Empty",
                43 => "Fog",
                44 => "Flowers",
                45 => "Animals",
                46 => "Bosses",
                47 => "NPC",
                48 => "Vehicles",
                49 => "Rockets",
                50 => "Balls",
                51 => "Bombs",
                52 => "Explosions",
                53 => "Particles",
                54 => "Clothes",
                55 => "Glow",
                56 => "Arrows",
                57 => "Buttons",
                58 => "Cursors",
                59 => "Drawing Tools",
                60 => "Indicator",
                61 => "Shapes",
                62 => "Shields",
                63 => "Shifting Blocks",
                64 => "Magnets",
                65 => "Negative Matter",
                66 => "Neutral Matter",
                67 => "Positive Matter",
                68 => "Breakable",
                69 => "Dissolving",
                70 => "Dialogue",
                71 => "HUD",
                72 => "Inventory",
                73 => "Inventory Item",
                74 => "Interface",
                75 => "Movable",
                76 => "Perspective",
                77 => "Calculation Objects",
                78 => "Invisible",
                79 => "Masks",
                80 => "Obstacles",
                81 => "Value Holder",
                82 => "Helpful",
                83 => "Powerups",
                84 => "Targets",
                85 => "Trapdoors",
                86 => "Dangers",
                87 => "Forbidden",
                88 => "Physical objects",
                89 => "3D Objects",
                90 => "Generic 1",
                91 => "Generic 2",
                92 => "Generic 3",
                93 => "Generic 4",
                94 => "Generic 5",
                95 => "Generic 6",
                96 => "Generic 7",
                97 => "Generic 8",
                98 => "Generic 9",
                99 => "Generic 10",
                _ => string.Empty
            };

            qualifierName += "." + qualifier.Type switch
            {
                2 => "Sprite",
                3 => "Text",
                4 => "Question",
                5 => "Score",
                6 => "Lives",
                7 => "Counter",
                _ => string.Empty
            };

            if (qualifier.Type >= 32 && NebulaCore.PackageData.Extensions.Exts.ContainsKey(qualifier.Type - 32))
            {
                Extension ext = NebulaCore.PackageData.Extensions.Exts[qualifier.Type - 32];
                qualifierName += ext.Name;
            }

            if (qualifierName.StartsWith("Group.."))
                return "Unknown Qualifier";
            else
                return qualifierName;
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
            ObjectCommon? oC = (ObjectCommon)GetObject()?.Properties;
            if (oC != null && !oC.ObjectAnimations.Animations.ContainsKey(id))
                return "Deleted Animation " + id;
            if (oC == null || string.IsNullOrEmpty(oC.ObjectAnimations.Animations[id].Name))
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

        public string GetFrameName(short id)
        {
            try
            {
                return $"\"{NebulaCore.PackageData.Frames[NebulaCore.PackageData.FrameHandles[id]].FrameName}\" ({NebulaCore.PackageData.FrameHandles[id] + 1})";
            }
            catch
            {
                return "(ERROR: UNKNOWN FRAME)";
            }
        }

        public string GetDirectionSettings(short id)
        {
            return id switch
            {
                0 => " (not an obstacle)",
                1 => " as obstacle",
                2 => " as platform",
                3 => " as ladder",
                4 => "",
                _ => throw new Exception("Unknown Direction Setting")
            };
        }

        public string GetInkEffect(ParameterDoubleShort param)
        {
            return param.Value1 switch
            {
                0 => "None",
                1 => $"Semi-transparent ({param.Value2}%)",
                2 => "Inverted",
                3 => "XOR",
                4 => "AND",
                5 => "OR",
                9 => "Add",
                10 => "Monochrome",
                11 => "Subtract",
                _ => throw new Exception("Unknown Ink Effect")
            };
        }

        public string GetAlterableValueName(ParameterChunk parameter)
        {
            if (parameter is ParameterInt idData)
            {
                int id = idData.Value;
                string output = "Alterable Value ";
                if (id > 25)
                    output += (char)('A' + Math.Floor(id / 26d));
                output += (char)('A' + id % 26);
                return output;
            }
            else return "Alterable Value(" + parameter.ToString() + ")";
        }

        public string GetAlterableStringName(ParameterChunk parameter)
        {
            if (parameter is ParameterInt idData)
            {
                int id = idData.Value;
                string output = "Alterable String ";
                if (id > 25)
                    output += (char)('A' + Math.Floor(id / 26d));
                output += (char)('A' + id % 26);
                return output;
            }
            else return "Alterable String(" + parameter.ToString() + ")";
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
    }
}
