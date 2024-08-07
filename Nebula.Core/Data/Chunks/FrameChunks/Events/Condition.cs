using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public class Condition : ACEventBase
    {
        public short Identifier;

        public Condition()
        {
            ChunkName = "Condition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadUShort());

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
            Identifier = reader.ReadShort();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
                Parameters[i].FrameEvents = Parent.Parent;
            }

            reader.Seek(endPosition);
            Fix((List<Condition>)extraInfo[0]);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + Math.Abs(reader.ReadUShort());

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadUShort();
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

            ByteWriter condWriter = new ByteWriter(new MemoryStream());
            condWriter.WriteShort(ObjectType);
            condWriter.WriteShort(Num);
            condWriter.WriteUShort(ObjectInfo);
            condWriter.WriteShort(ObjectInfoList);
            condWriter.WriteByte((byte)EventFlags.Value);
            condWriter.WriteByte((byte)OtherFlags.Value);
            condWriter.WriteByte((byte)Parameters.Length);
            condWriter.WriteByte(DefType);
            condWriter.WriteShort(Identifier);

            long startPosition = (long)extraInfo[0] + writer.Tell() + condWriter.Tell() + 4;
            foreach (Parameter parameter in Parameters)
                parameter.WriteMFA(condWriter, startPosition);

            writer.WriteUShort((ushort)(condWriter.Tell() + 2));
            writer.WriteWriter(condWriter);
            condWriter.Flush();
            condWriter.Close();
        }

        private void Fix(List<Condition> evntList)
        {
            short oldNum = Num;
            bool ignoreOptimization = false;

            if (NebulaCore.Fusion == 1.5f && ObjectType > 1 && Num <= -48)
            {
                // MMF1.5 EXTBASE = 48
                // MMF2   EXTBASE = 80
                Num -= 80 - 48;
                ignoreOptimization = true;
            }

            switch (ObjectType)
            {
                case -1:
                    switch (Num)
                    {
                        case 0:
                            DoAdd = false;
                            ignoreOptimization = true;
                            break;
                        case -28: // Compare Global Integer Equals
                        case -29: // Compare Global Integer Doesnt Equal
                        case -30: // Compare Global Integer Less Or Equal
                        case -31: // Compare Global Integer Less
                        case -32: // Compare Global Integer Greater Or Equal
                        case -33: // Compare Global Integer Greater
                        case -34: // Compare Global Double Equals
                        case -35: // Compare Global Double Doesnt Equal
                        case -36: // Compare Global Double Less Or Equal
                        case -37: // Compare Global Double Less
                        case -38: // Compare Global Double Greater Or Equal
                        case -39: // Compare Global Double Greater
                            Num = -8; // Compare Global
                            ignoreOptimization = true;
                            break;
                        case -42: // Idk, just deleting it
                        case -43: // Start Child Event
                            DoAdd = false;
                            ignoreOptimization = true;
                            break;
                    }
                    break;
                case >= 0:
                    switch (Num)
                    {
                        case -42: // Compare Alterable Integer
                        case -43: // Compare Alterable Double
                            Num = -27; // Compare Alterable
                            ignoreOptimization = true;
                            break;
                    }
                    break;
            }


            for (int i = 0; i < Parameters.Length; i++)
            {
                if (Parameters[i].Code == 68)
                {
                    ParameterVariables varsData = (ParameterVariables)Parameters[i].Data;

                    DoAdd = false;
                    if (Parameters.Length > 1)
                    {
                        List<Parameter> param = Parameters.ToList();
                        param.RemoveAt(i);
                        Parameters = param.ToArray();
                        evntList.Add(this);
                    }

                    foreach (ParameterVariable varData in varsData.Variables)
                    {
                        Condition varCond = Copy();
                        if (varData.Global)
                        {
                            varCond.ObjectInfo = 0;
                            varCond.ObjectType = -1;
                            varCond.Num = -8;
                        }
                        else
                            varCond.Num = -27;
                        varCond.EventFlags["Always"] = true;
                        ParameterInt varParamIndexVal = new ParameterInt();
                        varParamIndexVal.Value = varData.Index;
                        Parameter varParamIndex = new Parameter();
                        varParamIndex.Data = varParamIndexVal;
                        varParamIndex.Code = varData.Global ? 49 : 50;
                        ExpressionChunk varExpVal;
                        if (varData.Value is double)
                            varExpVal = new ExpressionDouble()
                            {
                                Value = (double)varData.Value,
                                Value2 = (float)(double)varData.Value
                            };
                        else
                            varExpVal = new ExpressionInt()
                            {
                                Value = (int)varData.Value
                            };
                        ParameterExpression varExp = new ParameterExpression();
                        varExp.Expression = varExpVal;
                        varExp.ObjectType = -1;
                        varExp.Num = (short)(varExpVal is ExpressionDouble ? 23 : 0);
                        ParameterExpressions varExps = new ParameterExpressions();
                        varExps.Comparison = (short)varData.Operator;
                        varExps.Expressions.Add(varExp);
                        Parameter varParamVal = new Parameter();
                        varParamVal.Data = varExps;
                        varParamVal.Code = 23;
                        varCond.Parameters = new Parameter[2];
                        varCond.Parameters[0] = varParamIndex;
                        varCond.Parameters[1] = varParamVal;
                        evntList.Add(varCond);
                    }

                    for (int flag = 0; flag < 32; flag++)
                    {
                        if (varsData.FlagMasks != flag)
                            continue;
                        bool flagValue = varsData.FlagValues == flag;
                        Condition flagCond = Copy();
                        flagCond.Num = (short)(flagValue ? -25 : -24);
                        flagCond.EventFlags["Always"] = true;
                        ExpressionInt flagExpInt = new ExpressionInt();
                        flagExpInt.Value = flag;
                        ParameterExpression flagExp = new ParameterExpression();
                        flagExp.Expression = flagExpInt;
                        flagExp.ObjectType = -1;
                        ParameterExpressions flagExps = new ParameterExpressions();
                        flagExps.Expressions.Add(flagExp);
                        Parameter flagParam = new Parameter();
                        flagParam.Data = flagExps;
                        flagParam.Code = 22;
                        flagCond.Parameters = new Parameter[1];
                        flagCond.Parameters[0] = flagParam;
                        evntList.Add(flagCond);
                    }
                }
            }

            FrameEvents.OptimizedEvents |= ((Num != oldNum) || (DoAdd == false)) && !ignoreOptimization;
        }

        public override string ToString()
        {
            return (OtherFlags["Negated"] ? "NOT " : "") + GetString();
        }

        public string GetString()
        {
            switch (ObjectType)
            {
                default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                case -7:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
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
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
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
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case -23:
                            return $"Pick all objects in line ({Parameters[0]},{Parameters[1]}) to ({Parameters[2]},{Parameters[3]})";
                        case -22:
                            return $"Pick objects with Flag {Parameters[0]} off";
                        case -21:
                            return $"Pick objects with Flag {Parameters[0]} on";
                        case -20:
                            return $"Pick objects which {GetAlterableValueName(Parameters[0].Data)} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
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
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
                        case -8:
                            return $"Every {Parameters[0]}";
                        case -7:
                            return $"Timer equals {Parameters[0]}";
                        case -6:
                            return $"On timer event {Parameters[0]}";
                        case -5:
                            return $"User has left the computer for {Parameters[0]}";
                        case -4:
                            return $"Every {Parameters[0]}";
                        case -2:
                            return $"Timer is less than {Parameters[0]}";
                        case -1:
                            return $"Timer is greater than {Parameters[0]}";
                    }
                case -3:
                    switch (Num)
                    {
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
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
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
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
                        default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
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
                        case -23:
                            return "On group activation";
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
                        case -12:
                            return $"Group \"{Parameters[0]}\" is activated";
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
                                    default: return $"[ERROR] Could not find ObjectType {ObjectType}, Num {Num}";
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
                            return $"{GetObjectAlterableStringName(Parameters[0].Data)} of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
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
                            return $"{GetObjectAlterableValueName(Parameters[0].Data)} of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[1].Data).Comparison)} {Parameters[1]}";
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
                            if (Parameters[0].Data is ParameterShort)
                                return $"{GetObjectName()} animation {GetObjectAnimation(((ParameterShort)Parameters[0].Data).Value)} is playing";
                            return $"{GetObjectName()} animation {Parameters[0].Data} is playing";
                        case -2:
                            if (Parameters[0].Data is ParameterShort)
                                return $"{GetObjectName()} animation {GetObjectAnimation(((ParameterShort)Parameters[0].Data).Value)} is over";
                            return $"{GetObjectName()} animation {Parameters[0].Data} is over";
                        case -1:
                            return $"Current frame of {GetObjectName()} {GetComparison(((ParameterExpressions)Parameters[0].Data).Comparison)} {Parameters[0]}";
                    }
            }
        }

        public Condition Copy()
        {
            Condition cnd = new Condition();
            cnd.ObjectType = ObjectType;
            cnd.Num = Num;
            cnd.ObjectInfo = ObjectInfo;
            cnd.ObjectInfoList = ObjectInfoList;
            cnd.EventFlags.Value = EventFlags.Value;
            cnd.OtherFlags.Value = OtherFlags.Value;
            cnd.DefType = DefType;
            cnd.Identifier = Identifier;
            cnd.Parent = Parent;
            cnd.DoAdd = DoAdd;
            return cnd;
        }
    }
}
