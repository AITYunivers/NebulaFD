using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Action = Nebula.Core.Data.Chunks.FrameChunks.Events.Action;

namespace Nebula.Tools.ZelTranslator_SD.GDevelop
{
    public class GDActions
    {
        public static GDJSON.Action SetAltVal(Action action, PackageData gameData, string Modify)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterShort;
            var Parameter2 = action.Parameters[1].Data as ParameterExpressions;

            var newAction = new GDJSON.Action();
            newAction.type.value = "ModVarObjet";

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>
            {
                ObjectName, //Object Name
                "AlterableValue" + GDWriter.AltCharacter(Parameter1.Value), //Alterable Value
                Modify, //Modifier
                ((int)(Parameter2.Expressions[0].Expression as ExpressionInt).Value).ToString() //To Modify
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action SetXorY(Action action, PackageData gameData, string XorY)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterExpressions;

            var newAction = new GDJSON.Action();
            newAction.type.value = "Mettre" + XorY;

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>
            {
                ObjectName, //Object Name
                "=", //Modifier
                ((int)(Parameter1.Expressions[0].Expression as ExpressionInt).Value).ToString() //To Modify
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action SetAnimation(Action action, PackageData gameData)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterShort;

            var newAction = new GDJSON.Action();
            newAction.type.value = "SetAnimationName";

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>
            {
                ObjectName, //Object Name
                $"\"Animation {Parameter1.Value}\"" //Animation Name
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action DefaultType(Action action, PackageData gameData, bool Object, string Type)
        {
            var newAction = new GDJSON.Action();
            newAction.type.value = Type;

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>();
            if (Object) Parameters.Add(ObjectName);
            else Parameters.Add("");

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action SetPosition(Action action, PackageData gameData)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterPosition;

            var newAction = new GDJSON.Action();
            newAction.type.value = "MettreXY";

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>
            {
                ObjectName, //Object Name
                "=", //Modifier
                Parameter1.X.ToString(), //X Position
                "=", //Modifier
                Parameter1.Y.ToString(), //Y Position
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action SetOpacity(Action action, PackageData gameData)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterExpressions;

            var newAction = new GDJSON.Action();
            newAction.type.value = "SetEffectDoubleParameter";

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>
            {
                ObjectName, //Object Name
                "\"Alpha Blending Coefficient\"", //Effect Name
                "\"opacity\"", //Effect Parameter Name
                (1.0 - (((int)(Parameter1.Expressions[0].Expression as ExpressionInt).Value) / 255.0)).ToString(), //Effect Parameter To Modify
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action PlaySoundChannel(Action action, PackageData gameData, bool Loop)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterSample;
            var Parameter2 = action.Parameters[1].Data as ParameterExpressions;

            var newAction = new GDJSON.Action();
            newAction.type.value = "PlaySoundCanal";

            foreach (Sound sound in gameData.SoundBank.Sounds.Values)
                if (sound.Name == Parameter1.Name)
                {
                    var snddata = sound.Data;
                    var sndext = ".wav";
                    if (snddata[0] == 0xff || snddata[0] == 0x49)
                        sndext = ".mp3";
                    Parameter1.Name += sndext;
                    break;
                }

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var Parameters = new List<string>
            {
                "", //?
                Parameter1.Name, //Sound File
                ((int)(Parameter2.Expressions[0].Expression as ExpressionInt).Value).ToString(), //Channel
            };

            if (Loop)
                Parameters.Add("yes");
            else
                Parameters.Add("no");

            Parameters.Add("100");
            Parameters.Add("1");

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action SetChannelVolume(Action action, PackageData gameData)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterExpressions;
            var Parameter2 = action.Parameters[1].Data as ParameterExpressions;

            var newAction = new GDJSON.Action();

            newAction.type.value = "ModVolumeSoundCanal";

            var Parameters = new List<string>
            {
                "", //?
                ((int)(Parameter1.Expressions[0].Expression as ExpressionInt).Value).ToString(), //Channel
                "=", //Modifier
                ((int)(Parameter2.Expressions[0].Expression as ExpressionInt).Value).ToString() //Modify To
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action ToFrame(Action action, PackageData gameData, int frameID = -1)
        {
            int toFrame = frameID;
            if (gameData.Frames.Count - 1 > frameID)
                toFrame += 1;

            if (frameID == -1)
            {
                var Parameter1 = action.Parameters[0].Data as ParameterShort;
                toFrame = Parameter1.Value;
            }

            var newAction = new GDJSON.Action();
            newAction.type.value = "Scene";

            var Parameters = new List<string>
            {
                "", //?
                $"\"{gameData.Frames[toFrame].FrameName}\"", //Scene Name
                "no" //Stop paused scenes
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action CreateAt(Action action, PackageData gameData, int frameID)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterCreate;

            var posx = Parameter1.X.ToString();
            var posy = Parameter1.Y.ToString();

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            if (Parameter1.ObjectInfoParent != -1)
            {
                string ObjectName2 = $"Qualifier {Parameter1.ObjectInfoParent}";
                if (Parameter1.ObjectInfoParent <= gameData.FrameItems.Items.Count)
                    ObjectName2 = gameData.FrameItems.Items[Parameter1.ObjectInfoParent].Name;

                posx = ObjectName2 + ".X() + " + posx;
                posy = ObjectName2 + ".Y() + " + posy;
            }

            var newAction = new GDJSON.Action();
            newAction.type.value = "Create";

            var Parameters = new List<string>
            {
                "", //?
                ObjectName, //Object Name
                posx, //X Position
                posy, //Y Position
                $"\"{gameData.Frames[frameID].FrameLayers.Layers[Parameter1.Layer].Name}\"", //Layer Name
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }

        public static GDJSON.Action SetCameraXorY(Action action, PackageData gameData, string xory)
        {
            var Parameter1 = action.Parameters[0].Data as ParameterExpressions;

            string ObjectName = $"Qualifier {action.ObjectInfo}";
            if (action.ObjectInfo <= gameData.FrameItems.Items.Count)
                ObjectName = gameData.FrameItems.Items[action.ObjectInfo].Name;

            var newAction = new GDJSON.Action();
            newAction.type.value = "SetCameraCenter" + xory;

            var Parameters = new List<string>
            {
                "", //?
                "=", //Modifier
                ((int)(Parameter1.Expressions[0].Expression as ExpressionInt).Value).ToString(), //Position
                "", //Layer Name
                "", //Camera ID
            };

            newAction.parameters = Parameters.ToArray();
            return newAction;
        }
    }
}
