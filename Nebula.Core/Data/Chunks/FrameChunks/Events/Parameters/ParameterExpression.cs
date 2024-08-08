using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using System.Diagnostics;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExpression : ParameterChunk
    {
        public short ObjectType;
        public short Num;
        public ushort Size;
        public ExpressionChunk Expression = new();
        public ushort ObjectInfo;
        public short ObjectInfoList;

        public ParameterExpression()
        {
            ChunkName = "ParameterExpression";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long Debut = reader.Tell();
            if (NebulaCore.Fusion == 1.5f)
            {
                ObjectType = reader.ReadSByte();
                Num = reader.ReadSByte();

                if (ObjectType > 1 && Num >= 48)
                {
                    // MMF1.5 EXTBASE = 48
                    // MMF2   EXTBASE = 80
                    Num += 80 - 48;
                }
            }
            else
            {
                ObjectType = reader.ReadShort();
                Num = reader.ReadShort();
            }

            if (ObjectType == 0 && Num == 0)
                return;

            Size = reader.ReadUShort();
            if (ObjectType == -1)
            {
                Expression = Num switch
                {
                    0 => new ExpressionInt(),
                    3 => new ExpressionString(),
                    23 => new ExpressionDouble(),
                    24 or 50 => new ExpressionCommon(),
                    _ => new ExpressionChunk()
                };
            }
            else if (ObjectType > 1 || ObjectType == -7)
            {
                ObjectInfo = reader.ReadUShort();
                ObjectInfoList = reader.ReadShort();

                if (Num == 16 || Num == 19)
                    Expression = new ExpressionShort();
            }
            else if (ObjectType == 0)
                Expression = new ExpressionExtension();

            Debug.Assert(Size >= 6 - (NebulaCore.Fusion == 1.5f ? 2 : 0));
            Expression.ReadCCN(reader, Size - 6 + (NebulaCore.Fusion == 1.5f ? 2 : 0));
            reader.Seek(Debut + Size);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            if (FrameEvents.QualifierJumptable.ContainsKey(Tuple.Create(ObjectInfo, ObjectType)))
                ObjectInfo = FrameEvents.QualifierJumptable[Tuple.Create(ObjectInfo, ObjectType)];

            writer.WriteShort(ObjectType);
            writer.WriteShort(Num);

            if (ObjectType == 0 && Num == 0)
                return;

            ByteWriter expWriter = new ByteWriter(new MemoryStream());
            if (ObjectType > 1 || ObjectType == -7)
            {
                expWriter.WriteUShort(ObjectInfo);
                expWriter.WriteShort(ObjectInfoList);
            }

            Expression.WriteMFA(expWriter);

            writer.WriteUShort((ushort)(expWriter.Tell() + 6));
            //Debug.Assert(expWriter.Tell() + 6 == Size);
            writer.WriteWriter(expWriter);

            expWriter.Flush();
            expWriter.Close();
        }

        public override string ToString()
        {
            switch (ObjectType)
            {
                default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                case -7:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return $"score(\"Player {ObjectInfo + 1}\")";
                        case 1:
                            return $"lives(\"Player {ObjectInfo + 1}\")";
                        case 2:
                            return $"input(\"Player {ObjectInfo + 1}\")";
                        case 3:
                            return $"key$(\"Player {ObjectInfo + 1}\", ";
                        case 4:
                            return $"playername$(\"Player {ObjectInfo + 1}\")";
                    }
                case -6:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return "XMouse";
                        case 1:
                            return "YMouse";
                        case 2:
                            return "WheelDelta";
                    }
                case -5:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return "Total Objects";
                        case 1:
                            return "Last fixed value";
                    }
                case -4:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return "timer";
                        case 1:
                            return "hundreds";
                        case 2:
                            return "seconds";
                        case 3:
                            return "hours";
                        case 4:
                            return "minutes";
                        case 5:
                            return "eventindex";
                    }
                case -3:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return "frame";
                        case 1:
                            return "players";
                        case 2:
                            return "X Left Frame";
                        case 3:
                            return "X Right Frame";
                        case 4:
                            return "Y Top Frame";
                        case 5:
                            return "Y Bottom Frame";
                        case 6:
                            return "Frame Width";
                        case 7:
                            return "Frame Height";
                        case 8:
                            return "frame";
                        case 9:
                            return "CollisionMask(";
                        case 10:
                            return "FrameRate";
                        case 11:
                            return "VirtualWidth";
                        case 12:
                            return "VirtualHeight";
                        case 13:
                            return "FrameBkdColor";
                        case 14:
                            return "DisplayMode";
                        case 15:
                            return "PixelShaderVersion";
                        case 16:
                            return "FrameAlphaCoef";
                        case 17:
                            return "FrameRGBCoef";
                        case 18:
                            return "FrameEffectParam(";
                        case 19:
                            return "DPIScale";
                    }
                case -2:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 0:
                            return "SampleMainVolume";
                        case 1:
                            return "SampleVolume(";
                        case 2:
                            return "ChannelVolume(";
                        case 3:
                            return "SampleMainPan";
                        case 4:
                            return "SamplePan(";
                        case 5:
                            return "ChannelPan(";
                        case 6:
                            return "SamplePosition(";
                        case 7:
                            return "ChannelPosition(";
                        case 8:
                            return "SampleDuration(";
                        case 9:
                            return "ChannelDuration(";
                        case 10:
                            return "SampleFreq(";
                        case 11:
                            return "ChannelFreq(";
                        case 12:
                            return "ChannelSampleName$(";
                    }
                case -1:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case -3:
                            return ", ";
                        case -2:
                            return ")";
                        case -1:
                            return "(";
                        case 0:
                            return ((ExpressionInt)Expression).Value.ToString();
                        case 1:
                            return "Random(";
                        case 2:
                            return "Global Value(";
                        case 3:
                            return '"' + ((ExpressionString)Expression).Value + '"';
                        case 4:
                            return "Str$(";
                        case 5:
                            return "Val(";
                        case 6:
                            return "Appdrive$";
                        case 7:
                            return "Appdir$";
                        case 8:
                            return "Apppath$";
                        case 9:
                            return "Appname$";
                        case 10:
                            return "Sin(";
                        case 11:
                            return "Cos(";
                        case 12:
                            return "Tan(";
                        case 13:
                            return "Sqr(";
                        case 14:
                            return "Log(";
                        case 15:
                            return "Ln(";
                        case 16:
                            return "Hex$(";
                        case 17:
                            return "Bin$(";
                        case 18:
                            return "Exp(";
                        case 19:
                            return "Left$(";
                        case 20:
                            return "Right$(";
                        case 21:
                            return "Mid$(";
                        case 22:
                            return "Len(";
                        case 23:
                            return ((ExpressionDouble)Expression).Value.ToString();
                        case 24:
                            return GetGlobalValueName();
                        case 28:
                            return "Int(";
                        case 29:
                            return "Abs(";
                        case 30:
                            return "Ceil(";
                        case 31:
                            return "Floor(";
                        case 32:
                            return "ACos(";
                        case 33:
                            return "ASin(";
                        case 34:
                            return "ATan(";
                        case 35:
                            return "NOT(";
                        case 36:
                            return "NDropped";
                        case 37:
                            return "Dropped$(";
                        case 38:
                            return "CommandLine$";
                        case 39:
                            return "CommandItem$(";
                        case 40:
                            return "Min(";
                        case 41:
                            return "Max(";
                        case 42:
                            return "GetRGB(";
                        case 43:
                            return "GetRed(";
                        case 44:
                            return "GetGreen(";
                        case 45:
                            return "GetBlue(";
                        case 46:
                            return "LoopIndex(";
                        case 47:
                            return "NewLine$";
                        case 48:
                            return "Round(";
                        case 49:
                            return "Global String(";
                        case 50:
                            return GetGlobalStringName();
                        case 51:
                            return "Lower$(";
                        case 52:
                            return "Upper$(";
                        case 53:
                            return "Find(";
                        case 54:
                            return "ReverseFind(";
                        case 55:
                            return "ClipText$";
                        case 56:
                            return "AppTempPath$";
                        case 57:
                            return "BinFileTempName$(";
                        case 58:
                            return "FloatToString$(";
                        case 59:
                            return "ATan2(";
                        case 60:
                            return "0";
                        case 61:
                            return "\"\"";
                        case 62:
                            return "Distance(";
                        case 63:
                            return "VAngle(";
                        case 64:
                            return "Range(";
                        case 65:
                            return "RRandom(";
                        case 66:
                            return "RuntimeName$";
                        case 67:
                            return "ReplaceString$(";
                    }
                case 0:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case 2:
                            return " + ";
                        case 4:
                            return " - ";
                        case 6:
                            return " * ";
                        case 8:
                            return " / ";
                        case 10:
                            return " mod ";
                        case 12:
                            return " pow ";
                        case 14:
                            return " and ";
                        case 16:
                            return " or ";
                        case 18:
                            return " xor ";
                    }
                case > 0:
                    switch (Num)
                    {
                        default:
                            if (ObjectType < 32)
                                switch (Num)
                                {
                                    default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                    case 80:
                                        if (ObjectType == 2)
                                            return $"RGBAt(\"{GetObjectName()}\", ";
                                        else if (ObjectType == 3)
                                            return $"paragraph(\"{GetObjectName()}\")";
                                        else if (ObjectType == 7)
                                            return $"value(\"{GetObjectName()}\")";
                                        else if (ObjectType == 9)
                                            return $"App Frame Number(\"{GetObjectName()}\")";
                                        return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                    case 81:
                                        if (ObjectType == 2)
                                            return $"XScale(\"{GetObjectName()}\")";
                                        else if (ObjectType == 3)
                                            return $"string$(\"{GetObjectName()}\")";
                                        else if (ObjectType == 7)
                                            return $"minvalue(\"{GetObjectName()}\")";
                                        else if (ObjectType == 9)
                                            return $"App Global Value(\"{GetObjectName()}\", ";
                                        return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                    case 82:
                                        if (ObjectType == 2)
                                            return $"YScale(\"{GetObjectName()}\")";
                                        else if (ObjectType == 3)
                                            return $"paragraph$(\"{GetObjectName()}\", ";
                                        else if (ObjectType == 7)
                                            return $"maxvalue(\"{GetObjectName()}\")";
                                        else if (ObjectType == 9)
                                            return $"App Global String(\"{GetObjectName()}\", ";
                                        return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                    case 83:
                                        if (ObjectType == 2)
                                            return $"Angle(\"{GetObjectName()}\")";
                                        else if (ObjectType == 3)
                                            return $"value(\"{GetObjectName()}\")";
                                        else if (ObjectType == 7)
                                            return $"cColor(\"{GetObjectName()}\")";
                                        return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                    case 84:
                                        if (ObjectType == 3)
                                            return $"npara(\"{GetObjectName()}\")";
                                        else if (ObjectType == 7)
                                            return $"cColor2(\"{GetObjectName()}\")";
                                        return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                                }
                            switch (ObjectType)
                            {
                                default:
                                    return $"Expression ID {Num}(\"{GetObjectName()}\")";
                            }
                        case 1:
                            return $"Y(\"{GetObjectName()}\")";
                        case 2:
                            return $"Image(\"{GetObjectName()}\")";
                        case 3:
                            return $"Speed(\"{GetObjectName()}\")";
                        case 4:
                            return $"Acc(\"{GetObjectName()}\")";
                        case 5:
                            return $"Dec(\"{GetObjectName()}\")";
                        case 6:
                            return $"Dir(\"{GetObjectName()}\")";
                        case 7:
                            return $"X Left(\"{GetObjectName()}\")";
                        case 8:
                            return $"X Right(\"{GetObjectName()}\")";
                        case 9:
                            return $"Y Top(\"{GetObjectName()}\")";
                        case 10:
                            return $"Y Bottom(\"{GetObjectName()}\")";
                        case 11:
                            return $"X(\"{GetObjectName()}\")";
                        case 12:
                            return $"Fixed(\"{GetObjectName()}\")";
                        case 13:
                            return $"Flag(\"{GetObjectName()}\", ";
                        case 14:
                            return $"Anim Number(\"{GetObjectName()}\")";
                        case 15:
                            return $"NObjects(\"{GetObjectName()}\")";
                        case 16:
                            return $"{GetAlterableValueName()}(\"{GetObjectName()}\")";
                        case 17:
                            return $"SemiTrans(\"{GetObjectName()}\")";
                        case 18:
                            return $"NMovement(\"{GetObjectName()}\")";
                        case 19:
                            return $"{GetAlterableStringName()}(\"{GetObjectName()}\")";
                        case 20:
                            return $"FontName$(\"{GetObjectName()}\")";
                        case 21:
                            return $"FontSize(\"{GetObjectName()}\")";
                        case 22:
                            return $"FontColor(\"{GetObjectName()}\")";
                        case 23:
                            return $"Layer(\"{GetObjectName()}\")";
                        case 24:
                            return $"Gravity(\"{GetObjectName()}\")";
                        case 25:
                            return $"XActionPoint(\"{GetObjectName()}\")";
                        case 26:
                            return $"YActionPoint(\"{GetObjectName()}\")";
                        case 27:
                            return $"AlphaCoef(\"{GetObjectName()}\")";
                        case 28:
                            return $"RGBCoef(\"{GetObjectName()}\")";
                        case 29:
                            return $"EffectParam(\"{GetObjectName()}\", ";
                        case 30:
                            return $"AltValN(\"{GetObjectName()}\", ";
                        case 31:
                            return $"AltStrN$(\"{GetObjectName()}\", ";
                        case 32:
                            return $"ODistance(\"{GetObjectName()}\", ";
                        case 33:
                            return $"OAngle(\"{GetObjectName()}\", ";
                        case 34:
                            return $"ForEachLoopIndex(\"{GetObjectName()}\")";
                        case 35:
                            return $"PFriction(\"{GetObjectName()}\")";
                        case 36:
                            return $"PElasticity(\"{GetObjectName()}\")";
                        case 37:
                            return $"PDensity(\"{GetObjectName()}\")";
                        case 38:
                            return $"PVelocity(\"{GetObjectName()}\")";
                        case 39:
                            return $"PVelocityAngle(\"{GetObjectName()}\")";
                        case 40:
                            return $"OWidth(\"{GetObjectName()}\")";
                        case 41:
                            return $"OHeight(\"{GetObjectName()}\")";
                        case 42:
                            return $"PMass(\"{GetObjectName()}\")";
                        case 43:
                            return $"PAngularVelocity(\"{GetObjectName()}\")";
                        case 44:
                            return $"OName$(\"{GetObjectName()}\")";
                        case 45:
                            return $"NSelectedObjects(\"{GetObjectName()}\")";
                        case 46:
                            return $"InstanceValue(\"{GetObjectName()}\")";
                    }
            }
        }

        public ObjectInfo? GetObject()
        {
            if (Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo).Any() == true)
                return null;
            else if (NebulaCore.MFA && Parent?.FrameEvents?.EventObjects.Count > 0)
                return NebulaCore.PackageData.FrameItems.Items[(int)Parent.FrameEvents.EventObjects[ObjectInfo].ItemHandle];
            else
                return NebulaCore.PackageData.FrameItems.Items[ObjectInfo];
        }

        public string GetGlobalValueName()
        {
            short id = (short)((uint)((ExpressionCommon)Expression).Value << 16);
            if (NebulaCore.PackageData.GlobalValueNames.Names.Length <= id || string.IsNullOrEmpty(NebulaCore.PackageData.GlobalValueNames.Names[id]))
            {
                string output = "Global Value ";
                if (id > 25)
                    output += (char)('A' + Math.Floor(id / 26d));
                output += (char)('A' + id % 26);
                return output;
            }
            else return NebulaCore.PackageData.GlobalValueNames.Names[id];
        }

        public string GetGlobalStringName()
        {
            short id = (short)((uint)((ExpressionCommon)Expression).Value << 16);
            if (NebulaCore.PackageData.GlobalStringNames.Names.Length <= id || string.IsNullOrEmpty(NebulaCore.PackageData.GlobalStringNames.Names[id]))
            {
                string output = "Global String ";
                if (id > 25)
                    output += (char)('A' + Math.Floor(id / 26d));
                output += (char)('A' + id % 26);
                return output;
            }
            else return NebulaCore.PackageData.GlobalStringNames.Names[id];
        }

        public string GetAlterableValueName()
        {
            short id = ((ExpressionShort)Expression).Value;
            string output = "Alterable Value ";
            if (id > 25)
                output += (char)('A' + Math.Floor(id / 26d));
            output += (char)('A' + id % 26);
            return output;
        }

        public string GetAlterableStringName()
        {
            short id = ((ExpressionShort)Expression).Value;
            string output = "Alterable String ";
            if (id > 25)
                output += (char)('A' + Math.Floor(id / 26d));
            output += (char)('A' + id % 26);
            return output;
        }

        public string GetObjectName()
        {
            ObjectInfo? objectInfo = GetObject();
            if (objectInfo != null)
                return objectInfo.Name;
            Qualifier[] qualifier = Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo && x.Type == ObjectType).ToArray()!;
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
    }
}
