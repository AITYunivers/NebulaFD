using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.MFAChunks;
using Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Nebula.Core.Data.Chunks.BankChunks.Shaders;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using System.Drawing;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using System.Xml.Linq;
using Nebula.Core.Data.Chunks.MFAChunks.MFAFrameChunks;
using System.Diagnostics;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class Frame : Chunk
    {
        public static int RealFrameCount = 0;
        public FrameHeader FrameHeader = new();               // 0x3334
        public string FrameName = string.Empty;               // 0x3335
        public string FramePassword = string.Empty;           // 0x3336
        public FramePalette FramePalette = new();             // 0x3337
        public FrameInstances FrameInstances = new();         // 0x3338
        public FrameTransitionIn FrameTransitionIn = new();   // 0x333B
        public FrameTransitionOut FrameTransitionOut = new(); // 0x333C
        public FrameEvents FrameEvents = new();               // 0x333D
        public FrameLayers FrameLayers = new();               // 0x3341
        public FrameRect FrameRect = new();                   // 0x3342
        public int FrameSeed;                                 // 0x3344
        public FrameLayerEffects FrameLayerEffects = new();   // 0x3345
        public int FrameMoveTimer;                            // 0x3347
        public FrameEffects FrameEffects = new();             // 0x3349
        public int Handle = -1;                               // 0x334C

        public MFAFrameInfo MFAFrameInfo = new();
        public Bitmap? BitmapCache = null;

        public Frame()
        {
            ChunkName = "Frame";
            ChunkID = 0x3333;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (Parameters.DontIncludeFrames.Contains(RealFrameCount++))
                return;
            string log = string.Empty;

            while (reader.HasMemory(8))
            {
                var newChunk = InitChunk(reader);
                log = $"Reading Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})";
                this.Log(log);

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader, this);
                newChunk.ChunkData = null;
            }

            log = $"Frame {(Handle >= 0 ? $"[{Handle}] " : "")}'{FrameName}' found.";

            if (FrameHeader.FrameFlags["DontInclude"])
                log += " (Not Included)";
            else
                NebulaCore.PackageData.Frames.Add(this);

            this.Log(log, color: ConsoleColor.Green);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadInt();
            FrameName = reader.ReadAutoYuniversal();
            FrameHeader.ReadMFA(reader);
            FrameHeader.SyncFlags(true);

            FrameEvents.MaxObjects = reader.ReadInt();

            FramePassword = reader.ReadAutoYuniversal();
            reader.Skip(reader.ReadInt()); // ?

            MFAFrameInfo.EditorX = reader.ReadInt();
            MFAFrameInfo.EditorY = reader.ReadInt();

            FramePalette.ReadMFA(reader);

            MFAFrameInfo.IconHandle = reader.ReadInt();
            MFAFrameInfo.EditorLayer = reader.ReadInt();

            FrameLayers.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                FrameTransitionIn.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                FrameTransitionOut.ReadMFA(reader);

            MFAFrameInfo.Objects = new MFAObjectInfo[reader.ReadInt()];
            for (int i = 0; i < MFAFrameInfo.Objects.Length; i++)
            {
                MFAFrameInfo.Objects[i] = new MFAObjectInfo();
                MFAFrameInfo.Objects[i].ReadMFA(reader);
            }

            MFAFrameInfo.Folders.ReadMFA(reader);
            FrameInstances.ReadMFA(reader);
            FrameEvents.ReadMFA(reader);

            while (true)
            {
                Chunk newChunk = InitMFAFrameChunk(reader);
                this.Log($"Reading MFA Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadMFA(chunkReader, this);
                newChunk.ChunkData = new byte[0];
                if (newChunk is Last)
                    break;
            }

            this.Log($"Frame '{FrameName}' found.", color: ConsoleColor.Green);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Handle);
            writer.WriteAutoYunicode(FrameName);
            FrameHeader.SyncFlags();
            FrameHeader.WriteMFA(writer);
            writer.WriteInt(FrameEvents.MaxObjects);
            writer.WriteAutoYunicode(FramePassword);

            // No clue, Ibs header
            writer.WriteInt(0);

            writer.WriteInt(FrameHeader.Width / 2);
            writer.WriteInt(FrameHeader.Height / 2);
            FramePalette.WriteMFA(writer);
            writer.WriteInt(MFAFrameInfo.IconHandle); // Icon Handle
            writer.WriteInt(0); // Active Layer
            FrameLayers.WriteMFA(writer);

            writer.WriteByte(string.IsNullOrEmpty(FrameTransitionIn.FileName) ? (byte)0 : (byte)1);
            if (!string.IsNullOrEmpty(FrameTransitionIn.FileName))
                FrameTransitionIn.WriteMFA(writer);

            writer.WriteByte(string.IsNullOrEmpty(FrameTransitionOut.FileName) ? (byte)0 : (byte)1);
            if (!string.IsNullOrEmpty(FrameTransitionOut.FileName))
                FrameTransitionOut.WriteMFA(writer);

            int backdropId = 1;
            Dictionary<uint, MFAObjectInfo> objectInfos = new Dictionary<uint, MFAObjectInfo>();
            foreach (FrameInstance instance in FrameInstances.Instances)
                if (!objectInfos.ContainsKey(instance.ObjectInfo) && NebulaCore.PackageData.FrameItems.Items.ContainsKey((int)instance.ObjectInfo))
                {
                    MFAObjectInfo newOI = new MFAObjectInfo();
                    ObjectInfo oI = NebulaCore.PackageData.FrameItems.Items[(int)instance.ObjectInfo];
                    newOI.SyncFlags(oI.Header.ObjectFlags);
                    newOI.Handle = oI.Header.Handle;
                    newOI.ObjectType = oI.Header.Type;
                    newOI.InkEffect = oI.Header.InkEffect;
                    newOI.InkEffectParameter = oI.Header.InkEffectParam;
                    newOI.Name = oI.Name;
                    newOI.Transparent = true;
                    newOI.IconHandle = oI.IconHandle;
                    newOI.IconType = 1;

                    if (newOI.InkEffect != 1)
                    {
                        newOI.ObjectEffects = new MFAObjectEffects();
                        newOI.ObjectEffects.RGBCoeff = oI.Header.RGBCoeff;
                        newOI.ObjectEffects.BlendCoeff = oI.Header.BlendCoeff;
                        newOI.ObjectEffects.HasShader = oI.Shader.ShaderHandle != -1;
                        if (newOI.ObjectEffects.HasShader)
                        {
                            newOI.ObjectEffects.Shader = NebulaCore.PackageData.ShaderBank[(int)oI.Shader.ShaderHandle!];
                            newOI.ObjectEffects.ShaderParameters = new ShaderParameter[oI.Shader.ShaderParameters.Length];
                            for (int i = 0; i < newOI.ObjectEffects.ShaderParameters.Length; i++)
                            {
                                newOI.ObjectEffects.ShaderParameters[i] = new ShaderParameter();
                                newOI.ObjectEffects.ShaderParameters[i].Name = newOI.ObjectEffects.Shader.Parameters[i].Name;
                                newOI.ObjectEffects.ShaderParameters[i].Type = newOI.ObjectEffects.Shader.Parameters[i].Type;
                                if (newOI.ObjectEffects.ShaderParameters[i].Type == 1)
                                    newOI.ObjectEffects.ShaderParameters[i].FloatValue = BitConverter.ToSingle(BitConverter.GetBytes(oI.Shader.ShaderParameters[i]));
                                else
                                    newOI.ObjectEffects.ShaderParameters[i].Value = oI.Shader.ShaderParameters[i];
                            }
                        }
                    }

                    switch (oI.Header.Type)
                    {
                        case 0:
                            MFAQuickBackdrop newOQB = new MFAQuickBackdrop();
                            ObjectQuickBackdrop? oldOQB = oI.Properties as ObjectQuickBackdrop;
                            newOQB.ObstacleType = oldOQB.ObstacleType;
                            newOQB.CollisionType = oldOQB.CollisionType;
                            newOQB.Width = oldOQB.Width * (oldOQB.Shape.LineFlags["FlipX"] ? -1 : 1);
                            newOQB.Height = oldOQB.Height * (oldOQB.Shape.LineFlags["FlipY"] ? -1 : 1);
                            newOQB.BorderSize = oldOQB.Shape.BorderSize;
                            newOQB.BorderColor = oldOQB.Shape.BorderColor;
                            newOQB.Shape = oldOQB.Shape.ShapeType;
                            newOQB.FillType = oldOQB.Shape.FillType;
                            newOQB.Color1 = oldOQB.Shape.Color1;
                            newOQB.Color2 = oldOQB.Shape.Color2;
                            newOQB.QuickBkdFlags.Value = oldOQB.Shape.VerticalGradient ? 1u : 0u;
                            newOQB.Image = oldOQB.Shape.Image;

                            newOI.ObjectLoader = newOQB;

                            if (NebulaCore.Fusion == 1.5f)
                                newOI.Name = "Quick Backdrop " + backdropId++;
                            break;
                        case 1:
                            MFABackdrop newOBD = new MFABackdrop();
                            ObjectBackdrop? oldOBD = oI.Properties as ObjectBackdrop;
                            newOBD.ObstacleType = oldOBD.ObstacleType;
                            newOBD.CollisionType = oldOBD.CollisionType;
                            newOBD.Image = oldOBD.Image;

                            newOI.ObjectLoader = newOBD;

                            if (NebulaCore.Fusion == 1.5f)
                                newOI.Name = "Backdrop " + backdropId++;
                            break;
                        default:
                            MFAObjectLoader newOC = oI.Header.Type switch
                            {
                                2 => new MFAActive(),
                                3 => new MFAString(),
                                4 => new MFAQNA(),
                                5 or 6 => new MFACounterAlt(),
                                7 => new MFACounter(),
                                8 => new MFAFormattedText(),
                                9 => new MFASubApplication(),
                                _ => new MFAExtensionObject()
                            };
                            ObjectCommon? oldOC = oI.Properties as ObjectCommon;
                            newOC.ObjectFlags.Value = oldOC.ObjectFlags.Value;
                            newOC.ObjectFlags["CCNCheck"] = false;
                            newOC.NewObjectFlags.Value = oldOC.NewObjectFlags.Value;
                            newOC.Background = oldOC.BackColor;
                            newOC.Qualifiers = oldOC.Qualifiers;
                            newOC.AlterableValues = oldOC.ObjectAlterableValues;
                            newOC.AlterableStrings = oldOC.ObjectAlterableStrings;
                            newOC.Movements = oldOC.ObjectMovements;

                            if (newOC.Movements.Movements.Length == 0)
                                newOC.Movements.Movements = new ObjectMovement[] { new ObjectMovement() { Opt = 1 } };

                            if (newOC.AlterableValues.AlterableFlags.Value != 0)
                            {
                                int count;
                                for (count = 31; count > 0; count--)
                                    if (newOC.AlterableValues.AlterableFlags == count - 1)
                                        break;
                                newOI.AltFlags = new MFAAltFlags();
                                newOI.AltFlags.AlterableFlags = new MFAAltFlag[count];
                                for (int i = 0; i < count; i++)
                                {
                                    MFAAltFlag altFlag = new MFAAltFlag();
                                    altFlag.Name = newOC.AlterableValues.FlagNames[i] ?? "";
                                    altFlag.Value = newOC.AlterableValues.AlterableFlags == i;
                                    newOI.AltFlags.AlterableFlags[i] = altFlag;
                                }
                            }

                            newOC.TransitionIn = oldOC.ObjectTransitionIn;
                            newOC.TransitionOut = oldOC.ObjectTransitionOut;

                            switch (oI.Header.Type)
                            {
                                case 2:
                                    int highest = 0;
                                    foreach (var Dict in oldOC.ObjectAnimations.Animations)
                                        highest = Math.Max(highest, Dict.Key);
                                    (newOC as MFAActive).Animations = new Dictionary<int, ObjectAnimation>();
                                    for (int i = 0; i <= highest; i++)
                                        if (oldOC.ObjectAnimations.Animations.ContainsKey(i))
                                            (newOC as MFAActive).Animations.Add(i, oldOC.ObjectAnimations.Animations[i]);
                                        else
                                            (newOC as MFAActive).Animations.Add(i, new ObjectAnimation());
                                    break;
                                case 3:
                                    (newOC as MFAString).Width = oldOC.ObjectParagraphs.Width;
                                    (newOC as MFAString).Height = oldOC.ObjectParagraphs.Height;
                                    (newOC as MFAString).Font = oldOC.ObjectParagraphs.Paragraphs[0].FontHandle;
                                    (newOC as MFAString).Color = oldOC.ObjectParagraphs.Paragraphs[0].Color;
                                    (newOC as MFAString).StringFlags.Value = oldOC.ObjectParagraphs.Paragraphs[0].ParagraphFlags.Value;
                                    (newOC as MFAString).Paragraphs = oldOC.ObjectParagraphs.Paragraphs;
                                    break;
                                case 4:
                                    (newOC as MFAQNA).Width = oldOC.ObjectParagraphs.Width;
                                    (newOC as MFAQNA).Height = oldOC.ObjectParagraphs.Height;

                                    (newOC as MFAQNA).QuestionFont = oldOC.ObjectParagraphs.Paragraphs[0].FontHandle;
                                    (newOC as MFAQNA).QuestionColor = oldOC.ObjectParagraphs.Paragraphs[0].Color;
                                    (newOC as MFAQNA).QuestionRelief = oldOC.ObjectParagraphs.Paragraphs[0].ParagraphFlags["Relief"];
                                    (newOC as MFAQNA).QuestionParagraph = oldOC.ObjectParagraphs.Paragraphs[0];

                                    (newOC as MFAQNA).AnswerFont = oldOC.ObjectParagraphs.Paragraphs[1].FontHandle;
                                    (newOC as MFAQNA).AnswerColor = oldOC.ObjectParagraphs.Paragraphs[1].Color;
                                    (newOC as MFAQNA).AnswerRelief = oldOC.ObjectParagraphs.Paragraphs[1].ParagraphFlags["Relief"];
                                    List<ObjectParagraph> ansParas = oldOC.ObjectParagraphs.Paragraphs.ToList();
                                    ansParas.RemoveAt(0);
                                    ansParas[0].ParagraphFlags["MFACorrect"] = oldOC.ObjectParagraphs.Paragraphs[1].ParagraphFlags["Correct"];
                                    (newOC as MFAQNA).AnswerParagraphs = ansParas.ToArray();
                                    break;
                                case 5:
                                case 6:
                                    (newOC as MFACounterAlt).Player = oldOC.ObjectCounter.Player;
                                    (newOC as MFACounterAlt).Images = oldOC.ObjectCounter.Frames;
                                    (newOC as MFACounterAlt).UseText = oldOC.ObjectCounter.Frames.Length == 0;
                                    if ((newOC as MFACounterAlt).UseText)
                                    {
                                        (newOC as MFACounterAlt).Color = oldOC.ObjectParagraphs.Paragraphs[0].Color;
                                        (newOC as MFACounterAlt).Font = oldOC.ObjectParagraphs.Paragraphs[0].FontHandle;
                                    }
                                    else
                                    {
                                        (newOC as MFACounterAlt).Color = Color.Black;
                                        (newOC as MFACounterAlt).Font = -1;
                                    }
                                    (newOC as MFACounterAlt).Width = oldOC.ObjectCounter.Width;
                                    (newOC as MFACounterAlt).Height = oldOC.ObjectCounter.Height;

                                    newOI.CounterAltFlags = new MFACounterAltFlags();
                                    newOI.CounterAltFlags.CounterAltFlags.Value = 0; // Default Value;
                                    newOI.CounterAltFlags.CounterAltFlags["FixedDigitCount"] = oldOC.ObjectCounter.IntDigitPadding;
                                    newOI.CounterAltFlags.FixedDigits = oldOC.ObjectCounter.IntDigitCount;
                                    break;
                                case 7:
                                    (newOC as MFACounter).DisplayType = oldOC.ObjectCounter.DisplayType;
                                    (newOC as MFACounter).Width = oldOC.ObjectCounter.Width * (oldOC.ObjectCounter.Shape.LineFlags["FlipX"] ? -1 : 1);
                                    (newOC as MFACounter).Height = oldOC.ObjectCounter.Height * (oldOC.ObjectCounter.Shape.LineFlags["FlipY"] ? -1 : 1);
                                    (newOC as MFACounter).BarDirection = oldOC.ObjectCounter.BarDirection ? 1 : 0;
                                    (newOC as MFACounter).FillType = oldOC.ObjectCounter.Shape.FillType;
                                    (newOC as MFACounter).Color1 = oldOC.ObjectCounter.Shape.Color1;
                                    (newOC as MFACounter).Color2 = oldOC.ObjectCounter.Shape.Color2;
                                    (newOC as MFACounter).VerticalGradient = oldOC.ObjectCounter.Shape.VerticalGradient;
                                    (newOC as MFACounter).Images = oldOC.ObjectCounter.Frames;
                                    (newOC as MFACounter).Font = oldOC.ObjectCounter.Font;
                                    (newOC as MFACounter).Value = oldOC.ObjectValue.Initial;
                                    (newOC as MFACounter).Minimum = oldOC.ObjectValue.Minimum;
                                    (newOC as MFACounter).Maximum = oldOC.ObjectValue.Maximum;

                                    newOI.CounterFlags = new MFACounterFlags();
                                    newOI.CounterFlags.CounterFlags.Value = 0; // Default Value;
                                    newOI.CounterFlags.CounterFlags["IntFixedDigitCount"] = oldOC.ObjectCounter.IntDigitPadding;
                                    newOI.CounterFlags.CounterFlags["FloatFixedWholeCount"] = oldOC.ObjectCounter.FloatWholePadding;
                                    newOI.CounterFlags.CounterFlags["FloatFixedDecimalCount"] = oldOC.ObjectCounter.FloatDecimalPadding;
                                    newOI.CounterFlags.CounterFlags["FloatPadLeft"] = oldOC.ObjectCounter.FloatPadding;
                                    newOI.CounterFlags.FixedDigits = oldOC.ObjectCounter.IntDigitCount;
                                    newOI.CounterFlags.SignificantDigits = oldOC.ObjectCounter.FloatWholeCount;
                                    newOI.CounterFlags.DecimalPoints = oldOC.ObjectCounter.FloatDecimalCount;
                                    break;
                                case 8:
                                    (newOC as MFAFormattedText).Width = oldOC.ObjectFormattedText.Width;
                                    (newOC as MFAFormattedText).Height = oldOC.ObjectFormattedText.Height;
                                    (newOC as MFAFormattedText).FTFlags.Value = oldOC.ObjectFormattedText.FTFlags.Value;
                                    (newOC as MFAFormattedText).Color = oldOC.ObjectFormattedText.Color;
                                    (newOC as MFAFormattedText).Data = oldOC.ObjectFormattedText.Data;
                                    break;
                                case 9:
                                    (newOC as MFASubApplication).Name = oldOC.ObjectSubApplication.Name;
                                    (newOC as MFASubApplication).Width = oldOC.ObjectSubApplication.Width;
                                    (newOC as MFASubApplication).Height = oldOC.ObjectSubApplication.Height;
                                    (newOC as MFASubApplication).SubAppFlags.Value = oldOC.ObjectSubApplication.SubAppFlags.Value;
                                    (newOC as MFASubApplication).StartFrame = oldOC.ObjectSubApplication.StartFrame;
                                    break;
                                default:
                                    (newOC as MFAExtensionObject).Type = -1;
                                    Extension ext = NebulaCore.PackageData.Extensions.Exts[newOI.ObjectType - 32];
                                    (newOC as MFAExtensionObject).Name = ext.Name;
                                    (newOC as MFAExtensionObject).FileName = ext.FileName;
                                    (newOC as MFAExtensionObject).Magic = (uint)ext.MagicNumber;
                                    (newOC as MFAExtensionObject).SubType = ext.SubType;
                                    (newOC as MFAExtensionObject).Version = oldOC.ObjectExtension.ExtensionVersion;
                                    (newOC as MFAExtensionObject).ID = oldOC.ObjectExtension.ExtensionID;
                                    (newOC as MFAExtensionObject).Private = oldOC.ObjectExtension.ExtensionPrivate;
                                    (newOC as MFAExtensionObject).Data = oldOC.ObjectExtension.ExtensionData;
                                    break;
                            }

                            newOI.ObjectLoader = newOC;
                            break;
                    }

                    objectInfos.Add(instance.ObjectInfo, newOI);
                }

            writer.Write(objectInfos.Count);
            foreach (MFAObjectInfo oI in objectInfos.Values)
                oI.WriteMFA(writer);

            MFAFrameInfo.Folders.Folders = new MFAFolder[objectInfos.Count];
            for (uint i = 0; i < objectInfos.Count; i++)
            {
                MFAFolder folder = new MFAFolder();
                folder.HiddenFolder = true;
                folder.Children = new int[1];
                folder.Children[0] = (int)objectInfos.Keys.ToArray()[i];
                MFAFrameInfo.Folders.Folders[i] = folder;
            }
            MFAFrameInfo.Folders.WriteMFA(writer);
            FrameInstances.WriteMFA(writer);

            FrameEvents.QualifierJumptable = new();
            if (!NebulaCore.MFA)
            {
                Dictionary<int, EventObject> evtObjs = new();
                List<short> quals = new();
                foreach (MFAObjectInfo oI in objectInfos.Values)
                {
                    if (oI.ObjectType < 2) continue;
                    EventObject evtObj = new();
                    evtObj.Handle = NebulaCore.MFA ? evtObjs.Count : oI.Handle;
                    evtObj.ObjectType = 1;
                    evtObj.ItemType = (ushort)oI.ObjectType;
                    evtObj.Name = oI.Name;
                    evtObj.TypeName = oI.ObjectType switch
                    {
                        2 => "Sprite",
                        3 => "Text",
                        4 => "Question",
                        5 => "Score",
                        6 => "Lives",
                        7 => "Counter",
                        _ => string.Empty
                    };
                    evtObj.ItemHandle = (uint)oI.Handle;
                    evtObj.InstanceHandle = -1;
                    evtObjs.Add(evtObj.Handle, evtObj);
                }

                foreach (Qualifier qual in FrameEvents.Qualifiers)
                {
                    if (FrameEvents.QualifierJumptable.ContainsKey(Tuple.Create(qual.ObjectInfo, qual.Type)))
                        continue;
                    ushort jump;
                    for (jump = 0; jump < ushort.MaxValue; jump++)
                        if (!evtObjs.ContainsKey(jump))
                            break;
                    FrameEvents.QualifierJumptable.Add(Tuple.Create(qual.ObjectInfo, qual.Type), jump);
                    EventObject qualEvtObj = new();
                    qualEvtObj.Handle = jump;
                    qualEvtObj.ObjectType = 3;
                    qualEvtObj.ItemType = (ushort)qual.Type;
                    qualEvtObj.Name = string.Empty;
                    qualEvtObj.TypeName = qual.Type switch
                    {
                        2 => "Sprite",
                        3 => "Text",
                        4 => "Question",
                        5 => "Score",
                        6 => "Lives",
                        7 => "Counter",
                        _ => string.Empty
                    };
                    qualEvtObj.SystemQualifier = (ushort)(qual.ObjectInfo & 0x7FFF);
                    evtObjs.Add(qualEvtObj.Handle, qualEvtObj);
                }

                FrameEvents.EventObjects = evtObjs;
            }
            else
                foreach (var evtObj in FrameEvents.EventObjects)
                    if (evtObj.Value.ObjectType == 3)
                        FrameEvents.QualifierJumptable.Add(Tuple.Create((ushort)evtObj.Key, (short)evtObj.Value.ObjectType), (ushort)evtObj.Key);

            if (NebulaCore.Windows)
            {
                Dictionary<long, ParameterGroup> groupLookupTable = new();
                List<Parameter> checkParams = new List<Parameter>();
                foreach (Event evnt in FrameEvents.Events)
                {
                    foreach (Condition cond in evnt.Conditions)
                        checkParams.AddRange(cond.Parameters.Where(x => x.Code == 38 || x.Code == 39));
                    foreach (Events.Action act in evnt.Actions)
                        checkParams.AddRange(act.Parameters.Where(x => x.Code == 38 || x.Code == 39));
                }
                foreach (Parameter param in checkParams.Where(x => x.Code == 38))
                    if (param.Data is ParameterGroup group)
                    {
                        groupLookupTable.TryAdd(group.CCNPointer, group);
                        group.ID = (short)groupLookupTable.Keys.ToList().IndexOf(group.CCNPointer);

                        if (NebulaCore.Plus)
                            group.Name = "Group " + group.ID;
                    }
                foreach (Parameter param in checkParams.Where(x => x.Code == 39))
                    if (param.Data is ParameterGroupPointer point)
                    {
                        Debug.Assert(groupLookupTable.ContainsKey(point.CCNPointer),
                            "CCN Pointer is offset incorrectly for build " + NebulaCore.Build);
                        point.ID = groupLookupTable[point.CCNPointer].ID;
                    }
            }

            FrameEvents.WriteMFA(writer);

            FrameLayerEffects.WriteMFA(writer, this);
            FrameEffects.WriteMFA(writer, this);
            if (NebulaCore.Fusion > 1.5f)
                FrameRect.WriteMFA(writer, this);
            writer.WriteByte(0); // Last Chunk
        }
    }
}
