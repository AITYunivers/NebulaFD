using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using Spectre.Console;

namespace SapphireD.Core.Data.PackageReaders
{
    public class MFAPackageData : PackageData
    {
        public ImageBank IconBank = new();
        public MFAQualifiers Qualifiers = new();
        public new MFAExtensions Extensions = new();
        public int[] FrameOffsets = new int[0];

        public bool Finished;

        public override void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {SapDCore.BuildDate} build.");
            Header = reader.ReadAscii(4);
            SapDCore._unicode = Header == "MFU2";
            SapDCore.MFA = true;
            Logger.Log(this, "Project Header: " + Header);

            RuntimeVersion = (short)reader.ReadUInt16();
            RuntimeSubversion = (short)reader.ReadUInt16();
            ProductVersion = reader.ReadInt32();
            ProductBuild = reader.ReadInt32();
            reader.Skip(4);
            SapDCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            AppName = reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            EditorFilename = reader.ReadAutoYuniversal();

            int stampLength = reader.ReadInt32();
            byte[] stamp = reader.ReadBytes(stampLength);

            var chunkHeader = reader.ReadAscii(4); // ATNF
            FontBank.ReadMFA(reader);

            chunkHeader = reader.ReadAscii(4); // APMS
            SoundBank.ReadMFA(reader);

            chunkHeader = reader.ReadAscii(4); // ASUM
            reader.Skip(4);
            //MusicBank.ReadMFA(reader);

            chunkHeader = reader.ReadAscii(4); // AGMI
            IconBank.ReadMFA(reader);

            chunkHeader = reader.ReadAscii(4); // AGMI
            ImageBank.ReadMFA(reader);

            AppName = reader.ReadAutoYuniversal();
            Author = reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            Copyright = reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            AppHeader.AppWidth = (short)reader.ReadInt();
            AppHeader.AppHeight = (short)reader.ReadInt();
            AppHeader.BorderColor = reader.ReadColor();
            AppHeader.DisplayFlags.Value = reader.ReadUInt();
            AppHeader.GraphicFlags.Value = reader.ReadUInt();
            reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            AppHeader.InitScore = (reader.ReadInt() + 1) * -1;
            AppHeader.InitLives = (reader.ReadInt() + 1) * -1;
            AppHeader.FrameRate = reader.ReadInt();
            reader.Skip(4);
            TargetFilename = reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            reader.Skip(4);
            BinaryFiles.ReadMFA(reader);

            var playerCount = reader.ReadInt();
            for (int i = 0; i < playerCount; i++)
            {
                AppHeader.ControlType[i] = (short)reader.ReadInt();
                reader.Skip(4);
                AppHeader.ControlKeys[i, 0] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 1] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 2] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 3] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 4] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 5] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 6] = (short)reader.ReadInt();
                AppHeader.ControlKeys[i, 7] = (short)reader.ReadInt();
                reader.Skip(32);
            }

            MenuBar.ReadMFA(reader);
            AppHeader.WindowMenu = reader.ReadInt();

            // Menu Images (Not implemented yet)
            reader.Skip(reader.ReadInt() * 8);

            GlobalValues.ReadMFA(reader, GlobalValueNames);
            GlobalStrings.ReadMFA(reader, GlobalStringNames);

            // Global Events (Not implemented yet)
            reader.Skip(reader.ReadInt());

            AppHeader.GraphicMode = (short)reader.ReadInt();
            reader.Skip(reader.ReadInt() * 4); // Icon Images
            Qualifiers.ReadMFA(reader);
            Extensions.ReadMFA(reader);

            if (reader.PeekInt() > 900)
                reader.Skip(2);

            FrameOffsets = new int[reader.ReadInt()];
            for (int i = 0; i < FrameOffsets.Length; i++)
                FrameOffsets[i] = reader.ReadInt();
            int returnOffset = reader.ReadInt();

            foreach (int offset in FrameOffsets)
            {
                reader.Seek(offset);
                Frame frame = new Frame();
                frame.ReadMFA(reader);
                Frames.Add(frame);
            }

            reader.Seek(returnOffset);

            Finished = true;
        }

        public override void CliUpdate()
        {
            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? mainTask = ctx.AddTask("[DeepSkyBlue3]Reading MFA[/]");
                mainTask.Value = 0;
                mainTask.MaxValue = 7;
                ProgressTask? secondaryTask = ctx.AddTask("[DeepSkyBlue3]Reading Font Bank[/]");

                while (!mainTask.IsFinished)
                {
                    if (Finished)
                        mainTask.Value = 7;
                    else if (Frames.Count > 0)
                    {
                        mainTask.Value = 6;

                        secondaryTask.Description = "[DeepSkyBlue3]Reading Frames[/]";
                        secondaryTask.Value = Frames.Count;
                        secondaryTask.MaxValue = FrameOffsets.Length;
                    }
                    else if (BinaryFiles.Count > 0)
                    {
                        mainTask.Value = 5;

                        secondaryTask.Description = "[DeepSkyBlue3]Reading Binary Files[/]";
                        secondaryTask.Value = BinaryFiles.Items.Count;
                        secondaryTask.MaxValue = BinaryFiles.Count;
                    }
                    else if (ImageBank.ImageCount > 0)
                    {
                        mainTask.Value = 4;

                        secondaryTask.Description = "[DeepSkyBlue3]Reading Image Bank[/]";
                        secondaryTask.Value = ImageBank.Images.Count;
                        secondaryTask.MaxValue = ImageBank.ImageCount;
                    }
                    else if (IconBank.ImageCount > 0)
                    {
                        mainTask.Value = 3;

                        secondaryTask.Description = "[DeepSkyBlue3]Reading Icon Bank[/]";
                        secondaryTask.Value = IconBank.Images.Count;
                        secondaryTask.MaxValue = IconBank.ImageCount;
                    }
                    // Insert Music Bank Here
                    else if (SoundBank.Count > 0)
                    {
                        mainTask.Value = 1;

                        secondaryTask.Description = "[DeepSkyBlue3]Reading Sound Bank[/]";
                        secondaryTask.Value = SoundBank.Sounds.Count;
                        secondaryTask.MaxValue = SoundBank.Count;
                    }
                    else if (FontBank.Count > 0)
                    {
                        mainTask.Value = 0;

                        secondaryTask.Description = "[DeepSkyBlue3]Reading Font Bank[/]";
                        secondaryTask.Value = FontBank.Fonts.Count;
                        secondaryTask.MaxValue = FontBank.Count;
                    }
                }
            });
        }
    }
}
