namespace ZelTranslator_SD.Parsers.Unity.ProjectFiles
{
    public class ProjectSettings
    {
        public void Write(string projectPath)
        {
            Directory.CreateDirectory(Path.Combine(projectPath, "ProjectSettings"));
        }
    }
}
