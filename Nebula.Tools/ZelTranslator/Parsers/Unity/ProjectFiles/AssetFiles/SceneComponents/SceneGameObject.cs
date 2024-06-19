using Nebula;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelTranslator_SD.Parsers.Unity.ProjectFiles.AssetFiles.SceneComponents
{
    public class SceneGameObject : ISceneComponent
    {
        public AssetScene Scene;
        public FrameInstance ObjectInstance;
        public ObjectInfo ObjectInfo;
        public long ID;
        public SceneObjectTransform Transform;
        
        public SceneGameObject(AssetScene scene, FrameInstance instance)
        {
            Scene = scene;
            ID = Scene.NextID++;
            ObjectInstance = instance;
            ObjectInfo = NebulaCore.PackageData.FrameItems.Items[(int)instance.ObjectInfo];
            scene.Components.Add(this);

            Transform = new SceneObjectTransform(scene, this);
        }

        public string Write()
        {
            List<string> yamlWriter = new()
            {
                "--- !u!1 &" + ID,
                "GameObject:",
                "  serializedVersion: 6",
                "  m_Component:",
               $"  - component: {{fileID: {Transform.ID}}}",
                "  m_Layer: 0",
                "  m_Name: " + ObjectInfo.Name,
                "  m_TagString: Untagged",
                "  m_IsActive: 1"
            };
            return string.Join('\n', yamlWriter);
        }
    }
}
