using Ionic.BZip2;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Security.AccessControl;
using System.Reflection;
using System.Linq.Expressions;
using Action = Nebula.Core.Data.Chunks.FrameChunks.Events.Action;
using static ZelTranslator_SD.Parsers.GMS2Writer;
using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Data.Chunks.FrameChunks.Events;

namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class GMS2Actions
    {
        public static string object_name(Action action, PackageData gameData)
        {
            string ObjName = "ZEL_UNKNOWN_OBJECT";
            var Object = gameData.FrameItems.Items[action.ObjectInfo];
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjName = CleanString(Object.Name).Replace(" ", "_") + "_" + Object.Header.Handle;
                ObjName = $"object({GMS2Writer.ObjectName(Object)})";

            return ObjName;
        }

        public static string SetPosition(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            if (action.Num == 1) // Set Position at (X,X)
            {
                var Object = action.Parameters[0].Data as ParameterPosition;

                if (Object.TypeParent == 2) // Set Position relative to an object
                {
                    var parentObj = CleanString(gameData.FrameItems.Items[Object.ObjectInfoParent].Name).Replace(" ", "_") + "_" + gameData.FrameItems.Items[Object.ObjectInfoParent].Header.Handle;
                    parentObj = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[Object.ObjectInfoParent])})";
                    return $"SetPosition({ObjectName}, {Object.X}, {Object.Y}, {evnt}, 2, {parentObj});";
                }
                else // Set Position to exact X and Y
                {
                    return $"SetPosition({ObjectName}, {Object.X}, {Object.Y}, {evnt});";
                }
            }
            else
            {
                string xy = "X"; // Set X Position
                if (action.Num == 3) xy = "Y"; // Set Y Position
                return "Set" + xy + $"Pos({ObjectName}, {GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evnt, gameData, extCodes, ref missing_code)}, {evnt});";
            }
        }
        public static string CenterDisplay(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code) // (FIX/REWORK to work with EACH instance)
        {
            if (action.Num == 7) // Center Display at Object Position (FIX/REWORK to work with EACH instance)
            {
                var Object = action.Parameters[0].Data as ParameterPosition;
                if (Object.TypeParent == 2) // Set Position relative to an object
                {
                    var parentObj = CleanString(gameData.FrameItems.Items[Object.ObjectInfoParent].Name).Replace(" ", "_") + "_" + gameData.FrameItems.Items[Object.ObjectInfoParent].Header.Handle;
                    parentObj = $"object({GMS2Writer.ObjectName(gameData.FrameItems.Items[Object.ObjectInfoParent])})";
                    return $"frameCamera.centerX = ({parentObj}.x + {Object.X});\n\tframeCamera.centerY = ({parentObj}.y + {Object.Y});";
                }
                else // Set Position to exact X and Y
                {
                    return $"frameCamera.centerX = {Object.X};\n\tframeCamera.centerY = {Object.Y};";
                }
            }
            else if (action.Num == 8) // Center Display at X=
            {
                var centerX = GMS2Expressions.Evaluate(action.Parameters[0].Data as ParameterExpressions, evnt, gameData, extCodes, ref missing_code);
                return $"frameCamera.centerX = {centerX};";
            }


            return "";
        }
        public static string BringToFront(Action action, int evnt, PackageData gameData)
        {
            // (FIX/REWORK this might not work like how I think it does...)
            string ObjectName = object_name(action, gameData);
            return $"SetGeneralValue({ObjectName}, \"depth\", \"=\", layer_get_depth(GetGeneralValue({ObjectName},\"layer\",\"str\",{evnt})), {evnt}); // Bring to front";
        }
        public static string SetVisibility(Action action, int evnt, PackageData gameData)
        {
            string ObjectName = object_name(action, gameData);
            string visible = "false";
            if (action.Num == 27) visible = "true";
            return $"SetVisibility({ObjectName}, {visible}, {evnt});";
        }
        public static string FlashObject(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            string flash = "0";
            if (action.Parameters[0].Data is ParameterTimer timer) flash = timer.Timer.ToString();
            else if (action.Parameters[0].Data is ParameterExpressions expParam) flash = GMS2Expressions.Evaluate(expParam, evnt, gameData, extCodes, ref missing_code);
            return $"FlashObject({ObjectName}, {flash}, {evnt});";
        }
        public static string DestroyObject(Action action, int evnt, PackageData gameData)
        {
            string ObjectName = object_name(action, gameData);
            return $"DestroyObject({ObjectName}, {evnt});";
        }
        public static string SetGlobalValue(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string operation = "=";
            if (action.Num == 4 || action.Num == 5) operation = (action.Num == 4 ? "-=" : "+=");
            string global = "";
            if (action.Parameters[0].Data is ParameterInt prmInt) global = prmInt.Value.ToString();
            if (action.Parameters[0].Data is ParameterShort prmShrt) global = prmShrt.Value.ToString();
            if (action.Parameters[0].Data is ParameterExpressions prmExp) global = GMS2Expressions.Evaluate(prmExp, evnt, gameData, extCodes, ref missing_code);
            string value = GMS2Expressions.Evaluate(action.Parameters[1].Data as ParameterExpressions, evnt, gameData, extCodes, ref missing_code);
            return $"global.Global{(action.Num == 3 ? "Values" : "Strings")}[{global}] {operation} {value}";
        }
        public static string SetAltValue(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            string setAltValue = "";
            if (action.Parameters[0].Data is ParameterShort prmShrt) setAltValue = prmShrt.Value.ToString();
            else if (action.Parameters[0].Data is ParameterExpressions prmExps) setAltValue = GMS2Expressions.Evaluate(prmExps, evnt, gameData, extCodes, ref missing_code);
            var setExpression = action.Parameters[1].Data as ParameterExpressions;
            //var setValue = (setExpression.Items[0].Data as LongExp).Value;
            string operation = action.Num switch { 31 => "=", 32 => "+=", 33 => "-=" };
            return $"SetAltValue({ObjectName}, {setAltValue}, \"{operation}\", {GMS2Expressions.Evaluate(setExpression, evnt, gameData, extCodes, ref missing_code)}, {evnt});";
        }
        public static string SetAltString(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            string setAltString = "";
            if (action.Parameters[0].Data is ParameterShort prmShrt) setAltString = prmShrt.Value.ToString();
            else if (action.Parameters[0].Data is ParameterExpressions prmExps) setAltString = GMS2Expressions.Evaluate(prmExps, evnt, gameData, extCodes, ref missing_code);
            var setExpression = action.Parameters[1].Data as ParameterExpressions;
            return $"SetAltString({ObjectName}, {setAltString}, {GMS2Expressions.Evaluate(setExpression, evnt, gameData, extCodes, ref missing_code)}, {evnt});";
        }
        public static string SetFlag(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            string val = action.Num switch { 35 => "true", 36 => "false", 37 => "\"toggle\"" };
            var flagExp = action.Parameters[0].Data as ParameterExpressions;
            return $"SetFlag({ObjectName}, {flagExp}, {val}, {evnt});";
        }
        public static string SetAlpha(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            var setExpression = action.Parameters[0].Data as ParameterExpressions;
            string operation = "AlphaCoef";
            if (action.Num == 39) operation = "SemiTrans";
            return $"Set{operation}({ObjectName}, {GMS2Expressions.Evaluate(setExpression, evnt, gameData, extCodes, ref missing_code)}, {evnt});";
        }
        public static string SetParagraph(Action action, int evnt, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            string ObjectName = object_name(action, gameData);
            
            string setVal = "-1";
            if (action.Num == 84)
            {
                if (action.Parameters[0].Data is ParameterExpressions exp) setVal = GMS2Expressions.Evaluate(exp, evnt, gameData, extCodes, ref missing_code);
                else if (action.Parameters[0].Data is ParameterShort shrt) setVal = shrt.Value.ToString();
            }
            
            string prevNext = action.Num switch { 84 => "", 85 => ", \"prev\"", 86 => ", \"next\"" };
            return $"SetParagraph({ObjectName}, {setVal}, {evnt}{prevNext});";
        }
    }
}