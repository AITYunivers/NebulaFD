using Nebula.Core.CTF25.MFA.Object.Data.Animation;
using Nebula.Core.Memory;
using System.Reflection.PortableExecutable;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class ExtensionData : CommonObjectData
    {
        public AnimationBank? Animations;
        public uint Type;
        public string? Name;
        public string? FileName;
        public uint? Magic;
        public string? SubType;
        public byte[] Data = [];

        public override void ReadUncommonData(ByteReader reader)
        {
            bool hasAnimations = reader.ReadBool();
            if (hasAnimations)
                (Animations = []).Read(reader);

            Type = reader.ReadUInt();
            if (Type == uint.MaxValue)
            {
                Name = reader.ReadAutoYuniversal();
                FileName = reader.ReadAutoYuniversal();
                Magic = reader.ReadUInt();
                SubType = reader.ReadAutoYuniversal();
            }

            Data = reader.ReadBytes(reader.ReadInt());
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteBool(Animations != null);
            Animations?.Write(writer);

            writer.WriteUInt(Type);
            if (Type == uint.MaxValue)
            {
                writer.WriteAutoYunicode(Name!);
                writer.WriteAutoYunicode(FileName!);
                writer.WriteUInt(Magic!.Value);
                writer.WriteAutoYunicode(SubType!);
            }

            writer.WriteBytes(Data, true);
        }
    }
}
