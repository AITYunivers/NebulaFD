using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using Spectre.Console;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks;

#pragma warning disable CS8602
namespace SapphireD.Core.Data.PackageReaders
{
    public class MFAPackageData : PackageData
    {
        public ImageBank IconBank = new();
        public string Description = string.Empty;
        public string Company = string.Empty;
        public string Version = string.Empty;
        public FrameEvents GlobalEvents = new FrameEvents();
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
            Description = reader.ReadAutoYuniversal();
            Copyright = reader.ReadAutoYuniversal();
            Company = reader.ReadAutoYuniversal();
            Version = reader.ReadAutoYuniversal();
            AppHeader.AppWidth = (short)reader.ReadInt();
            AppHeader.AppHeight = (short)reader.ReadInt();
            AppHeader.BorderColor = reader.ReadColor();
            AppHeader.DisplayFlags.Value = reader.ReadUInt();
            AppHeader.GraphicFlags.Value = reader.ReadUInt();
            HelpFile = reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            AppHeader.InitScore = (reader.ReadInt() + 1) * -1;
            AppHeader.InitLives = (reader.ReadInt() + 1) * -1;
            AppHeader.FrameRate = reader.ReadInt();
            reader.Skip(4);
            TargetFilename = reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            reader.ReadAutoYuniversal();
            About = reader.ReadAutoYuniversal();
            reader.Skip(4);
            BinaryFiles.ReadMFA(reader);

            AppHeader.ControlType = new int[reader.ReadInt()];
            AppHeader.ControlKeys = new int[AppHeader.ControlType.Length][];
            for (int i = 0; i < AppHeader.ControlType.Length; i++)
            {
                AppHeader.ControlType[i] = reader.ReadInt();
                AppHeader.ControlKeys[i] = new int[reader.ReadInt()];
                for (int ii = 0; ii < AppHeader.ControlKeys[i].Length; ii++)
                    AppHeader.ControlKeys[i][ii] = reader.ReadInt();
            }

            MenuBar.ReadMFA(reader);
            AppHeader.WindowMenu = reader.ReadInt();

            // Menu Images (Not implemented yet)
            reader.Skip(reader.ReadInt() * 8);

            GlobalValues.ReadMFA(reader, GlobalValueNames);
            GlobalStrings.ReadMFA(reader, GlobalStringNames);
            GlobalEvents.ReadMFA(reader, true);

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

            while (true)
            {
                Chunk newChunk = Chunk.InitMFAChunk(reader);
                Logger.Log(this, $"Reading MFA Object Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadMFA(chunkReader, this);
                newChunk.ChunkData = new byte[0];
                if (newChunk is Last)
                    break;
            }

            FinishParsing();

            Finished = true;
        }

        public void FinishParsing()
        {
            foreach (Frame frame in Frames)
            {
                foreach (MFAObjectInfo oI in frame.MFAFrameInfo.Objects)
                {
                    ObjectInfo newOI = new ObjectInfo();
                    newOI.Header.Flags = oI.Flags;
                    newOI.Header.Handle = oI.Handle;
                    newOI.Header.Type = oI.ObjectType;
                    newOI.Header.InkEffect = oI.InkEffect;
                    newOI.Header.InkEffectParam = oI.InkEffectParameter;
                    newOI.Name = oI.Name;
                    
                    switch (oI.ObjectType)
                    {
                        case 0:
                            ObjectQuickBackdrop newOQB = new ObjectQuickBackdrop();
                            MFAQuickBackdrop? oldOQB = oI.ObjectLoader as MFAQuickBackdrop;
                            newOQB.ObstacleType = oldOQB.ObstacleType;
                            newOQB.CollisionType = oldOQB.CollisionType;
                            newOQB.Width = oldOQB.Width;
                            newOQB.Height = oldOQB.Height;
                            newOQB.Shape.BorderSize = oldOQB.BorderSize;
                            newOQB.Shape.BorderColor = oldOQB.BorderColor;
                            newOQB.Shape.ShapeType = oldOQB.Shape;
                            newOQB.Shape.FillType = oldOQB.FillType;
                            newOQB.Shape.LineFlags = oldOQB.Flags;
                            newOQB.Shape.Color1 = oldOQB.Color1;
                            newOQB.Shape.Color2 = oldOQB.Color2;
                            newOQB.Shape.GradientFlags = oldOQB.Flags;
                            newOQB.Shape.Image = oldOQB.Image;
                            break;
                        case 1:
                            ObjectBackdrop newOBD = new ObjectBackdrop();
                            MFABackdrop? oldOBD = oI.ObjectLoader as MFABackdrop;
                            newOBD.ObstacleType = oldOBD.ObstacleType;
                            newOBD.CollisionType = oldOBD.CollisionType;
                            newOBD.Image = oldOBD.Image;
                            break;
                        default:
                            ObjectCommon newOC = new ObjectCommon();
                            MFAObjectLoader? oldOC = oI.ObjectLoader;
                            newOC.BackColor = oldOC.Background;
                            newOC.Qualifiers = oldOC.Qualifiers;
                            newOC.ObjectAlterableValues = oldOC.AlterableValues;
                            newOC.ObjectAlterableStrings = oldOC.AlterableStrings;
                            newOC.ObjectMovements = oldOC.Movements;
                            newOC.ObjectTransitionIn = oldOC.TransitionIn;
                            newOC.ObjectTransitionOut = oldOC.TransitionOut;

                            switch (oI.ObjectType)
                            {
                                case 2:
                                    newOC.ObjectAnimations.Animations = (oldOC as MFAActive).Animations;
                                    break;
                                case 3:
                                    newOC.ObjectParagraphs.Width = (oldOC as MFAString).Width;
                                    newOC.ObjectParagraphs.Height = (oldOC as MFAString).Height;
                                    newOC.ObjectParagraphs.Paragraphs = (oldOC as MFAString).Paragraphs;
                                    break;
                                case 7:
                                    newOC.ObjectCounter.Flags = (oldOC as MFACounter).Flags;
                                    newOC.ObjectCounter.DisplayType = (oldOC as MFACounter).DisplayType;
                                    newOC.ObjectCounter.Width = (oldOC as MFACounter).Width;
                                    newOC.ObjectCounter.Height = (oldOC as MFACounter).Height;
                                    newOC.ObjectCounter.Frames = (oldOC as MFACounter).Images;
                                    newOC.ObjectCounter.Font = (oldOC as MFACounter).Font;
                                    newOC.ObjectValue.Initial = (oldOC as MFACounter).Value;
                                    newOC.ObjectValue.Minimum = (oldOC as MFACounter).Minimum;
                                    newOC.ObjectValue.Maximum = (oldOC as MFACounter).Maximum;
                                    break;
                                default:
                                    newOC.ObjectExtension.ExtensionVersion = (oldOC as MFAExtensionObject).Version;
                                    newOC.ObjectExtension.ExtensionID = (oldOC as MFAExtensionObject).ID;
                                    newOC.ObjectExtension.ExtensionPrivate = (oldOC as MFAExtensionObject).Private;
                                    newOC.ObjectExtension.ExtensionData = (oldOC as MFAExtensionObject).Data;
                                    break;
                            }
                            break;
                    }
                }
            }
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
