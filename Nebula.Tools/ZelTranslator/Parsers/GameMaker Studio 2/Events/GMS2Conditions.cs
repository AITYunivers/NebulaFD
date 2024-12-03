using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using static ZelTranslator_SD.Parsers.GameMakerStudio2.EventsToGML;
using Condition = Nebula.Core.Data.Chunks.FrameChunks.Events.Condition;

namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class GMS2Conditions
    {
        public static string Storyboard()
        {
            return "";
        }
        public static string CreateObject()
        {
            return "";
        }
        public static string OverlapBackdrop(Condition cond, int evntCount, PackageData gameData)
        {
            string returnString = "";
            var overlapObj = gameData.FrameItems.Items[cond.ObjectInfo];
            var overlapObjName = GMS2Writer.CleanString(overlapObj.Name).Replace(" ", "_") + "_" + overlapObj.Header.Handle;
            overlapObjName = $"object({GMS2Writer.ObjectName(overlapObj)})";
            //if (cond.OtherFlags == 1) returnString += "!";
            returnString += $"IsOverlappingBackdrop({overlapObjName}, {evntCount})/*Flags:{cond.EventFlags}OtherFlags:{cond.OtherFlags}DefType:{cond.DefType}*/";
            return returnString;
        }
        public static string OverlapObject(string overlapObjName, Condition cond, int evntCount, PackageData gameData)
        {
            string returnString = "";
            var overlapObj2 = gameData.FrameItems.Items[(cond.Parameters[0].Data as ParameterObject).ObjectInfo];
            var overlapObj2Name = $"object({GMS2Writer.ObjectName(overlapObj2)})";
            //if (cond.OtherFlags == 1) returnString += "!";
            returnString += $"IsOverlapping({overlapObjName}, {overlapObj2Name}, {evntCount})/*Flags:{cond.EventFlags}OtherFlags:{cond.OtherFlags}DefType:{cond.DefType}*/";
            return returnString;
        }
        public static string CompareXorYPos(string objName, Condition cond, int evntCount, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            var expression = cond.Parameters[0].Data as ParameterExpressions;
            string XorY = "x";
            if (cond.Num == -16) XorY = "y";
            return $"CompareXorYPos({objName}, \"{XorY}\", \"{GetOperator(expression.Comparison)}\", {GMS2Expressions.Evaluate(expression, evntCount, gameData, extCodes, ref missing_code)}, {evntCount})";
        }
        public static string IsVisible(string objName, Condition cond, int evntCount, PackageData gameData)
        {
            var prefix = "";
            if (cond.Num == -28) prefix = "!";
            return $"{prefix}IsVisible({objName}, {evntCount})";
        }
    }
}