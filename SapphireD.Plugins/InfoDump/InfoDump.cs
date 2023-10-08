using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Utilities;
using Spectre.Console;

namespace SapphireD.Plugins.GameDumper
{
    public class InfoDump : SapDPlugin
    {
        public string Name => "Info Dumper";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));

            var infoTree = new Tree(SapDCore.PackageData.AppName);
            if (!string.IsNullOrEmpty(SapDCore.PackageData.Copyright))
                infoTree.AddNode(new Markup("Copyright: " + SapDCore.PackageData.Copyright));
            infoTree.AddNode(new Markup("Build: " + SapDCore.Build));
            infoTree.AddNode(new Markup("Build Type: " + SapDCore.GameType()));
            var framesTree = infoTree.AddNode(new Markup($"Frames ({SapDCore.PackageData.Frames.Count}):"));
            foreach (Frame frame in SapDCore.PackageData.Frames)
                framesTree.AddNode(new Markup(frame.FrameName));

            AnsiConsole.Write(infoTree);
            Console.ReadKey();
        }
    }
}