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
using Condition = Nebula.Core.Data.Chunks.FrameChunks.Events.Condition;
using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using static ZelTranslator_SD.Parsers.GameMakerStudio2.EventsToGML;

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
        public static string OverlapObject(Condition cond, int evntCount, PackageData gameData)
        {
            string returnString = "";
            var overlapObj1 = gameData.FrameItems.Items[cond.ObjectInfo];
            var overlapObj2 = gameData.FrameItems.Items[(cond.Parameters[0].Data as ParameterObject).ObjectInfo];
            var overlapObj1Name = GMS2Writer.CleanString(overlapObj1.Name).Replace(" ", "_") + "_" + overlapObj1.Header.Handle;
            overlapObj1Name = $"object({GMS2Writer.ObjectName(overlapObj1)})";
            var overlapObj2Name = GMS2Writer.CleanString(overlapObj2.Name).Replace(" ", "_") + "_" + overlapObj2.Header.Handle;
            overlapObj2Name = $"object({GMS2Writer.ObjectName(overlapObj2)})";
            //if (cond.OtherFlags == 1) returnString += "!";
            returnString += $"IsOverlapping({overlapObj1Name}, {overlapObj2Name}, {evntCount})/*Flags:{cond.EventFlags}OtherFlags:{cond.OtherFlags}DefType:{cond.DefType}*/";
            return returnString;
        }
        public static string CompareXorYPos(Condition cond, int evntCount, PackageData gameData, Dictionary<string, int> extCodes, ref List<string> missing_code)
        {
            var obj = gameData.FrameItems.Items[cond.ObjectInfo];
            var objName = GMS2Writer.CleanString(obj.Name).Replace(" ", "_") + "_" + obj.Header.Handle;
            objName = $"object({GMS2Writer.ObjectName(obj)})";
            var expression = cond.Parameters[0].Data as ParameterExpressions;
            string XorY = "x";
            if (cond.Num == -16) XorY = "y";
            return $"CompareXorYPos({objName}, \"{XorY}\", \"{GetOperator(expression.Comparison)}\", {GMS2Expressions.Evaluate(expression, evntCount, gameData, extCodes, ref missing_code)}, {evntCount})";
        }
        public static string IsVisible(Condition cond, int evntCount, PackageData gameData)
        {
            var obj = gameData.FrameItems.Items[cond.ObjectInfo];
            var objName = $"object({GMS2Writer.ObjectName(obj)})";
            var prefix = "";
            if (cond.Num == -28) prefix = "!";
            return $"{prefix}IsVisible({objName}, {evntCount})";
        }
    }
}