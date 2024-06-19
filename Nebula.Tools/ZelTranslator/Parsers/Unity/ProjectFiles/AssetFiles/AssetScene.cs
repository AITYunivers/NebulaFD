using Nebula.Core.Data.Chunks.FrameChunks;
using ZelTranslator_SD.Parsers.Unity.ProjectFiles.AssetFiles.SceneComponents;

namespace ZelTranslator_SD.Parsers.Unity.ProjectFiles.AssetFiles
{
    public class AssetScene : IAssetFile
    {
        public Frame FusionFrame;
        public long NextID = 112336350;
        public List<ISceneComponent> Components = new();

        public AssetScene(Frame frame)
        {
            FusionFrame = frame;
            foreach (FrameInstance inst in FusionFrame.FrameInstances.Instances)
                new SceneGameObject(this, inst);
        }

        public void Write(string projectPath)
        {
            Directory.CreateDirectory(Path.Combine(projectPath, "Assets\\Frames"));

            List<string> yamlWriter = new();
            yamlWriter.Add("%YAML 1.1");
            yamlWriter.Add("%TAG !u! tag:unity3d.com,2011:");
            Components.ForEach(x => yamlWriter.Add(x.Write()));
            File.WriteAllLines(Path.Combine(projectPath, "Assets\\Frames", FusionFrame.FrameName + ".unity"), yamlWriter.ToArray());
        }
    }
}
