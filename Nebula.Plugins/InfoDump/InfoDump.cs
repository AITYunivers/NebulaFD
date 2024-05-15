using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.FileReaders;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Reflection;

namespace Nebula.Plugins.GameDumper
{
    public class InfoDump : INebulaTool
    {
        public string Name => "Info Dumper";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            Dictionary<string, FieldInfo> propertyNames = new()
            {
                { "[[Quit]]", null! }
            };

            foreach (FieldInfo propertyInfo in typeof(PackageData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                switch (propertyInfo.FieldType.Name)
                {
                    case "Boolean":
                    case "Byte":
                    case "Char":
                    case "Int8":
                    case "UInt8":
                    case "Int16":
                    case "UInt16":
                    case "Int32":
                    case "UInt32":
                    case "String":
                        propertyNames.Add(propertyInfo.Name + ": " + propertyInfo.GetValue(NebulaCore.PackageData), null);
                        break;
                    default:
                        propertyNames.Add(propertyInfo.Name, propertyInfo);
                        break;
                }
            }

            SelectionPrompt<string> selectionPrompt = new SelectionPrompt<string>();
            selectionPrompt.Title = $"[{NebulaCore.ColorRules[1]}]Package Data[/]";
            selectionPrompt = selectionPrompt.AddChoices(propertyNames.Keys.ToArray());
            string? selectedProperty = AnsiConsole.Prompt(selectionPrompt);

            if (selectedProperty == "[[Quit]]")
                return;
            else if (propertyNames[selectedProperty] == null)
            {
                Execute();
                return;
            }
            else
            {
                SelectionPromptObject(propertyNames[selectedProperty].GetValue(NebulaCore.PackageData), new List<object> { null });
                return;
            }
        }

        public void SelectionPromptObject(object obj, List<object> parentObj)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            Dictionary<string, FieldInfo> propertyNames = new()
            {
                { "[[Up]]", null! }
            };

            foreach (FieldInfo propertyInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                switch (propertyInfo.FieldType.Name)
                {
                    case "Boolean":
                    case "Byte":
                    case "Char":
                    case "Int8":
                    case "UInt8":
                    case "Int16":
                    case "UInt16":
                    case "Int32":
                    case "UInt32":
                    case "Int64":
                    case "UInt64":
                    case "String":
                        propertyNames.Add(propertyInfo.Name + ": " + propertyInfo.GetValue(obj), null);
                        break;
                    default:
                        propertyNames.Add(propertyInfo.Name, propertyInfo);
                        break;
                }
            }

            SelectionPrompt<string> selectionPrompt = new SelectionPrompt<string>();
            selectionPrompt.Title = "[DeepSkyBlue2]" + obj.GetType().Name.Replace("[", "[[").Replace("]", "]]") + "[/]";
            selectionPrompt = selectionPrompt.AddChoices(propertyNames.Keys.ToArray());
            string? selectedProperty = AnsiConsole.Prompt(selectionPrompt);

            if (selectedProperty == "[[Up]]")
            {
                if (parentObj.Last() == null)
                    Execute();
                else
                {
                    object childObj = parentObj.Last();
                    parentObj.RemoveAt(parentObj.Count - 1);
                    SelectionPromptObject(childObj, parentObj);
                }
                return;
            }
            else if (propertyNames[selectedProperty] == null)
            {
                SelectionPromptObject(obj, parentObj);
                return;
            }
            else
            {
                parentObj.Add(obj);
                SelectionPromptObject(propertyNames[selectedProperty].GetValue(obj), parentObj);
                return;
            }
        }

        public void MenuBarWriter(ref TreeNode node, List<MenuItem> menuItems)
        {
            foreach (MenuItem menuItem in menuItems)
            {
                var menuItemTree = node.AddNode(new Markup(menuItem.Name));
                if (menuItem.Items != null && menuItem.Items.Count > 0)
                    MenuBarWriter(ref menuItemTree, menuItem.Items);
            }
        }
    }
}