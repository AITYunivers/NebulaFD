using ILGPU.IR.Types;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Memory;
using Nebula.Tools.ZelTranslator_SD.GDevelop;
using System.Linq.Expressions;
using System;
using ParameterExpression = Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters.ParameterExpression;
using ILGPU.Runtime.Cuda;
using System.Reflection.Emit;
using ILGPU.Runtime;

namespace ZelTranslator_SD.Parsers.GDevelop
{
    public static class GDEventWriter
    {
        public static int EventIndex = 0;

        public static GDJSON.Condition? Translate(Condition conditionData)
        {
            GDJSON.Condition output = new GDJSON.Condition();
            List<string> parameters = new();
            bool forceNegated = false;
            switch (conditionData.ObjectType)
            {
                case -7:
                    switch (conditionData.Num)
                    {
                        case -6:
                            // "Repeat while Player {ObjectInfo + 1} {GetJoystickString(((ParameterShort)Parameters[0].Data).Value)}";
                            break;
                        case -5:
                            // "Number of lives of Player {ObjectInfo + 1} reaches 0";
                            break;
                        case -4:
                            // "Player {ObjectInfo + 1} {GetJoystickString(((ParameterShort)Parameters[0].Data).Value)}";
                            break;
                        case -3:
                            // "Number of lives of Player {ObjectInfo + 1} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                            break;
                        case -2:
                            // "Score of Player {ObjectInfo + 1} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                            break;
                    }
                    break;
                case -6:
                    switch (conditionData.Num)
                    {
                        case -12:
                            output.type.value = "IsMouseWheelScrollingDown";
                            parameters.Add("");
                            break;
                        case -11:
                            output.type.value = "IsMouseWheelScrollingUp";
                            parameters.Add("");
                            break;
                        case -10:
                            // "Mouse cursor is displayed";
                            break;
                        case -9:
                            output.type.value = "AnyKeyPressed";
                            parameters.Add("");
                            break;
                        case -8:
                            /*string return8 = "Repeat while ";
                            return8 += ((ParameterShort)Parameters[0].Data).Value switch
                            {
                                1 => "left",
                                2 => "right",
                                4 => "middle",
                                _ => "left"
                            };
                            return return8 + " mouse-key is pressed";*/
                            break;
                        case -7:
                            /*ParameterClick click7 = (ParameterClick)Parameters[0].Data;
                            string return7 = $"User {(click7.IsDouble != 0 ? "double-" : "")}clicks with ";
                            return7 += click7.Button switch
                            {
                                0 => "left",
                                1 => "middle",
                                2 => "right",
                                _ => "left"
                            };
                            return return7 + $" button on {Parameters[1]}";*/
                            break;
                        case -6:
                            /*ParameterClick click6 = (ParameterClick)Parameters[0].Data;
                            string return6 = $"User {(click6.IsDouble != 0 ? "double-" : "")}clicks with ";
                            return6 += click6.Button switch
                            {
                                0 => "left",
                                1 => "middle",
                                2 => "right",
                                _ => "left"
                            };
                            return return6 + $" button within zone {Parameters[1]}";*/
                            break;
                        case -5:
                            {
                                if (conditionData.Parameters[0].Data is ParameterClick click && click.IsDouble == 0)
                                {
                                    output.type.value = "MouseButtonPressed";
                                    parameters.Add("");
                                    parameters.Add(click.Button switch
                                    {
                                        0 => "\"Left\"",
                                        1 => "\"Middle\"",
                                        2 => "\"Right\"",
                                        _ => "\"Left\""
                                    });
                                }
                            }
                            // User (double-)clicks with ? button
                            break;
                        case -4:
                            // "Mouse pointer is over {Parameters[0]}";
                            break;
                        case -3:
                            // "Mouse pointer lays within zone {Parameters[0]}";
                            break;
                        case -2:
                            // "Repeat while \"{GetKeyboardKey(((ParameterShort)Parameters[0].Data).Value)}\" is pressed";
                            break;
                        case -1:
                            {
                                output.type.value = "KeyPressed";
                                parameters.Add("");
                                if (conditionData.Parameters[0].Data is ParameterShort shortParam)
                                    parameters.Add(GDKeyCodes.ShortKeyCodes[shortParam.Value]);
                            }
                            break;
                    }
                    break;
                case -5:
                    switch (conditionData.Num)
                    {
                        case -23:
                            // "Pick all objects in line ({Parameters[0]},{Parameters[1]}) to ({Parameters[2]},{Parameters[3]})";
                            break;
                        case -22:
                            // "Pick objects with Flag {Parameters[0]} off";
                            break;
                        case -21:
                            // "Pick objects with Flag {Parameters[0]} on";
                            break;
                        case -20:
                            // "Pick objects which {GetAlterableValueName(Parameters[0].Data)} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                            break;
                        case -19:
                            // "Pick objects with fixed value {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                            break;
                        case -18:
                            // "Pick all objects in zone {Parameters[0]}";
                            break;
                        case -17:
                            // "Pick an object at random";
                            break;
                        case -16:
                            // "Pick a random object from zone {Parameters[0]}";
                            break;
                        case -15:
                            // "Total number of objects {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                            break;
                        case -14:
                            // "Number of objects in zone {Parameters[0]} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                            break;
                        case -13:
                            // "No more objects in zone {Parameters[0]}";
                            break;
                    }
                    break;
                case -4:
                    switch (conditionData.Num)
                    {
                        case -8:
                            {
                                output.type.value = "RepeatEveryXSeconds::Repeat";
                                parameters.Add("");
                                parameters.Add($"\"{EventIndex++}\"");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add($"({Translate(exps, out bool isString)}) / 1000");
                                if (conditionData.Parameters[0].Data is ParameterTimer timer)
                                    parameters.Add((timer.Timer / 1000.0).ToString());
                                else
                                    parameters.Add($"({conditionData.Parameters[0]}) / 1000");
                                GDWriter.extensions.Add("Every");
                            }
                            break;
                        case -7:
                            {
                                output.type.value = "BuiltinCommonInstructions::CompareNumbers";
                                parameters.Add("round(TimeFromStart() * 1000)");
                                parameters.Add(">=");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add(Translate(exps, out bool isString));
                                if (conditionData.Parameters[0].Data is ParameterTimer timer)
                                    parameters.Add(timer.Timer.ToString());
                                else
                                    parameters.Add(conditionData.Parameters[0].ToString());
                            }
                            // "Timer equals {Parameters[0]}";
                            break;
                        case -6:
                            // "On timer event {Parameters[0]}";
                            break;
                        case -5:
                            // "User has left the computer for {Parameters[0]}";
                            break;
                        case -4:
                            {
                                output.type.value = "RepeatEveryXSeconds::Repeat";
                                parameters.Add("");
                                parameters.Add($"\"{EventIndex++}\"");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add($"({Translate(exps, out bool isString)}) / 1000");
                                if (conditionData.Parameters[0].Data is ParameterTimer timer)
                                    parameters.Add((timer.Timer / 1000.0).ToString());
                                else
                                    parameters.Add($"({conditionData.Parameters[0]}) / 1000");
                                GDWriter.extensions.Add("Every");
                            }
                            break;
                        case -2:
                            {
                                output.type.value = "BuiltinCommonInstructions::CompareNumbers";
                                parameters.Add("round(TimeFromStart() * 1000)");
                                parameters.Add("<");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add(Translate(exps, out bool isString));
                                if (conditionData.Parameters[0].Data is ParameterTimer timer)
                                    parameters.Add(timer.Timer.ToString());
                                else
                                    parameters.Add(conditionData.Parameters[0].ToString());
                            }
                            break;
                        case -1:
                            {
                                output.type.value = "BuiltinCommonInstructions::CompareNumbers";
                                parameters.Add("round(TimeFromStart() * 1000)");
                                parameters.Add(">");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add(Translate(exps, out bool isString));
                                if (conditionData.Parameters[0].Data is ParameterTimer timer)
                                    parameters.Add(timer.Timer.ToString());
                                else
                                    parameters.Add(conditionData.Parameters[0].ToString());
                            }
                            break;
                    }
                    break;
                case -3:
                    switch (conditionData.Num)
                    {
                        case -10:
                            // "Frame position has just been saved";
                            break;
                        case -9:
                            // "Frame position has just been loaded";
                            break;
                        case -8:
                            output.type.value = "SceneJustResumed";
                            parameters.Add("");
                            break;
                        case -7:
                            // "V-Sync is enabled";
                            break;
                        case -6:
                            // "{Parameters[0]},{Parameters[1]} is a ladder";
                            break;
                        case -5:
                            // "{Parameters[0]},{Parameters[1]} is an obstacle";
                            break;
                        case -4:
                            // "End of Application";
                            break;
                        case -2:
                            // "End of Frame";
                            break;
                        case -1:
                            output.type.value = "DepartScene";
                            parameters.Add("");
                            break;
                    }
                    break;
                case -2:
                    switch (conditionData.Num)
                    {
                        case -9:
                            {
                                output.type.value = "SoundPaused";
                                parameters.Add("");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add(Translate(exps, out bool isString));
                                else
                                    parameters.Add(conditionData.Parameters[0].ToString());
                            }
                            break;
                        case -8:
                            {
                                output.type.value = "SoundPlaying";
                                forceNegated = true;
                                parameters.Add("");
                                if (conditionData.Parameters[0].Data is ParameterExpressions exps)
                                    parameters.Add(Translate(exps, out bool isString));
                                else
                                    parameters.Add(conditionData.Parameters[0].ToString());
                            }
                            break;
                        case -7:
                            {
                                output.type.value = "MusicPaused";
                                parameters.Add("");
                                parameters.Add("0");
                            }
                            break;
                        case -6:
                            // "Is sample {Parameters[0]} paused?";
                            break;
                        case -4:
                            {
                                output.type.value = "MusicPlaying";
                                forceNegated = true;
                                parameters.Add("");
                                parameters.Add("0");
                            }
                            break;
                        case -3:
                            // "No sample is playing";
                            break;
                        case -1:
                            // "{Parameters[0]} is not playing";
                            break;
                    }
                    break;
                case -1:
                    switch (conditionData.Num)
                    {
                        case -42:
                            output.type.value = "BuiltinCommonInstructions::Always";
                            forceNegated = true;
                            break;
                        case -41:
                            // "Is profiling in progress";
                            break;
                        case -40:
                            if (conditionData.Parameters[0].Data is ParameterInt)
                            {
                                switch (((ParameterInt)conditionData.Parameters[0].Data).Value)
                                {
                                    case 0:
                                        // "Is running as Windows application";
                                        break;
                                    case 1:
                                        // "Is running as Mac application";
                                        break;
                                    case 2:
                                        // "Is running as SWF / Flash Player application";
                                        break;
                                    case 3:
                                        // "Is running as Android application";
                                        break;
                                    case 4:
                                        // "Is running as iOS application";
                                        break;
                                    case 5:
                                        // "Is running as Html5 application";
                                        break;
                                    case 6:
                                        // "Is running as XNA application";
                                        break;
                                    case 7:
                                        // "Is running as UWP application";
                                        break;
                                }
                            }
                            // "Is running as {Parameters[0]}";
                            break;
                        case -26:
                            // "{Parameters[0]} chances out of {Parameters[1]} at random";
                            break;
                        case -25:
                            // "OR (logical)";
                            break;
                        case -24:
                            // "OR";
                            break;
                        case -22:
                            // "Text is available in clipboard";
                            break;
                        case -21:
                            // "Close window has been selected";
                            break;
                        case -20:
                            // "Menu bar is visible";
                            break;
                        case -19:
                            // "Menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)} is enabled";
                            break;
                        case -18:
                            // "Menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)} is enabled";
                            break;
                        case -17:
                            // "Menu option {GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)} is checked";
                            break;
                        case -16:
                            // "On loop {Parameters[0]}";
                            break;
                        case -15:
                            // "Files have been dropped";
                            break;
                        case -14:
                            // "Menu option \"{GetMenuItemName(((ParameterInt)Parameters[0].Data).Value)}\" selected";
                            break;
                        case -8:
                            // "{GetGlobalValueName(Parameters[0].Data)} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
                            break;
                        case -7:
                            output.type.value = "BuiltinCommonInstructions::Once";
                            break;
                        case -6:
                            // "Run this event once";
                            break;
                        case -5:
                            // "Repeat {Parameters[0]} times";
                            break;
                        case -4:
                            // "Restrict actions for {Parameters[0]}";
                            break;
                        case -3:
                            {
                                string exp1 = Translate((ParameterExpressions)conditionData.Parameters[0].Data, out bool isString);
                                string exp2 = Translate((ParameterExpressions)conditionData.Parameters[1].Data, out isString);
                                output.type.value = $"BuiltinCommonInstructions::{(isString ? "CompareStrings" : "CompareNumbers")}";
                                parameters.Add(exp1);
                                parameters.Add(GetComparison(((ParameterExpressions)conditionData.Parameters[1].Data).Comparison));
                                parameters.Add(exp2);
                            }
                            break;
                        case -2:
                            output.type.value = "BuiltinCommonInstructions::Always";
                            forceNegated = true;
                            break;
                        case -1:
                            output.type.value = "BuiltinCommonInstructions::Always";
                            break;
                    }
                    break;
            }
            output.type.inverted = conditionData.OtherFlags["Negated"];
            if (forceNegated)
                output.type.inverted = !output.type.inverted;
            output.parameters = parameters.ToArray();
            return string.IsNullOrEmpty(output.type.value) ? null : output;
        }

        public static string Translate(ParameterExpressions expressions, out bool isString)
        {
            return GetExpression(expressions, 0, 0, new() { 0 }, out isString, out int seekedCount);
        }

        public static string GetExpression(ParameterExpressions expressions, int startPos, int startLevel, List<int> levelBase, out bool isString, out int seekedCount)
        {
            string output = string.Empty;
            isString = false;
            int level = startLevel;
            Dictionary<int, string> doAppend = new();

            for (int i = startPos; i < expressions.Expressions.Count; i++)
            {
                ParameterExpression expression = expressions.Expressions[i];
                switch (expression.ObjectType)
                {
                    case -4:
                        switch (expression.Num)
                        {
                            case 0:
                                output += "round(TimeFromStart() * 1000)";
                                break;
                        }
                        break;
                    case -1:
                        switch (expression.Num)
                        {
                            case -3:
                                output += ", ";
                                break;
                            case -2:
                                if (doAppend.ContainsKey(level))
                                {
                                    output += doAppend;
                                    doAppend.Remove(level);
                                }
                                output += ")";
                                if (levelBase.Contains(level))
                                    levelBase.Remove(level);
                                level--;
                                if (level < startLevel)
                                {
                                    seekedCount = i - startPos;
                                    return output;
                                }
                                break;
                            case -1:
                                output += "(";
                                level++;
                                levelBase.Add(level);
                                break;
                            case 0:
                                output += ((ExpressionInt)expression.Expression).Value.ToString();
                                break;
                            case 1:
                                output += "Random(";
                                level++;
                                break;
                            case 3:
                                output += '"' + ((ExpressionString)expression.Expression).Value + '"';
                                if (levelBase.Contains(level))
                                    isString = true;
                                break;
                            case 4:
                                output += "ToString(";
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                            case 5:
                                output += "ToNumber(";
                                level++;
                                break;
                            case 6:
                                output += "SubStr(FileSystem::ExecutableFolderPath(), 0, 2)";
                                if (levelBase.Contains(level))
                                    isString = true;
                                break;
                            case 7:
                                output += "SubStr(FileSystem::ExecutableFolderPath(), 2, StrLength(FileSystem::ExecutableFolderPath()) - 2) + FileSystem::PathDelimiter()";
                                if (levelBase.Contains(level))
                                    isString = true;
                                break;
                            case 8:
                                output += "FileSystem::ExecutableFolderPath() + FileSystem::PathDelimiter()";
                                if (levelBase.Contains(level))
                                    isString = true;
                                break;
                            case 9:
                                output += "SubStr(FileSystem::FileName(FileSystem::ExecutablePath()), 0, StrLength(FileSystem::FileName(FileSystem::ExecutablePath())) - StrLength(FileSystem::ExtensionName(FileSystem::ExecutablePath())))";
                                if (levelBase.Contains(level))
                                    isString = true;
                                break;
                            case 10:
                                output += "sin(";
                                doAppend.Add(level, " / 57.2957795");
                                level++;
                                break;
                            case 11:
                                output += "cos(";
                                doAppend.Add(level, " / 57.2957795");
                                level++;
                                break;
                            case 12:
                                output += "tan(";
                                doAppend.Add(level, " / 57.2957795");
                                level++;
                                break;
                            case 13:
                                output += "sqrt(";
                                level++;
                                break;
                            case 14:
                                output += "log10(";
                                level++;
                                break;
                            case 15:
                                output += "log(";
                                level++;
                                break;
                            case 16:
                                output += "\"0x\" + ToUpperCase(BaseConversion::ToBase(123, 16))";
                                GDWriter.extensions.Add("BaseConversion");
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                            case 17:
                                output += "\"0b\" + BaseConversion::ToBase(123, 2)";
                                GDWriter.extensions.Add("BaseConversion");
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                            case 18:
                                output += "exp(";
                                level++;
                                break;
                            case 19:
                                bool leftIsString = false;
                                int leftSeekedCount = 0;
                                List<string> leftParams = GetExpression(expressions, i + 1, level + 1, levelBase, out leftIsString, out leftSeekedCount).TrimEnd(')').Split(", ").ToList();
                                i += leftSeekedCount;
                                isString |= leftIsString;
                                leftParams.Insert(1, "0");
                                output += $"SubStr({string.Join(", ", leftParams)})";
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                            case 20:
                                bool rightIsString = false;
                                int rightSeekedCount = 0;
                                List<string> rightParams = GetExpression(expressions, i + 1, level + 1, levelBase, out rightIsString, out rightSeekedCount).TrimEnd(')').Split(", ").ToList();
                                i += rightSeekedCount;
                                isString |= rightIsString;
                                rightParams.Insert(1, $"StrLength({rightParams[0]}) - {rightParams[1]}");
                                output += $"SubStr({string.Join(", ", rightParams)})";
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                            case 21:
                                output += "SubStr(";
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                            case 22:
                                output += "StrLength(";
                                level++;
                                break;
                            case 23:
                                output += ((ExpressionDouble)expression.Expression).Value.ToString();
                                break;
                            case 28:
                                output += "floor(";
                                level++;
                                break;
                            case 29:
                                output += "abs(";
                                level++;
                                break;
                            case 30:
                                output += "ceil(";
                                level++;
                                break;
                            case 31:
                                output += "floor(";
                                level++;
                                break;
                            case 32:
                                bool acosIsString = false;
                                int acosSeekedCount = 0;
                                string acosParams = GetExpression(expressions, i + 1, level + 1, levelBase, out acosIsString, out acosSeekedCount);
                                i += acosSeekedCount;
                                isString |= acosIsString;
                                output += $"acos({string.Join(", ", acosParams)} * 57.2957795";
                                level++;
                                break;
                            case 33:
                                bool asinIsString = false;
                                int asinSeekedCount = 0;
                                string asinParams = GetExpression(expressions, i + 1, level + 1, levelBase, out asinIsString, out asinSeekedCount);
                                i += asinSeekedCount;
                                isString |= asinIsString;
                                output += $"asin({string.Join(", ", asinParams)} * 57.2957795";
                                level++;
                                break;
                            case 34:
                                bool atanIsString = false;
                                int atanSeekedCount = 0;
                                string atanParams = GetExpression(expressions, i + 1, level + 1, levelBase, out atanIsString, out atanSeekedCount);
                                i += atanSeekedCount;
                                isString |= atanIsString;
                                output += $"atan({string.Join(", ", atanParams)} * 57.2957795";
                                level++;
                                break;
                            case 35:
                                bool notIsString = false;
                                int notSeekedCount = 0;
                                string notParams = GetExpression(expressions, i + 1, level + 1, levelBase, out notIsString, out notSeekedCount);
                                i += notSeekedCount;
                                isString |= notIsString;
                                output += $"floor(({string.Join(", ", notParams)} * -1 - 1";
                                level++;
                                break;
                            case 36:
                                // "NDropped";
                                break;
                            case 37:
                                // "Dropped$(";
                                if (levelBase.Contains(level))
                                    isString = true;
                                level++;
                                break;
                        }
                        break;
                    case 0:
                        switch (expression.Num)
                        {
                            case 2:
                                // " + ";
                                break;
                            case 4:
                                // " - ";
                                break;
                            case 6:
                                // " * ";
                                break;
                            case 8:
                                // " / ";
                                break;
                            case 10:
                                // " mod ";
                                break;
                            case 12:
                                // " pow ";
                                break;
                            case 14:
                                // " and ";
                                break;
                            case 16:
                                // " or ";
                                break;
                            case 18:
                                // " xor ";
                                break;
                        }
                        break;
                }
            }
            seekedCount = expressions.Expressions.Count - startPos;
            return output;
        }

        public static string GetComparison(int comparison)
        {
            return comparison switch
            {
                0 => "=",
                1 => "!=",
                2 => "<=",
                3 => "<",
                4 => ">=",
                5 => ">",
                _ => "="
            };
        }
    }
}
