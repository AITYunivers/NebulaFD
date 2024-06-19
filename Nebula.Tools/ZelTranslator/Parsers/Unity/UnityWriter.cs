using Nebula.Core.Utilities;
using ZelTranslator_SD.Parsers.Unity.ProjectFiles;

namespace Nebula.Tools.ZelTranslator_SD.Unity
{
    public class UnityWriter //: INebulaTool // Comment ': INebulaTool' out to disable
    {
        public string Name => "Unity Translator";
        public void Execute()
        {
            Project unityProject = Project.CreateNew();

            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\UnityProject\\";
            Directory.CreateDirectory(path);

            unityProject.Write(path);
        }
    }
}