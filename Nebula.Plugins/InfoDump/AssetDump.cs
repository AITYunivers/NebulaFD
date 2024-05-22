﻿using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using System.Diagnostics;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using System.Reflection;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.BankChunks.Music;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.FileReaders;
using Nebula.Core.Data.Chunks.BankChunks.Shaders;
using System.Text;
using System.Xml;
#pragma warning disable CA1416

namespace Nebula.Plugins.GameDumper
{
    public class AssetDump : INebulaTool
    {
        public string Name => "Asset Dumper";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            List<string> tasknames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Exit[/]",
                $"[{NebulaCore.ColorRules[3]}]Dump Images[/]",
                //$"[{NebulaCore.ColorRules[3]}]Dump Sprite Sheets[/]",
                $"[{NebulaCore.ColorRules[3]}]Dump Sounds[/]",
                $"[{NebulaCore.ColorRules[3]}]Dump Music[/]",
                $"[{NebulaCore.ColorRules[3]}]Dump Packed Data[/]",
                $"[{NebulaCore.ColorRules[3]}]Dump Shaders[/]",
            };

            List<string> selectedTasks = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"[{NebulaCore.ColorRules[1]}]Select a task.[/]")
                    .InstructionsText(
                        $"[{NebulaCore.ColorRules[2]}](Press [{NebulaCore.ColorRules[1]}]<space>[/] to select a task, " +
                        $"[{NebulaCore.ColorRules[1]}]<enter>[/] to execute)[/]")
                    .AddChoices(tasknames)
                    .HighlightStyle(NebulaCore.ColorRules[4]));


            List<Task> runningTasks = new List<Task>();
            foreach (string task in selectedTasks)
            {
                switch (Markup.Remove(task))
                {
                    case "Dump Images":
                        runningTasks.Add(new Task(ImageDump));
                        break;
                    case "Dump Sprite Sheets":
                        runningTasks.Add(new Task(SpriteSheetDump));
                        break;
                    case "Dump Sounds":
                        runningTasks.Add(new Task(SoundDump));
                        break;
                    case "Dump Music":
                        runningTasks.Add(new Task(MusicDump));
                        break;
                    case "Dump Packed Data":
                        runningTasks.Add(new Task(PackedDump));
                        break;
                    case "Dump Shaders":
                        runningTasks.Add(new Task(ShaderDump));
                        break;
                }
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                progressContext = ctx;

                foreach (Task task in runningTasks)
                    task.Start();

                foreach (Task task in runningTasks)
                    task.Wait();
            });
        }

        ProgressContext? progressContext;
        public void ImageDump()
        {
            ProgressTask? task = progressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Images[/]", false);

            int progress = 0;
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Images";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.ImageBank != null)
                {
                    if (NebulaCore.PackageData.ImageBank.Images.Count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = progress;
                    task.MaxValue = NebulaCore.PackageData.ImageBank.Images.Count;
                    if (NebulaCore.PackageData is MFAPackageData && ((MFAPackageData)NebulaCore.PackageData).IconBank != null)
                        task.MaxValue += ((MFAPackageData)NebulaCore.PackageData).IconBank.Images.Count;

                    Image[] images = NebulaCore.PackageData.ImageBank.Images.Values.ToArray();
                    for (int i = 0; i < images.Length; i++)
                    {
                        Directory.CreateDirectory(path);
                        images[i].GetBitmap().Save(path + "\\" + images[i].Handle + ".png");
                        task.Value = ++progress;
                    }

                    if (NebulaCore.PackageData is MFAPackageData && ((MFAPackageData)NebulaCore.PackageData).IconBank != null)
                    {
                        if (((MFAPackageData)NebulaCore.PackageData).IconBank.Images.Count == 0)
                            return;

                        images = ((MFAPackageData)NebulaCore.PackageData).IconBank.Images.Values.ToArray();
                        for (int i = 0; i < images.Length; i++)
                        {
                            Directory.CreateDirectory(path + "\\Icons");
                            images[i].GetBitmap().Save(path + "\\Icons\\" + images[i].Handle + ".png");
                            task.Value = ++progress;
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find the image bank.[/]");
                    Console.ReadKey();
                }
            }
        }

        public void SpriteSheetDump()
        {
            ProgressTask? task = progressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Sprite Sheets[/]", false);

            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\SpriteSheets";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.ImageBank != null && NebulaCore.PackageData.FrameItems != null)
                {
                    if (NebulaCore.PackageData.ImageBank.Images.Count == 0 ||
                        NebulaCore.PackageData.FrameItems.Items.Count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = 0;
                    task.MaxValue = NebulaCore.PackageData.FrameItems.Items.Count;

                    ObjectInfo[] objects = NebulaCore.PackageData.FrameItems.Items.Values.ToArray();
                    Directory.CreateDirectory(path);
                    for (int i = 0; i < objects.Length; i++)
                    {
                        ObjectInfo oI = objects[i];
                        switch (oI.Header.Type)
                        {
                            case 0: // Quick Backdrop
                                ObjectQuickBackdrop qBD = (ObjectQuickBackdrop)oI.Properties;
                                if (qBD.Shape.FillType != 3) continue;
                                Bitmap qBDOutput = new Bitmap(qBD.Width, qBD.Height);
                                using (Graphics qBDGraphics = Graphics.FromImage(qBDOutput))
                                {
                                    Image qBDImg = NebulaCore.PackageData.ImageBank.Images[qBD.Shape.Image];
                                    Rectangle qBDRect = new Rectangle(0, 0, qBD.Width, qBD.Height);
                                    Utilities.doDraw(qBDGraphics, qBDImg.GetBitmap(), qBDRect, 1.0f);
                                }
                                qBDOutput.Save($"{path}\\[{i}] {oI.Name}.png");
                                break;
                            case 1: // Backdrop
                                ObjectBackdrop bD = (ObjectBackdrop)oI.Properties;
                                Image bDImg = NebulaCore.PackageData.ImageBank.Images[bD.Image];
                                bDImg.GetBitmap().Save($"{path}\\[{i}] {oI.Name}.png");
                                break;
                            default: // Object Common
                                ObjectCommon oC = (ObjectCommon)oI.Properties;
                                switch (oI.Header.Type)
                                {
                                    case 2: // Active
                                        foreach (KeyValuePair<int, ObjectAnimation> aAnim in oC.ObjectAnimations.Animations)
                                        {
                                            foreach (ObjectDirection aDir in aAnim.Value.Directions)
                                            {
                                                int aColumns = (int)Math.Floor(Math.Pow(aDir.Frames.Length, 0.5));
                                                int aWidth, aHeight;
                                                foreach (uint aFrm in aDir.Frames)
                                                {
                                                    Image aImg = NebulaCore.PackageData.ImageBank.Images[aFrm];
                                                    int aImgWidth = aImg.Width;
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                        //(path + "\\" + i + ".png");
                        task.Value++;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find the image bank.[/]");
                    Console.ReadKey();
                }
            }
        }

        public void SoundDump()
        {
            ProgressTask? task = progressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Sounds[/]", false);

            int progress = 0;
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Sounds";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.SoundBank != null)
                {
                    if (NebulaCore.PackageData.SoundBank.Sounds.Count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = progress;
                    task.MaxValue = NebulaCore.PackageData.SoundBank.Sounds.Count;

                    Sound[] sounds = NebulaCore.PackageData.SoundBank.Sounds.Values.ToArray();
                    for (int i = 0; i < sounds.Length; i++)
                    {
                        Directory.CreateDirectory(path);
                        File.WriteAllBytes(path + "\\" + sounds[i].Name + GetExtension(sounds[i].Data), sounds[i].Data);
                        task.Value = ++progress;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find the sound bank.[/]");
                    Console.ReadKey();
                }
            }
        }

        public static string GetExtension(byte[] data)
        {
            if (data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F')
                return ".wav";
            if (data[0] == 0xFF && data[1] == 0xFB && data[2] == 0x90)
                return ".wav";
            if (data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S')
                return ".ogg";
            if (data[0] == 'F' && data[1] == 'O' && data[2] == 'R' && data[3] == 'M')
                return ".aiff";
            if (data[0] == 'I' && data[1] == 'D' && data[2] == '3')
                return ".mp3";

            // Because of Clickteam stole the MOD replayer from open-source OpenMPT library,
            // there's more file formats that can be supported by modflt.sft.
            return ".wav";
        }

        public void MusicDump()
        {
            ProgressTask? task = progressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Music[/]", false);

            int progress = 0;
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Music";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.MusicBank != null)
                {
                    if (NebulaCore.PackageData.MusicBank.Music.Count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = progress;
                    task.MaxValue = NebulaCore.PackageData.MusicBank.Music.Count;

                    Music[] music = NebulaCore.PackageData.MusicBank.Music.Values.ToArray();
                    for (int i = 0; i < music.Length; i++)
                    {
                        Directory.CreateDirectory(path);
                        File.WriteAllBytes(path + "\\" + music[i].Name + ".mid", music[i].Data);
                        task.Value = ++progress;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find the music bank.[/]");
                    Console.ReadKey();
                }
            }
        }

        public void PackedDump()
        {
            ProgressTask? task = progressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Packed Data[/]", false);

            int progress = 0;
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Packed Data";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.BinaryFiles != null || NebulaCore.PackageData.PackData != null)
                {
                    int count = 0;
                    if (NebulaCore.PackageData.BinaryFiles != null)
                        count += NebulaCore.PackageData.BinaryFiles.Count;
                    if (NebulaCore.PackageData.PackData != null)
                        count += NebulaCore.PackageData.PackData.Items.Length;
                    if (count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = progress;
                    task.MaxValue = count;

                    if (NebulaCore.PackageData.PackData != null)
                    {
                        PackFile[] packFiles = NebulaCore.PackageData.PackData.Items;
                        for (int i = 0; i < packFiles.Length; i++)
                        {
                            Directory.CreateDirectory(path);
                            File.WriteAllBytes(path + "\\" + Path.GetFileName(packFiles[i].PackFilename), packFiles[i].Data);
                            task.Value = ++progress;
                        }
                    }

                    if (NebulaCore.PackageData.BinaryFiles != null)
                    {
                        List<BinaryFile> binFiles = NebulaCore.PackageData.BinaryFiles.Items;
                        for (int i = 0; i < binFiles.Count; i++)
                        {
                            Directory.CreateDirectory(path + "\\Binary Files");
                            File.WriteAllBytes(path + "\\Binary Files\\" + Path.GetFileName(binFiles[i].FileName), binFiles[i].FileData);
                            task.Value = ++progress;
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find any packed data or binary files.[/]");
                    Console.ReadKey();
                }
            }
        }

        public void ShaderDump()
        {
            ProgressTask? task = progressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Shaders[/]", false);

            int progress = 0;
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Shaders";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.ShaderBank != null || NebulaCore.PackageData.DX9ShaderBank != null)
                {
                    int count = 0;
                    if (NebulaCore.PackageData.ShaderBank != null)
                        count += NebulaCore.PackageData.ShaderBank.Shaders.Count;
                    if (NebulaCore.PackageData.DX9ShaderBank != null)
                        count += NebulaCore.PackageData.DX9ShaderBank.Shaders.Count;
                    if (count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = progress;
                    task.MaxValue = count;

                    Shader[] shdrs;
                    if (NebulaCore.PackageData.ShaderBank != null)
                    {
                        shdrs = NebulaCore.PackageData.ShaderBank.Shaders.Values.ToArray();
                        int[] keys = NebulaCore.PackageData.ShaderBank.Shaders.Keys.ToArray();
                        ExtendedHeader eHdr = NebulaCore.PackageData.ExtendedHeader;
                        for (int i = 0; i < shdrs.Length; i++)
                        {
                            Directory.CreateDirectory(path);
                            string filePath = path + "\\" + Path.GetFileNameWithoutExtension(shdrs[i].Name) + (shdrs[i].Compiled ? (eHdr.Flags["PremultipliedAlpha"] ? ".premultiplied.fxc" : ".fxc") : ".fx");
                            File.WriteAllBytes(filePath, shdrs[i].FXData);
                            filePath = path + "\\" + Path.GetFileNameWithoutExtension(shdrs[i].Name) + ".xml";
                            string fxData = string.Empty;
                            if (NebulaCore.PackageData.DX9ShaderBank != null && NebulaCore.PackageData.DX9ShaderBank.Shaders.ContainsKey(keys[i]))
                                fxData = Encoding.ASCII.GetString(NebulaCore.PackageData.DX9ShaderBank.Shaders[keys[i]].FXData);
                            File.WriteAllText(filePath, GenerateXML(shdrs[i], fxData));
                            task.Value = ++progress;
                        }
                    }

                    if (NebulaCore.PackageData.DX9ShaderBank != null)
                    {
                        if (NebulaCore.PackageData.DX9ShaderBank.Shaders.Count != 0)
                        {
                            shdrs = NebulaCore.PackageData.DX9ShaderBank.Shaders.Values.ToArray();
                            for (int i = 0; i < shdrs.Length; i++)
                            {
                                Directory.CreateDirectory(path);
                                string filePath = path + "\\" + shdrs[i].Name;
                                File.WriteAllBytes(filePath, shdrs[i].FXData);
                                task.Value = ++progress;
                            }
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find the shader bank.[/]");
                    Console.ReadKey();
                }
            }
        }

        public string GenerateXML(Shader shdr, string fxData)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter w = XmlWriter.Create(sb, new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true
            });
            w.WriteStartDocument();

            w.WriteStartElement("effect");
            List<string> shdName = Path.GetFileNameWithoutExtension(shdr.Name).Replace('_', ' ').Replace('-', ' ').Split(' ').ToList();
            shdName.RemoveAll(x => x.Length == 0);
            for (int i = 0; i < shdName.Count; i++)
                shdName[i] = shdName[i].Substring(0, 1).ToUpper() + shdName[i].Substring(1);
            w.WriteElementString("name", string.Join(' ', shdName));
            w.WriteElementString("name", Path.GetFileNameWithoutExtension(shdr.Name));
            w.WriteElementString("description", "Decompiled using Nebula by Yunivers");
            w.WriteElementString("author", "Unknown");

            int[] shdrParams = new int[shdr.Parameters.Length];
            foreach (ObjectInfo obj in NebulaCore.PackageData.FrameItems.Items.Values)
                if (obj.Shader.ShaderHandle == shdr.Handle)
                {
                    shdrParams = obj.Shader.ShaderParameters;
                    break;
                }

            for (int i = 0; i < shdr.Parameters.Length; i++)
            {
                ShaderParameter param = shdr.Parameters[i];
                w.WriteStartElement("parameter");
                List<string> name = Path.GetFileNameWithoutExtension(shdr.Name).Replace('_', ' ').Replace('-', ' ').Split(' ').ToList();
                name.RemoveAll(x => x.Length == 0);
                for (int ii = 0; ii < name.Count; ii++)
                    name[ii] = name[ii].Substring(0, 1).ToUpper() + name[ii].Substring(1);
                w.WriteElementString("name", string.Join(' ', name));
                w.WriteElementString("code", param.Name);
                int value = shdrParams[i];
                switch (param.Type)
                {
                    case 0:
                        w.WriteElementString("type", "int");
                        if (fxData.Replace(" ", "").Contains("bool" + param.Name + ";") ||
                            fxData.Replace(" ", "").Contains("bool" + param.Value + "="))
                            w.WriteElementString("property", "checkbox");
                        else
                            w.WriteElementString("property", "edit");
                        w.WriteElementString("value", value.ToString());
                        break;
                    case 1:
                        w.WriteElementString("type", "float");
                        w.WriteElementString("property", "edit");
                        w.WriteElementString("value", value == 0 ? value.ToString() : BitConverter.Int32BitsToSingle(value).ToString());
                        break;
                    case 2:
                        w.WriteElementString("type", "INT_FLOAT4");
                        w.WriteElementString("property", "COLOR");
                        w.WriteElementString("value", value.ToString());
                        break;
                    case 3:
                        w.WriteElementString("type", "IMAGE");
                        w.WriteElementString("property", "image");
                        break;
                }
                w.WriteEndElement();
            }

            w.WriteEndElement();
            w.Flush();
            return sb.ToString();
        }
    }
}