using System.Drawing;

namespace ZelTranslator_SD.Parsers.Unity.ProjectFiles.AssetFiles.SceneComponents
{
    public class SceneObjectTransform : ISceneComponent
    {
        public AssetScene Scene;
        public long ID;
        SceneGameObject GameObject;

        public SceneObjectTransform(AssetScene scene, SceneGameObject gameObject)
        {
            Scene = scene;
            ID = Scene.NextID++;
            GameObject = gameObject;
            scene.Components.Add(this);
        }

        public string Write()
        {
            // Big code, so gonna calculate it early.
            int zCalc = GameObject.Scene.FusionFrame.FrameInstances.Instances.Length * (int)GameObject.ObjectInstance.Layer;
            zCalc += GameObject.Scene.FusionFrame.FrameInstances.Instances.ToList().IndexOf(GameObject.ObjectInstance);

            List<string> yamlWriter = new()
            {
                "--- !u!4 &" + ID,
                "Transform:",
               $"  m_GameObject: {{fileID: {GameObject.ID}}}",
                "  serializedVersion: 2",
                "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
               $"  m_LocalPosition: {{x: {GameObject.ObjectInstance.PositionX}, " +
                                    $"y: {GameObject.ObjectInstance.PositionY}, " +
                                    $"z: {zCalc}}}",
                "  m_LocalScale: {x: 1, y: 1, z: 1}",
            };
            return string.Join('\n', yamlWriter);
        }
    }
}
