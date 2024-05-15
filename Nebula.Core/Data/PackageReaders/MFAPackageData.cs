using Nebula.Core.Data.Chunks;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.MFAChunks;
using Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;

#pragma warning disable CS8602
namespace Nebula.Core.Data.PackageReaders
{
    public class MFAPackageData : PackageData
    {
        public ImageBank IconBank = new();
        public string Description = string.Empty;
        public string Company = string.Empty;
        public string Version = string.Empty;
        public FrameEvents GlobalEvents = new FrameEvents();
        public MFAQualifiers Qualifiers = new();
        public int[] FrameOffsets = new int[0];

        public override void Read()
        {
            Logger.Log(this, $"Running {NebulaCore.BuildDate} build.");
            Header = Reader.ReadAscii(4);
            NebulaCore._yunicode = Header == "MFU2";
            NebulaCore.MFA = true;
            Logger.Log(this, "Project Header: " + Header);

            RuntimeVersion = (short)Reader.ReadUInt16();
            RuntimeSubversion = (short)Reader.ReadUInt16();
            ProductVersion = Reader.ReadInt32();
            ProductBuild = Reader.ReadInt32();
            Reader.Skip(4); // Stamp
            NebulaCore.Build = ProductBuild;
            Logger.Log(this, "Fusion Build: " + ProductBuild);

            AppName = Reader.ReadAutoYuniversal();
            Reader.ReadAutoYuniversal();
            EditorFilename = Reader.ReadAutoYuniversal();

            int stampLength = Reader.ReadInt32();
            byte[] stamp = Reader.ReadBytes(stampLength);

            Reader.Skip(4); // ATNF
            FontBank.ReadMFA(Reader);

            Reader.Skip(4); // APMS
            SoundBank.ReadMFA(Reader);

            Reader.Skip(4); // ASUM
            Reader.Skip(4);
            //MusicBank.ReadMFA(Reader);

            Reader.Skip(4); // AGMI
            IconBank.ReadMFA(Reader);

            Reader.Skip(4); // AGMI
            ImageBank.ReadMFA(Reader);

            AppName = Reader.ReadAutoYuniversal();
            Author = Reader.ReadAutoYuniversal();
            Description = Reader.ReadAutoYuniversal();
            Copyright = Reader.ReadAutoYuniversal();
            Company = Reader.ReadAutoYuniversal();
            Version = Reader.ReadAutoYuniversal();
            AppHeader.AppWidth = (short)Reader.ReadInt();
            AppHeader.AppHeight = (short)Reader.ReadInt();
            AppHeader.BorderColor = Reader.ReadColor();
            AppHeader.DisplayFlags.Value = Reader.ReadUInt();
            AppHeader.GraphicFlags.Value = Reader.ReadUInt();
            AppHeader.SyncFlags(true);
            HelpFile = Reader.ReadAutoYuniversal();
            Reader.ReadAutoYuniversal();
            AppHeader.InitScore = (Reader.ReadInt() + 1) * -1;
            AppHeader.InitLives = (Reader.ReadInt() + 1) * -1;
            AppHeader.FrameRate = Reader.ReadInt();
            Reader.Skip(4); // Build Type
            TargetFilename = Reader.ReadAutoYuniversal();
            Reader.ReadAutoYuniversal();
            Reader.ReadAutoYuniversal();
            About = Reader.ReadAutoYuniversal();
            Reader.Skip(4);
            BinaryFiles.ReadMFA(Reader);

            AppHeader.ControlType = new int[Reader.ReadInt()];
            AppHeader.ControlKeys = new int[AppHeader.ControlType.Length][];
            for (int i = 0; i < AppHeader.ControlType.Length; i++)
            {
                AppHeader.ControlType[i] = Reader.ReadInt();
                AppHeader.ControlKeys[i] = new int[Reader.ReadInt()];
                for (int ii = 0; ii < AppHeader.ControlKeys[i].Length; ii++)
                    AppHeader.ControlKeys[i][ii] = Reader.ReadInt();
            }

            MenuBar.ReadMFA(Reader);
            AppHeader.WindowMenu = Reader.ReadInt();

            // Menu Images (Not implemented yet)
            Reader.Skip(Reader.ReadInt() * 8);

            GlobalValues.ReadMFA(Reader, GlobalValueNames);
            GlobalStrings.ReadMFA(Reader, GlobalStringNames);
            GlobalEvents.ReadMFA(Reader, true);

            AppHeader.GraphicMode = (short)Reader.ReadInt();
            {
                int cnt = Reader.ReadInt();
                long end = Reader.Tell() + cnt * 4;
                if (cnt >= 3)
                {
                    NebulaCore.CurrentReader.Icons.Add(64, IconBank.Images[Reader.ReadUInt()].GetBitmap());
                    NebulaCore.CurrentReader.Icons.Add(32, IconBank.Images[Reader.ReadUInt()].GetBitmap());
                    NebulaCore.CurrentReader.Icons.Add(16, IconBank.Images[Reader.ReadUInt()].GetBitmap());
                }
                Reader.Seek(end);
            }
            Qualifiers.ReadMFA(Reader);
            Extensions.ReadMFA(Reader);

            if (Reader.PeekInt() > 900)
                Reader.Skip(2);

            FrameOffsets = new int[Reader.ReadInt()];
            for (int i = 0; i < FrameOffsets.Length; i++)
                FrameOffsets[i] = Reader.ReadInt();
            int returnOffset = Reader.ReadInt();

            foreach (int offset in FrameOffsets)
            {
                Reader.Seek(offset);
                Frame frame = new Frame();
                frame.ReadMFA(Reader);
                Frames.Add(frame);
                FrameHandles.Add((short)frame.Handle);
            }

            Reader.Seek(returnOffset);

            while (true)
            {
                Chunk newChunk = Chunk.InitMFAChunk(Reader);
                Logger.Log(this, $"Reading MFA Object Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadMFA(chunkReader, this);
                newChunk.ChunkData = new byte[0];
                if (newChunk is Last)
                    break;
            }

            FinishParsing();
        }

        public void FinishParsing()
        {
            Dictionary<int, ObjectInfo> frameItems = new Dictionary<int, ObjectInfo>();
            foreach (Frame frame in Frames)
            {
                foreach (MFAObjectInfo oI in frame.MFAFrameInfo.Objects)
                {
                    ObjectInfo newOI = new ObjectInfo();
                    newOI.Header.SyncFlags(oI.ObjectFlags);
                    newOI.Header.Handle = oI.Handle;
                    newOI.Header.Type = oI.ObjectType;
                    newOI.Header.InkEffect = oI.InkEffect;
                    newOI.Header.InkEffectParam = oI.InkEffectParameter;
                    newOI.Name = oI.Name;

                    if (newOI.Header.InkEffect != 1 && oI.ObjectEffects != null)
                    {
                        newOI.Header.RGBCoeff = oI.ObjectEffects.RGBCoeff;
                        newOI.Header.BlendCoeff = oI.ObjectEffects.BlendCoeff;
                        newOI.Shader.ShaderHandle = oI.ObjectEffects.ShaderHandle;
                        if (newOI.Shader.ShaderHandle != 0)
                        {
                            newOI.Shader.ShaderParameters = new int[oI.ObjectEffects.ShaderParameters.Length];
                            for (int i = 0; i < newOI.Shader.ShaderParameters.Length; i++)
                            {
                                if (oI.ObjectEffects.ShaderParameters[i].Type == 1)
                                    newOI.Shader.ShaderParameters[i] = BitConverter.ToInt32(BitConverter.GetBytes(oI.ObjectEffects.ShaderParameters[i].FloatValue));
                                else
                                    newOI.Shader.ShaderParameters[i] = oI.ObjectEffects.ShaderParameters[i].Value;
                            }
                        }
                    }

                    switch (oI.ObjectType)
                    {
                        case 0:
                            ObjectQuickBackdrop newOQB = new ObjectQuickBackdrop();
                            MFAQuickBackdrop? oldOQB = oI.ObjectLoader as MFAQuickBackdrop;
                            newOQB.ObstacleType = oldOQB.ObstacleType;
                            newOQB.CollisionType = oldOQB.CollisionType;
                            newOQB.Shape.LineFlags.Value = 0; // Default Value
                            newOQB.Shape.LineFlags["FlipX"] = oldOQB.Width < 0;
                            newOQB.Shape.LineFlags["FlipY"] = oldOQB.Height < 0;
                            newOQB.Width = oldOQB.Width * (newOQB.Shape.LineFlags["FlipX"] ? -1 : 1);
                            newOQB.Height = oldOQB.Height * (newOQB.Shape.LineFlags["FlipY"] ? -1 : 1);
                            newOQB.Shape.BorderSize = oldOQB.BorderSize;
                            newOQB.Shape.BorderColor = oldOQB.BorderColor;
                            newOQB.Shape.ShapeType = oldOQB.Shape;
                            newOQB.Shape.FillType = oldOQB.FillType;
                            newOQB.Shape.Color1 = oldOQB.Color1;
                            newOQB.Shape.Color2 = oldOQB.Color2;
                            newOQB.Shape.VerticalGradient = oldOQB.QuickBkdFlags["VerticalGradient"];
                            newOQB.Shape.Image = oldOQB.Image;

                            newOI.Properties = newOQB;
                            break;
                        case 1:
                            ObjectBackdrop newOBD = new ObjectBackdrop();
                            MFABackdrop? oldOBD = oI.ObjectLoader as MFABackdrop;
                            newOBD.ObstacleType = oldOBD.ObstacleType;
                            newOBD.CollisionType = oldOBD.CollisionType;
                            newOBD.Image = oldOBD.Image;

                            newOI.Properties = newOBD;
                            break;
                        default:
                            ObjectCommon newOC = new ObjectCommon();
                            MFAObjectLoader? oldOC = oI.ObjectLoader;
                            newOC.ObjectFlags = oldOC.ObjectFlags;
                            newOC.ObjectFlags["CCNCheck"] = true;
                            newOC.NewObjectFlags = oldOC.NewObjectFlags;
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
                                    newOC.ObjectAnimations.Animations = new Dictionary<int, ObjectAnimation>();
                                    for (int i = 0; i < (oldOC as MFAActive).Animations.Count; i++)
                                        newOC.ObjectAnimations.Animations.Add(i, (oldOC as MFAActive).Animations[i]);
                                    break;
                                case 3:
                                    newOC.ObjectParagraphs.Width = (oldOC as MFAString).Width;
                                    newOC.ObjectParagraphs.Height = (oldOC as MFAString).Height;
                                    newOC.ObjectParagraphs.Paragraphs = (oldOC as MFAString).Paragraphs;
                                    newOC.ObjectParagraphs.Paragraphs[0].FontHandle = (ushort)(oldOC as MFAString).Font;
                                    newOC.ObjectParagraphs.Paragraphs[0].Color = (oldOC as MFAString).Color;
                                    newOC.ObjectParagraphs.Paragraphs[0].ParagraphFlags.Value = (oldOC as MFAString).StringFlags.Value;
                                    break;
                                case 4:
                                    newOC.ObjectParagraphs.Width = (oldOC as MFAQNA).Width;
                                    newOC.ObjectParagraphs.Height = (oldOC as MFAQNA).Height;
                                    List<ObjectParagraph> objParas = (oldOC as MFAQNA).AnswerParagraphs.ToList();
                                    objParas.Insert(0, (oldOC as MFAQNA).QuestionParagraph);
                                    objParas[0].FontHandle = (ushort)(oldOC as MFAQNA).QuestionFont;
                                    objParas[0].Color = (oldOC as MFAQNA).QuestionColor;
                                    objParas[0].ParagraphFlags["Relief"] = (oldOC as MFAQNA).QuestionRelief;
                                    objParas[1].FontHandle = (ushort)(oldOC as MFAQNA).AnswerFont;
                                    objParas[1].Color = (oldOC as MFAQNA).AnswerColor;
                                    objParas[1].ParagraphFlags["Relief"] = (oldOC as MFAQNA).AnswerRelief;
                                    objParas[1].ParagraphFlags["Correct"] = objParas[1].ParagraphFlags["MFACorrect"];
                                    newOC.ObjectParagraphs.Paragraphs = objParas.ToArray();
                                    break;
                                case 5:
                                case 6:
                                    newOC.ObjectCounter.Player = (short)(oldOC as MFACounterAlt).Player;
                                    newOC.ObjectCounter.Frames = (oldOC as MFACounterAlt).Images;
                                    if ((oldOC as MFACounterAlt).UseText)
                                    {
                                        newOC.ObjectParagraphs.Paragraphs = new ObjectParagraph[1];
                                        newOC.ObjectParagraphs.Paragraphs[0] = new ObjectParagraph();
                                        newOC.ObjectParagraphs.Paragraphs[0].Color = (oldOC as MFACounterAlt).Color;
                                        newOC.ObjectParagraphs.Paragraphs[0].FontHandle = (ushort)(oldOC as MFACounterAlt).Font;
                                    }
                                    newOC.ObjectCounter.Width = (oldOC as MFACounterAlt).Width;
                                    newOC.ObjectCounter.Height = (oldOC as MFACounterAlt).Height;
                                    newOC.ObjectCounter.IntDigitPadding = oI.CounterAltFlags.CounterAltFlags["FixedDigitCount"];
                                    newOC.ObjectCounter.IntDigitCount = oI.CounterAltFlags.FixedDigits;
                                    break;
                                case 7:
                                    newOC.ObjectCounter.DisplayType = (oldOC as MFACounter).DisplayType;
                                    newOC.ObjectCounter.Width = (oldOC as MFACounter).Width;
                                    newOC.ObjectCounter.Height = (oldOC as MFACounter).Height;
                                    newOC.ObjectCounter.Frames = (oldOC as MFACounter).Images;
                                    newOC.ObjectCounter.Font = (oldOC as MFACounter).Font;
                                    newOC.ObjectCounter.Shape.LineFlags.Value = (oldOC as MFACounter).Width  < 0 ? 1u : 0 +
                                                                                (oldOC as MFACounter).Height < 0 ? 2u : 0;
                                    newOC.ObjectCounter.Shape.FillType = (oldOC as MFACounter).FillType;
                                    newOC.ObjectCounter.Shape.Color1 = (oldOC as MFACounter).Color1;
                                    newOC.ObjectCounter.Shape.Color2 = (oldOC as MFACounter).Color2;
                                    newOC.ObjectCounter.Shape.VerticalGradient = (oldOC as MFACounter).VerticalGradient;
                                    newOC.ObjectValue.Initial = (oldOC as MFACounter).Value;
                                    newOC.ObjectValue.Minimum = (oldOC as MFACounter).Minimum;
                                    newOC.ObjectValue.Maximum = (oldOC as MFACounter).Maximum;
                                    newOC.ObjectCounter.BarDirection = (oldOC as MFACounter).BarDirection == 1;

                                    if (oI.CounterFlags != null)
                                    {
                                        newOC.ObjectCounter.IntDigitPadding = oI.CounterFlags.CounterFlags["IntFixedDigitCount"];
                                        newOC.ObjectCounter.FloatWholePadding = oI.CounterFlags.CounterFlags["FloatFixedWholeCount"];
                                        newOC.ObjectCounter.FloatDecimalPadding = oI.CounterFlags.CounterFlags["FloatFixedDecimalCount"];
                                        newOC.ObjectCounter.FloatPadding = oI.CounterFlags.CounterFlags["FloatPadLeft"];
                                        newOC.ObjectCounter.IntDigitCount = oI.CounterFlags.FixedDigits;
                                        newOC.ObjectCounter.FloatWholeCount = oI.CounterFlags.SignificantDigits;
                                        newOC.ObjectCounter.FloatDecimalCount = oI.CounterFlags.DecimalPoints;
                                    }
                                    break;
                                case 8:
                                    newOC.ObjectFormattedText.FTFlags.Value = (oldOC as MFAFormattedText).FTFlags.Value;
                                    newOC.ObjectFormattedText.Color = (oldOC as MFAFormattedText).Color;
                                    newOC.ObjectFormattedText.Width = (oldOC as MFAFormattedText).Width;
                                    newOC.ObjectFormattedText.Height = (oldOC as MFAFormattedText).Height;
                                    newOC.ObjectFormattedText.Data = (oldOC as MFAFormattedText).Data;
                                    break;
                                case 9:
                                    newOC.ObjectSubApplication.Width = (oldOC as MFASubApplication).Width;
                                    newOC.ObjectSubApplication.Height = (oldOC as MFASubApplication).Height;
                                    newOC.ObjectSubApplication.StartFrame = (short)(oldOC as MFASubApplication).StartFrame;
                                    newOC.ObjectSubApplication.SubAppFlags = (oldOC as MFASubApplication).SubAppFlags;
                                    newOC.ObjectSubApplication.Name = (oldOC as MFASubApplication).Name;
                                    break;
                                default:
                                    newOC.ObjectExtension.ExtensionVersion = (oldOC as MFAExtensionObject).Version;
                                    newOC.ObjectExtension.ExtensionID = (oldOC as MFAExtensionObject).ID;
                                    newOC.ObjectExtension.ExtensionPrivate = (oldOC as MFAExtensionObject).Private;
                                    newOC.ObjectExtension.ExtensionData = (oldOC as MFAExtensionObject).Data;
                                    break;
                            }

                            newOI.Properties = newOC;
                            break;
                    }

                    if (!frameItems.ContainsKey(newOI.Header.Handle))
                        frameItems.Add(newOI.Header.Handle, newOI);
                }
            }

            FrameItems.Count = frameItems.Count;
            FrameItems.Items = frameItems;
        }
    }
}
