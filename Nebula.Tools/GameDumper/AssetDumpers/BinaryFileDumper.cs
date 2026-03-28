using Nebula;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Utilities;
using Nebula.Tools.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class BinaryFileDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Binary Files[/]", false);
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Binary Files\\";
            task.MaxValue = NebulaCore.PackageData.BinaryFiles.Items.Count;
            Directory.CreateDirectory(path);

            foreach (BinaryFile binFile in NebulaCore.PackageData.BinaryFiles.Items)
            {
                File.WriteAllBytes(GetPath(path, binFile.FileName), binFile.FileData);
                task.Value++;
            }
            task.StopTask();
        }

        public static string GetPath(string basePath, string binFileName)
        {
            string[] pathSplit = binFileName.Split(Path.DirectorySeparatorChar);
            int searchCnt = 1;
            string search = pathSplit[^1];
            while (NebulaCore.PackageData.BinaryFiles.Items.Select(x => x.FileName).Where(x => x.EndsWith(search)).Count() > 1 && searchCnt < pathSplit.Length)
                search = pathSplit[^++searchCnt] + Path.DirectorySeparatorChar + search;

            string outPath = Path.Combine(basePath, search).Replace("..\\", "");
            string outDir = Path.GetDirectoryName(outPath)!;
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            return outPath;
        }
    }
}
