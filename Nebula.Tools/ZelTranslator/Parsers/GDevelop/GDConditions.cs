using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;

namespace Nebula.Tools.ZelTranslator_SD.GDevelop
{
    public class GDConditions
    {
        public static GDJSON.Condition CompareAltVal(Condition condition, PackageData gameData)
        {
            var Parameter1 = condition.Parameters[0].Data as ParameterShort;
            var Parameter2 = condition.Parameters[1].Data as ParameterExpressions;

            var newCondition = new GDJSON.Condition();
            newCondition.type.value = "VarObjet";

            string ObjectName = $"Qualifier {condition.ObjectInfo}";
            if (condition.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[condition.ObjectInfo].Name;

            string ComparisonStr;
            switch (Parameter2.Comparison)
            {
                default:
                case 0:
                    ComparisonStr = "=";
                    break;
                case 1:
                    ComparisonStr = "!=";
                    break;
                case 2:
                    ComparisonStr = "<=";
                    break;
                case 3:
                    ComparisonStr = "<";
                    break;
                case 4:
                    ComparisonStr = ">=";
                    break;
                case 5:
                    ComparisonStr = ">";
                    break;
            }

            var Parameters = new List<string>
            {
                ObjectName, //Object Name
                "AlterableValue" + GDWriter.AltCharacter(Parameter1.Value), //Alterable Value
                ComparisonStr, //Comparison
                ((int)(Parameter2.Expressions[0].Expression as ExpressionInt).Value).ToString() //Compare To
            };

            newCondition.parameters = Parameters.ToArray();
            return newCondition;
        }

        public static GDJSON.Condition DefaultType(Condition condition, PackageData gameData, bool Object, bool Inverted, string Type)
        {
            var a = ((32767 - 14000) / 18767) * 800;
            var newCondition = new GDJSON.Condition();
            newCondition.type.value = Type;
            newCondition.type.inverted = Inverted;

            string ObjectName = $"Qualifier {condition.ObjectInfo}";
            if (condition.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[condition.ObjectInfo].Name;

            var Parameters = new List<string>();
            if (Object) Parameters.Add(ObjectName);
            else Parameters.Add("");
            Parameters.Clear();

            newCondition.parameters = Parameters.ToArray();
            return newCondition;
        }

        public static GDJSON.Condition KeyPressed(Condition condition, PackageData gameData)
        {
            var Parameter1 = condition.Parameters[0].Data as ParameterShort;

            var newCondition = new GDJSON.Condition();
            newCondition.type.value = "KeyPressed";

            var Parameters = new List<string>
            {
                "",
                GDKeyCodes.ShortKeyCodes[Parameter1.Value],
            };

            newCondition.parameters = Parameters.ToArray();
            return newCondition;
        }

        public static GDJSON.Condition EveryTimer(Condition condition, PackageData gameData, int index)
        {
            var Parameter1 = condition.Parameters[0].Data as ParameterTimer;

            var newCondition = new GDJSON.Condition();
            newCondition.type.value = "RepeatEveryXSeconds::Repeat";

            var Parameters = new List<string>
            {
                "", //?
                $"\"{index}\"",
                (Parameter1.Timer / 1000.0).ToString(),
                ""
            };

            newCondition.parameters = Parameters.ToArray();
            return newCondition;
        }
    }
}
