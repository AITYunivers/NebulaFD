using Nebula;
using Nebula.Core.FileReaders;
using Nebula.Core.Utilities;
using Nebula.Tools.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class PackDataDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Packed Data[/]", false);
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Packed Data\\";
            task.MaxValue = NebulaCore.PackageData.PackData.Items.Length;
            Directory.CreateDirectory(path);

            PackFile[] packFiles = NebulaCore.PackageData.PackData.Items;
            for (int i = 0; i < packFiles.Length; i++)
            {
                File.WriteAllBytes(GetPath(path, packFiles[i].PackFilename), packFiles[i].Data);
                task.Value++;
            }
            task.StopTask();
        }

        public static string GetPath(string basePath, string packFileName)
        {
            string[] pathSplit = packFileName.Split(Path.DirectorySeparatorChar);
            int searchCnt = 1;
            string search = pathSplit[^1];
            while (NebulaCore.PackageData.PackData.Items.Select(x => x.PackFilename).Where(x => x.EndsWith(search)).Count() > 1 && searchCnt < pathSplit.Length)
                search = pathSplit[^++searchCnt] + Path.PathSeparator + search;

            string outPath = Path.Combine(basePath, search);
            string outDir = Path.GetDirectoryName(outPath)!;
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outDir);
            return outPath;
        }
    }
}
