using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Music;
using Nebula.Core.Utilities;
using Nebula.Plugins.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class MusicDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Music[/]", false);
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Music\\";
            task.MaxValue = NebulaCore.PackageData.MusicBank.Music.Count;
            Directory.CreateDirectory(path);

            Music[] music = NebulaCore.PackageData.MusicBank.Music.Values.ToArray();
            for (int i = 0; i < music.Length; i++)
            {
                File.WriteAllBytes(path + music[i].Name + ".mid", music[i].Data);
                task.Value++;
            }
            task.StopTask();
        }
    }
}
