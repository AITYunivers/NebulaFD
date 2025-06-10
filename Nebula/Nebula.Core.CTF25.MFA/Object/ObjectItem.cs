using Nebula.Core.CTF25.MFA.Object.Data;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Reflection.PortableExecutable;

namespace Nebula.Core.CTF25.MFA.Object
{
    internal class ObjectItem : IReadable, IWritable
    {
        public EObjectTypes ObjectType;
        public uint Handle;
        public string Name = string.Empty;
        public bool Transparent;
        public uint InkEffectHandle;
        public int InkEffectParameter;
        public bool AntiAliasing;
        public uint Flags;
        public uint IconType;
        public uint IconHandle;
        public object? ObjectData;

        public void Read(ByteReader reader)
        {
            ObjectType = (EObjectTypes)reader.ReadUInt();
            Handle = reader.ReadUInt();
            Name = reader.ReadAutoYuniversal();
            Transparent = reader.ReadBool4();
            InkEffectHandle = reader.ReadUInt();
            InkEffectParameter = reader.ReadInt();
            AntiAliasing = reader.ReadBool4();
            Flags = reader.ReadUInt();
            IconType = reader.ReadUInt();
            IconHandle = reader.ReadUInt();

            while (true)
            {
                bool isLast = reader.ReadByte() == 0x00;
                if (isLast)
                    break;

                reader.Skip(reader.ReadInt()); // Data
            }

            switch (ObjectType)
            {
                case EObjectTypes.QUICK_BACKDROP:
                    QuickBackdropData quickBackdropData = new QuickBackdropData();
                    quickBackdropData.Read(reader);
                    ObjectData = quickBackdropData;
                    break;
                case EObjectTypes.BACKDROP:
                    BackdropData backdropData = new BackdropData();
                    backdropData.Read(reader);
                    ObjectData = backdropData;
                    break;
                case EObjectTypes.ACTIVE:
                    ActiveData activeData = new ActiveData();
                    activeData.Read(reader);
                    ObjectData = activeData;
                    break;
                case EObjectTypes.STRING:
                    StringData stringData = new StringData();
                    stringData.Read(reader);
                    ObjectData = stringData;
                    break;
                case EObjectTypes.QUESTION_AND_ANSWER:
                    QuestionData questionData = new QuestionData();
                    questionData.Read(reader);
                    ObjectData = questionData;
                    break;
                case EObjectTypes.SCORE:
                    BasicCounterData scoreData = new BasicCounterData();
                    scoreData.Read(reader);
                    ObjectData = scoreData;
                    break;
                case EObjectTypes.LIVES:
                    BasicCounterData livesData = new BasicCounterData();
                    livesData.Read(reader);
                    ObjectData = livesData;
                    break;
                case EObjectTypes.COUNTER:
                    CounterData counterData = new CounterData();
                    counterData.Read(reader);
                    ObjectData = counterData;
                    break;
                case EObjectTypes.FORMATTED_TEXT:
                    FormattedTextData formattedTextData = new FormattedTextData();
                    formattedTextData.Read(reader);
                    ObjectData = formattedTextData;
                    break;
                case EObjectTypes.SUB_APPLICATION:
                    SubApplicationData subApplicationData = new SubApplicationData();
                    subApplicationData.Read(reader);
                    ObjectData = subApplicationData;
                    break;
                case >= EObjectTypes.EXTENSION_BASE:
                    ExtensionData extensionData = new ExtensionData();
                    extensionData.Read(reader);
                    ObjectData = extensionData;
                    break;
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt((uint)ObjectType);
            writer.WriteUInt(Handle);
            writer.WriteAutoYunicode(Name);
            writer.WriteBool4(Transparent);
            writer.WriteUInt(InkEffectHandle);
            writer.WriteInt(InkEffectParameter);
            writer.WriteBool4(AntiAliasing);
            writer.WriteUInt(Flags);
            writer.WriteUInt(IconType);
            writer.WriteUInt(IconHandle);

            writer.WriteByte(0); // LAST Chunk

            if (ObjectData is IWritable writableObjectData)
                writableObjectData.Write(writer);
        }
    }
}
