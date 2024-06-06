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
using System.Linq.Expressions;
using ZelTranslator_SD.Parsers;
using ParameterExpression = Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters.ParameterExpression;

namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class GMS2Expressions
    {
        public static int try_val(string key, Dictionary<string, int> dict)
        {
            try
            {
                if (dict.TryGetValue(key, out int value))
                {
                    return value;
                }
                else
                {
                    return -5552471;
                }
            }
            catch
            {
                return -5552471;
            }
        }
        public static void NoParam(ParameterExpression expParam, PackageData gameData, ref List<string> missing_code)
        {
            var obj = gameData.FrameItems.Items[expParam.ObjectInfo];
            string errStr = $"Obj {expParam.ObjectType} ~ Parameter {expParam.Num} not implemented";
            if (obj.Properties is ObjectCommon common)
            {
                errStr = $"ObjCmn {expParam.ObjectType} [{common.Identifier}] ~ Parameter {expParam.Num} not implemented";
            }
            missing_code.Add(errStr);
            throw new Exception(errStr);
        }
        public static string Evaluate(ParameterExpressions expression, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string buildString = "";
            int expParamCount = 0;

            foreach (var exp in expression.Expressions)
            {
                var expParam = exp.Expression;

                var intExp = expParam as ExpressionInt;
                var doubleExp = expParam as ExpressionDouble;
                var shortExp = expParam as ExpressionShort;
                var stringExp = expParam as ExpressionString;
                var globalCommon = expParam as ExpressionCommon;

                switch (exp.ObjectType)
                {
                    case -6: // Keyboard/Mouse
                        {
                            switch (exp.Num)
                            {
                                case 0: // XMouse
                                    buildString += "mouse_x";
                                    break;
                                case 1: // YMouse
                                    buildString += "mouse_y";
                                    break;
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case -3: // Constants
                        {
                            switch (exp.Num)
                            {

                                case 6: // FrameWidth
                                    buildString += "room_width";
                                    break;
                                case 7: // FrameHeight
                                    buildString += "room_height";
                                    break;
                                case 0: // CurrentFrameOld (?)
                                case 8: // CurrentFrame -- FIX/REWORK THIS: rooms in gms2 don't really correspond to base-0 numbers
                                    buildString += "room+1";
                                    break;
                                case 10: // FrameRate
                                    buildString += "fps";
                                    break;
                                case 11: // VirtualWidth -- FIX/REWORK THIS: actually implement accurate Frame/Virtual Width & Height for frames
                                    buildString += "Frame.VirtualWidth";
                                    break;
                                case 12: // VirtualHeight
                                    buildString += "Frame.VirtualHeight";
                                    break;
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case -2: // Sound
                        {
                            if (GMS2Writer.Args["-nosnds"]) throw new Exception("Ignoring sound parameter");
                            switch (exp.Num)
                            {
                                case 2: // ChannelVolume(
                                    buildString += "ChannelVolume(";
                                    break;
                                //case 6: // SamplePosition
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case -1: // Special/General numbers
                        {
                            switch (exp.Num)
                            {
                                case -3: // , comma
                                    buildString += ", ";
                                    break;
                                case -2: // ) close parentheses
                                    buildString += ")";
                                    break;
                                case -1: // ( open parentheses
                                    buildString += "(";
                                    break;
                                case 0: // intExp
                                    buildString += $"{intExp.Value}";
                                    break;
                                case 1: // Random
                                    buildString += $"irandom(";
                                    break;
                                case 2: // Global Value from number expression
                                case 49: // Global String from number expression
                                    buildString += $"Global(\"{(exp.Num == 2 ? "val" : "str")}\", ";
                                    break;
                                case 3: // StringExp
                                    buildString += $"\"{stringExp.Value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
                                    break;
                                case 4: // Str$()
                                    buildString += $"string(";
                                    break;
                                case 5: // Val("number")
                                    buildString += $"real(";
                                    break;
                                case 10: // Sin()
                                    buildString += $"dsin(";
                                    break;
                                case 11: // Cos()
                                    buildString += $"dcos(";
                                    break;
                                case 12: // Tan()
                                    buildString += $"dtan(";
                                    break;
                                case 13: // Sqr()
                                    buildString += $"sqrt(";
                                    break;
                                case 19: // Left$(
                                    buildString += "LeftStr(";
                                    break;
                                case 20: // Right$(
                                    buildString += "RightStr(";
                                    break;
                                case 21: // Mid$( lol
                                    buildString += "MidStr(";
                                    break;
                                case 22: // Len(
                                    buildString += $"string_length(";
                                    break;
                                case 23: // DoubleExp
                                    buildString += $"{doubleExp.Value2}";
                                    break;
                                case 24: // Global Value _
                                case 50: // Global String _
                                    buildString += $"global.Global{(exp.Code == 24 ? "Values" : "Strings")}[{(exp.Code == 24 ? globalCommon.Value : globalCommon.Value + 65536)}]";
                                    break;
                                case 29: // Abs()
                                    buildString += "abs(";
                                    break;
                                case 30: // Ceil()
                                    buildString += "ceil(";
                                    break;
                                case 31: // Floor()
                                    buildString += "floor(";
                                    break;
                                case 40: // Min()
                                    buildString += "min(";
                                    break;
                                case 41: // Max()
                                    buildString += "max(";
                                    break;
                                case 47: // NewLine$
                                    buildString += "\"\\n\"";
                                    break;
                                case 51: // Lower$(
                                    buildString += "string_lower(";
                                    break;
                                case 52: // Upper$(
                                    buildString += "string_upper(";
                                    break;
                                case 53: // Find(
                                    buildString += "Find(";
                                    break;
                                case 54: // ReverseFind(
                                    buildString += "ReverseFind(";
                                    break;
                                case 64: // Clamp()
                                    buildString += "clamp(";
                                    break;
                                case 65: // RandomRange
                                    buildString += "irandom_range(";
                                    break;
                                case 68: // ReplaceString$(
                                    buildString += "string_replace_all(";
                                    break;
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case 0: // Operators
                        {
                            switch (exp.Num)
                            {
                                case 0: // End (?)
                                    break;
                                case 2: // + addition symbol
                                    buildString += " + ";
                                    break;
                                case 4: // - subtract symbol
                                    buildString += " - ";
                                    break;
                                case 6: // * multiply symbol
                                    buildString += " * ";
                                    break;
                                case 8: // / division symbol
                                    buildString += " / ";
                                    break;
                                case 10: // % modulous symbol
                                    buildString += " % ";
                                    break;
                                case 12: // Power (currently impossible to implement without coding a function to completely restructure the translated expression)
                                    buildString += " POW ";
                                    break;
                                case 14: // AND
                                    buildString += " & ";
                                    break;
                                case 16: // OR
                                    buildString += " | ";
                                    break;
                                case 18: // XOR
                                    buildString += " ^ ";
                                    break;
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case 2: // Active Objects
                        {
                            string actobjName = GMS2Writer.ObjectName(exp, gameData, true);
                            switch (exp.Num)
                            {
                                case 1: // Y Position (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.y";
                                    break;
                                case 2: // Current Animation Frame (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.image_index";
                                    break;
                                case 3: // Movement speed (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.Movement.spd";
                                    break;
                                case 6: // Direction (FIX/REWORK to work with multiple instances)
                                    buildString += $"({actobjName}.direction/11.25)";
                                    break;
                                case 9: // Y Top
                                case 10: // Y Bottom
                                    buildString += $"{actobjName}.bbox_{(exp.Num == 9 ? "top" : "bottom")}";
                                    break;
                                case 11: // X Position (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.x";
                                    break;
                                case 13: // Flags (FIX/REWORK to work with multiple instances)
                                    buildString += $"Flag({actobjName}, {evnt}, ";
                                    break;
                                case 14: // Current Animation (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.animation";
                                    break;
                                case 15: // Number of object instances
                                    buildString += $"instance_number({actobjName})";
                                    break;
                                case 16: // Alterable Value (FIX/REWORK to work with multiple instances)
                                    buildString += $"Alterable({actobjName}, \"val\", {evnt}, {shortExp.Value})";
                                    break;
                                case 17: // SemiTrans (FIX/REWORK to work with multiple instances)
                                    buildString += $"((1 - {actobjName}.image_alpha)*128.0)";
                                    break;
                                case 18: // NMovement (the current movement index)
                                    buildString += $"{actobjName}.movement";
                                    break;
                                case 19: // Alterable Strings (FIX/REWORK to work with multiple instances)
                                    buildString += $"Alterable({actobjName}, \"str\", {evnt}, {shortExp.Value})";
                                    break;
                                case 25: // XActionPoint
                                case 26: // YActionPoint
                                    buildString += $"GetGeneralValue({actobjName}, \"{(exp.Num == 25 ? "X" : "Y")}ActionPoint\", \"val\", {evnt})";
                                    break;
                                case 27: // BlendCoeff (FIX/REWORK to work with multiple instances)
                                    buildString += $"((1 - {actobjName}.image_alpha)*256.0)";
                                    break;
                                case 30: // Alterable Value by specific number [ AltValN() ]
                                    buildString += $"Alterable({actobjName}, \"val\", {evnt}, ";
                                    break;
                                case 31: // Alterable String by specific number [ AltStrN$() ]
                                    buildString += $"Alterable({actobjName}, \"str\", {evnt}, ";
                                    break;
                                case 40: // Width (in pixels) (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.sprite_width";
                                    break;
                                case 41: // Height (in pixels) (FIX/REWORK to work with multiple instances)
                                    buildString += $"{actobjName}.sprite_height";
                                    break;
                                case 32: // ODistance(obj, x, y) -- Returns distance between object and point (X,Y)
                                    buildString += $"point_distance(GetGeneralValue({actobjName}, \"x\", \"val\", {evnt}), GetGeneralValue({actobjName}, \"y\", \"val\", {evnt}), ";
                                    break;
                                case 33: // OAngle(obj, x, y) -- Returns angle vector between object and point (X,Y)
                                    buildString += $"point_direction(GetGeneralValue({actobjName}, \"x\", \"val\", {evnt}), GetGeneralValue({actobjName}, \"y\", \"val\", {evnt}), ";
                                    break;
                                case 81: // XScale
                                case 82: // YScale
                                    buildString += $"GetGeneralValue({actobjName}, \"image_{(exp.Num == 81 ? "x" : "y")}scale\", \"val\", {evnt})";
                                    break;
                                // case 83: // Angle
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case 3: // Strings
                        {
                            string StringName = GMS2Writer.ObjectName(exp, gameData, true);
                            switch (exp.Num)
                            {
                                case 80: // Current number of paragraph displayed
                                    buildString += $"GetGeneralValue({StringName}, \"paragraph\", \"str\", {evnt})";
                                    break;
                                case 81: // Alterable string
                                    buildString += $"AlterableString({StringName}, {evnt})";
                                    break;
                                case 82: // Text of a paragraph
                                    buildString += $"ParagraphText({StringName}, {evnt}, ";
                                    break;
                                case 83: // Numeric value of current string
                                    buildString += $"StringVal({StringName}, {evnt})";
                                    break;
                                case 84: // Number of paragraphs
                                    buildString += $"NPara({StringName}, {evnt})";
                                    break;
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }
                    case 7: // Counters
                        {
                            string CounterName = GMS2Writer.ObjectName(exp, gameData, true);
                            switch (exp.Num)
                            {
                                case 80: // Counter value
                                    buildString += $"CounterValue({CounterName}, {evnt})";
                                    break;
                                case 81: // Counter minimum
                                case 82: // Counter maximum
                                    string minmax = "";
                                    if (exp.Num == 81) minmax = "min";
                                    else if (exp.Num == 82) minmax = "max";
                                    buildString += $"CounterMinMax({CounterName}, {minmax}, {evnt})";
                                    break;
                                default:
                                    NoParam(exp, gameData, ref missing_code);
                                    break;
                            }
                            break;
                        }

                    default:
                        {
                            #region ExtensionExpressions
                            if (exp.ObjectType == try_val("SYSO", extCodes)) // OS object
                            {
                                switch (exp.Num)
                                {
                                    case 81: // ComputerName$
                                        buildString += "ComputerName()";
                                        break;
                                    default:
                                        NoParam(exp, gameData, ref missing_code);
                                        break;
                                }
                            }
                            if (exp.ObjectType == try_val("2YOJ", extCodes)) // Joystick 2 Object
                            {
                                switch (exp.Num)
                                {
                                    case 80: // Raw X Value
                                        buildString += $"RawX(";
                                        break;
                                    case 81: // Raw Y Value
                                        buildString += $"RawY(";
                                        break;
                                    default:
                                        NoParam(exp, gameData, ref missing_code);
                                        break;
                                }
                            }
                            else if (exp.ObjectType == try_val("0I", extCodes) || exp.ObjectType == try_val("0INI", extCodes)) // Ini object
                            {
                                var ini_name = GMS2Writer.ObjectName(exp, gameData);
                                switch (exp.Num)
                                {
                                    case 80: // Get value
                                        buildString += $"Exp_GetValue({ini_name}, ";
                                        break;
                                    case 81: // Get string
                                        buildString += $"Exp_GetString({ini_name}, ";
                                        break;
                                    case 82: // Get value (item)
                                        buildString += $"Exp_GetValueItem({ini_name}, ";
                                        break;
                                    case 83: // Get value (group - item)
                                        buildString += $"Exp_GetValueGroupItem({ini_name}, ";
                                        break;
                                    case 84: // Get string (item)
                                        buildString += $"Exp_GetStringItem({ini_name}, ";
                                        break;
                                    case 85: // Get string (group - item)
                                        buildString += $"Exp_GetStringGroupItem({ini_name}, ";
                                        break;
                                    default:
                                        NoParam(exp, gameData, ref missing_code);
                                        break;
                                }
                            }
                            else
                            {   // If object/extension expression parameter not implemented yet (erroring like this might not work well)
                                NoParam(exp, gameData, ref missing_code);
                            }
                            break;
                            #endregion
                        }

                }
                expParamCount++;
            }

            return buildString;
        }
    }
}