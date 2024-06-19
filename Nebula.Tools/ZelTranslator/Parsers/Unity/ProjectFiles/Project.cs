using Nebula;
using Nebula.Core.Data.Chunks.FrameChunks;
using ZelTranslator_SD.Parsers.Unity.ProjectFiles.AssetFiles;

namespace ZelTranslator_SD.Parsers.Unity.ProjectFiles
{
    public class Project
    {
        public List<IAssetFile> Assets = new();
        public ProjectSettings Settings = new ProjectSettings();

        public static Project CreateNew()
        {
            Project project = new Project();

            foreach (Frame frm in NebulaCore.PackageData.Frames)
            {
                AssetScene newScene = new(frm);
                project.Assets.Add(newScene);
            }

            return project;
        }

        public void Write(string projectPath)
        {
            Assets.ForEach(x => x.Write(projectPath));
            Settings.Write(projectPath);
        }
    }
}
