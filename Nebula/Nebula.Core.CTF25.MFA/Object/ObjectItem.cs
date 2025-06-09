using Nebula.Core.CTF25.MFA.Object.Data;
using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object
{
    internal class ObjectItem : IReadable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            EObjectTypes objectType = (EObjectTypes)reader.ReadUInt();
            uint handle = reader.ReadUInt();
            Name = reader.ReadAutoYuniversal();
            bool transparent = reader.ReadBool4();
            int inkEffect = reader.ReadInt();
            int inkEffectParameter = reader.ReadInt();
            bool antiAliasing = reader.ReadBool4();
            uint flags = reader.ReadUInt();
            uint iconType = reader.ReadUInt();
            uint iconHandle = reader.ReadUInt();

            while (true)
            {
                bool isLast = reader.ReadByte() == 0x00;
                if (isLast)
                    break;

                reader.Skip(reader.ReadInt()); // Data
            }

            switch (objectType)
            {
                case EObjectTypes.QUICK_BACKDROP:
                    QuickBackdropData quickBackdropData = new QuickBackdropData();
                    quickBackdropData.Read(reader);
                    break;
                case EObjectTypes.BACKDROP:
                    BackdropData backdropData = new BackdropData();
                    backdropData.Read(reader);
                    break;
                case EObjectTypes.ACTIVE:
                    ActiveData activeData = new ActiveData();
                    activeData.Read(reader);
                    break;
                case EObjectTypes.STRING:
                    StringData stringData = new StringData();
                    stringData.Read(reader);
                    break;
                case EObjectTypes.QUESTION_AND_ANSWER:
                    QuestionData questionData = new QuestionData();
                    questionData.Read(reader);
                    break;
                case EObjectTypes.SCORE:
                    BasicCounterData scoreData = new BasicCounterData();
                    scoreData.Read(reader);
                    break;
                case EObjectTypes.LIVES:
                    BasicCounterData livesData = new BasicCounterData();
                    livesData.Read(reader);
                    break;
                case EObjectTypes.COUNTER:
                    CounterData counterData = new CounterData();
                    counterData.Read(reader);
                    break;
                case EObjectTypes.FORMATTED_TEXT:
                    FormattedTextData formattedTextData = new FormattedTextData();
                    formattedTextData.Read(reader);
                    break;
                case EObjectTypes.SUB_APPLICATION:
                    SubApplicationData subApplicationData = new SubApplicationData();
                    subApplicationData.Read(reader);
                    break;
                case >= EObjectTypes.EXTENSION_BASE:
                    ExtensionData extensionData = new ExtensionData();
                    extensionData.Read(reader);
                    break;
            }
        }
    }
}
