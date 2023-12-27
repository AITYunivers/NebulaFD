using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using SapphireD.Core.Data.Chunks.BankChunks.Shaders;
using SapphireD.Core.Data.Chunks.FrameChunks.Events;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class Frame : Chunk
    {
        public int Handle = 0;
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
        public int FrameMoveTimer;                            // 0x3347
        public FrameEffects FrameEffects = new();             // 0x3349

        public MFAFrameInfo MFAFrameInfo = new();

        public Frame()
        {
            ChunkName = "Frame";
            ChunkID = 0x3333;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            string log = string.Empty;

            while (reader.HasMemory(8))
            {
                var newChunk = InitChunk(reader);
                log = $"Reading Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})";
                Logger.Log(this, log);

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader, this);
                newChunk.ChunkData = null;
            }

            SapDCore.PackageData.Frames.Add(this);
            log = $"Frame '{FrameName}' found.";
            Logger.Log(this, log, color: ConsoleColor.Green);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadInt();
            FrameName = reader.ReadAutoYuniversal();
            FrameHeader.ReadMFA(reader);
            FrameHeader.SyncFlags(true);

            // Max Objects
            reader.Skip(4);

            FramePassword = reader.ReadAutoYuniversal();
            reader.Skip(reader.ReadInt()); // ?

            MFAFrameInfo.EditorX = reader.ReadInt();
            MFAFrameInfo.EditorY = reader.ReadInt();

            FramePalette.ReadMFA(reader);

            MFAFrameInfo.Stamp = reader.ReadInt();
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
                Chunk newChunk = InitMFAChunk(reader, false);
                Logger.Log(this, $"Reading MFA Frame Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadMFA(chunkReader, this);
                newChunk.ChunkData = new byte[0];
                if (newChunk is Last)
                    break;
            }

            Logger.Log(this, $"Frame '{FrameName}' found.", color: ConsoleColor.Green);
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
            writer.WriteInt(1000); // Max Objects
            writer.WriteAutoYunicode(FramePassword);

            // No clue, Ibs header
            writer.WriteInt(0);

            writer.WriteInt(FrameHeader.Width / 2);
            writer.WriteInt(FrameHeader.Height / 2);
            FramePalette.WriteMFA(writer);
            writer.WriteInt(73); // Stamp
            writer.WriteInt(0); // Active Layer
            FrameLayers.WriteMFA(writer);

            writer.WriteByte(string.IsNullOrEmpty(FrameTransitionIn.FileName) ? (byte)0 : (byte)1);
            if (!string.IsNullOrEmpty(FrameTransitionIn.FileName))
                FrameTransitionIn.WriteMFA(writer);

            writer.WriteByte(string.IsNullOrEmpty(FrameTransitionOut.FileName) ? (byte)0 : (byte)1);
            if (!string.IsNullOrEmpty(FrameTransitionOut.FileName))
                FrameTransitionOut.WriteMFA(writer);

            Dictionary<uint, MFAObjectInfo> objectInfos = new Dictionary<uint, MFAObjectInfo>();
            foreach (FrameInstance instance in FrameInstances.Instances)
                if (!objectInfos.ContainsKey(instance.ObjectInfo))
                {
                    MFAObjectInfo newOI = new MFAObjectInfo();
                    ObjectInfo oI = SapDCore.PackageData.FrameItems.Items[(int)instance.ObjectInfo];
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
                        newOI.ObjectEffects.ShaderHandle = oI.Shader.ShaderHandle;
                        if (newOI.ObjectEffects.ShaderHandle != 0)
                        {
                            newOI.ObjectEffects.Shader = SapDCore.PackageData.ShaderBank.Shaders[oI.Shader.ShaderHandle];
                            newOI.ObjectEffects.ShaderParameters = new ShaderParameter[Math.Min(oI.Shader.ShaderParameters.Length, newOI.ObjectEffects.Shader.Parameters.Length)];
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
                            break;
                        case 1:
                            MFABackdrop newOBD = new MFABackdrop();
                            ObjectBackdrop? oldOBD = oI.Properties as ObjectBackdrop;
                            newOBD.ObstacleType = oldOBD.ObstacleType;
                            newOBD.CollisionType = oldOBD.CollisionType;
                            newOBD.Image = oldOBD.Image;

                            newOI.ObjectLoader = newOBD;
                            break;
                        default:
                            MFAObjectLoader newOC = oI.Header.Type switch
                            {
                                2 => new MFAActive(),
                                3 => new MFAString(),
                                7 => new MFACounter(),
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
                                case 0:
                                case 1:
                                case 4:
                                case 5:
                                case 6:
                                case 8:
                                case 9:
                                    break;
                                default:
                                    (newOC as MFAExtensionObject).Type = -1;
                                    Extension ext = SapDCore.PackageData.Extensions.Exts[newOI.ObjectType - 32];
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
            List<EventObject> evtObjs = new();
            foreach (MFAObjectInfo oI in objectInfos.Values)
            {
                EventObject evtObj = new();
                evtObj.Handle = oI.Handle;
                evtObj.ObjectType = 1;
                evtObj.ItemType = (ushort)oI.ObjectType;
                evtObj.Name = oI.Name;
                evtObj.TypeName = oI.ObjectType switch
                {
                    2 => "Sprite",
                    3 => "Text",
                    7 => "Counter",
                    _ => ""
                };
                evtObj.ItemHandle = (uint)oI.Handle;
                evtObj.InstanceHandle = -1;
                evtObjs.Add(evtObj);
            }
            FrameEvents.EventObjects = evtObjs.ToArray();
            FrameEvents.WriteMFA(writer);

            //FrameLayerEffects layerEffects = new();
            //layerEffects.WriteMFA(writer, this);
            writer.WriteByte(0); // Last Chunk
        }
    }
}
