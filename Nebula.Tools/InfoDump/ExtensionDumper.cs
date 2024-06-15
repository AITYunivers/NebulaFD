/*using Ressy;
using Nebula.Core.Utilities;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula;
using Nebula.Core.Data.Chunks.AppChunks;
using Spectre.Console;
using Action = Nebula.Core.Data.Chunks.FrameChunks.Events.Action;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;

namespace Nebula.Tools.GameDumper
{
    public class ExtensionDumper : INebulaTool
    {
        public string Name => "Extension Dumper";

        public void Execute()
        {
            List<Extension> exts = GetExtension();

            foreach (Extension ext in exts)
                DumpExtension(ext);
        }

        public List<Extension> GetExtension()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            List<string> extNames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Exit[/]",
            };

            foreach (Extension ext in NebulaCore.PackageData.Extensions.Exts.Values)
                extNames.Add($"[{NebulaCore.ColorRules[3]}]{ext.Name}[/]");

            List<string> selectedExts = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"[{NebulaCore.ColorRules[1]}]Select extension(s) to dump.[/]")
                    .InstructionsText(
                        $"[{NebulaCore.ColorRules[2]}](Press [{NebulaCore.ColorRules[1]}]<space>[/] to select an extension, " +
                        $"[{NebulaCore.ColorRules[1]}]<enter>[/] to execute)[/]")
                    .AddChoices(extNames)
                    .HighlightStyle(NebulaCore.ColorRules[4]));


            List<Extension> outputExts = new List<Extension>();
            foreach (string ext in selectedExts)
            {
                string extName = Markup.Remove(ext);
                if (extName == "Exit")
                    continue;
                outputExts.Add(NebulaCore.PackageData.Extensions.Exts.Values.Where(x => x.Name == extName).First());
            }

            return outputExts;
        }

        public void DumpExtension(Extension ext)
        {
            List<ExtensionEventData> Conditions = new();
            List<ExtensionEventData> Actions = new();
            List<ExtensionEventData> Expressions = new();

            foreach (Frame frm in NebulaCore.PackageData.Frames)
            {
                foreach (Event evt in frm.FrameEvents.Events)
                {
                    foreach (Condition cnd in evt.Conditions)
                    {
                        foreach (Parameter param in cnd.Parameters)
                        {
                            if (param.Data is ParameterExpressions expParams)
                            {
                                for (int i = 0; i < expParams.Expressions.Count; i++)
                                {
                                    ParameterExpression exp = expParams.Expressions[i];
                                    if (exp.ObjectType == ext.Handle + 32)
                                    {
                                        if (Expressions.Where(x => x.Num == exp.Num).Count() > 0)
                                            continue;
                                        ExtensionEventData evteData = new();
                                        evteData.Num = cnd.Num;
                                        Expressions.Add(evteData);
                                    }
                                }
                            }
                        }
                        if (cnd.ObjectType == ext.Handle + 32)
                        {
                            if (Conditions.Where(x => x.Num == cnd.Num).Count() > 0)
                                continue;
                            ExtensionEventData evtData = new();
                            evtData.Num = cnd.Num;
                            foreach (Parameter param in cnd.Parameters)
                                evtData.Parameters.Add(param.Code);
                            Conditions.Add(evtData);
                        }
                    }
                    foreach (Action act in evt.Actions)
                    {
                        foreach (Parameter param in act.Parameters)
                        {
                            if (param.Data is ParameterExpressions expParams)
                            {
                                for (int i = 0; i < expParams.Expressions.Count; i++)
                                {
                                    ParameterExpression exp = expParams.Expressions[i];
                                    if (exp.ObjectType == ext.Handle + 32)
                                    {
                                        if (Expressions.Where(x => x.Num == exp.Num).Count() > 0)
                                            continue;
                                        ExtensionEventData evteData = new();
                                        evteData.Num = act.Num;
                                        Expressions.Add(evteData);
                                    }
                                }
                            }
                        }
                        if (act.ObjectType == ext.Handle + 32)
                        {
                            if (Actions.Where(x => x.Num == act.Num).Count() > 0)
                                continue;
                            ExtensionEventData evtData = new();
                            evtData.Num = act.Num;
                            foreach (Parameter param in act.Parameters)
                                evtData.Parameters.Add(param.Code);
                            Actions.Add(evtData);
                        }
                    }
                }
            }

            List<string> StringTable = new();
            StringTable.Add(ext.FileName);


            //PortableExecutable exec = new PortableExecutable("Tools\\ExtensionDumper.mfx");
            //exec.SetResource()
        }

        public class ExtensionEventData
        {
            public short Num;
            public List<int> Parameters = new();
        }
    }
}
*/