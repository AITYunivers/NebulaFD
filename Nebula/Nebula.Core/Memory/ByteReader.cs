using Nebula.Core.Utilities;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Nebula.Core.Memory
{
    public class ByteReader : BinaryReader
    {
        private bool _unicode = false;

        public ByteReader(Stream input) : base(input){}
        public ByteReader(Stream input, Encoding encoding) : base(input, encoding){}
        public ByteReader(byte[] data) : base(new MemoryStream(data)){}
        public ByteReader(byte[] data, int length) : base(new MemoryStream(data, 0, length)){}
        public ByteReader(string path, FileMode fileMode) : base(new FileStream(path, fileMode)){}
        public void Seek(long offset, SeekOrigin seekOrigin = SeekOrigin.Begin) => BaseStream.Seek(offset, seekOrigin);
        public void Skip(long count) => BaseStream.Seek(count, SeekOrigin.Current);
        public long Tell() => BaseStream.Position;
        public long Size() => BaseStream.Length;
        public bool HasMemory(int size) => Size() - Tell() >= size;

        public void SetUnicode(bool unicode)
        {
            _unicode = unicode;
        }

        public bool IsUnicode()
        {
            return _unicode;
        }

        public byte PeekByte()
        {
            byte value = ReadByte();
            Skip(-1);
            return value;
        }

        public ushort PeekUInt16()
        {
            ushort value = ReadUShort();
            Skip(-2);
            return value;
        }

        public short PeekInt16()
        {
            short value = ReadShort();
            Skip(-2);
            return value;
        }

        public uint PeekUInt32()
        {
            uint value = ReadUInt();
            Skip(-4);
            return value;
        }

        public int PeekInt32()
        {
            int value = ReadInt();
            Skip(-4);
            return value;
        }

        public float PeekSingle()
        {
            float value = ReadFloat();
            Skip(-4);
            return value;
        }

        public ushort PeekUShort() => PeekUInt16();
        public short PeekShort() => PeekInt16();
        public uint PeekUInt() => PeekUInt32();
        public int PeekInt() => PeekInt32();
        public float PeekFloat() => PeekSingle();

        public ushort ReadUShort() => ReadUInt16();
        public short ReadShort() => ReadInt16();
        public uint ReadUInt() => ReadUInt32();
        public int ReadInt() => ReadInt32();
        public ulong ReadULong() => ReadUInt64();
        public long ReadLong() => ReadInt64();
        public float ReadFloat() => ReadSingle();
        public bool ReadBool() => ReadByte() == 1;
        public bool ReadBool4() => ReadInt() == 1;

        public string PeekHeader(int length = 4)
        {
            string header = ReadAscii(length);
            Skip(-length);
            return header;
        }

        public string ReadAscii(int length = -1)
        {
            string str = "";
            if (Tell() >= Size()) return str;
            if (length >= 0)
            {
                for (int i = 0; i < length; i++)
                {
                    str += Convert.ToChar(ReadByte());
                }
            }
            else
            {
                byte b = ReadByte();
                while (b != 0)
                {
                    str += Convert.ToChar(b);
                    if (Tell() >= Size()) break;
                    b = ReadByte();
                }
            }

            return str;
        }

        public string ReadAsciiStop(int length)
        {
            string str = "";
            long debut = Tell();
            if (length >= 0)
                for (int i = 0; i < length; i++)
                {
                    byte ch = ReadByte();
                    if (ch == 0) break;
                    str += Convert.ToChar(ch);
                }

            Seek(debut + length);
            return str;
        }

        public string ReadYunicode(int length = -1)
        {
            string str = "";
            if (Tell() >= Size()) return str;
            if (length >= 0)
                for (int i = 0; i < length; i++)
                    str += Convert.ToChar(ReadUShort());
            else
            {
                var b = ReadUShort();
                while (b != 0)
                {
                    str += Convert.ToChar(b);
                    if (!HasMemory(2)) break;
                    b = ReadUShort();
                }
            }

            return str;
        }

        public string ReadYunicodeStop(int length = -1)
        {
            string str = "";
            long debut = Tell();
            if (length >= 0)
                for (int i = 0; i < length; i++)
                {
                    ushort ch = ReadUShort();
                    if (ch == 0) break;
                    str += Convert.ToChar(ch);
                }

            Seek(debut + length * 2);
            return str;
        }

        public string ReadAutoYuniversal()
        {
            short len = ReadShort();
            Skip(2);

            if (_unicode)
                return ReadYunicode(len);
            else
                return ReadAscii(len);
        }

        public void SkipAutoYuniversal(int count = 1)
        {
            for (int i = 0; i < count; i++)
                Skip(2 + ReadShort() * (_unicode ? 2 : 1));
        }

        public string ReadYuniversal(int len = -1)
        {
            if (_unicode)
                return ReadYunicode(len); 
            else
                return ReadAscii(len);
        }

        public string ReadYuniversalStop(int len = -1)
        {
            if (_unicode)
                return ReadYunicodeStop(len); 
            else
                return ReadAsciiStop(len);
        }

        public Color ReadColor(int type = 0)
        {
            var r = ReadByte();
            var g = ReadByte();
            var b = ReadByte();
            var a = ReadByte();

            switch (type)
            {
                case 0:
                default: // RGBA
                    return Color.FromArgb(a, r, g, b);
                case 1: // BGRA
                    return Color.FromArgb(a, b, g, r);
            }
        }

        public override byte[] ReadBytes(int count = -1)
        {
            if (count == -1)
                return base.ReadBytes((int)Size());
            return base.ReadBytes(count);
        }

        public byte GetByteAt(long position, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            long orgPos = Tell();
            Seek(position, seekOrigin);
            byte output = ReadByte();
            Seek(orgPos);
            return output;
        }

        public string GetAsciiAt(long position, SeekOrigin seekOrigin = SeekOrigin.Begin, int length = -1)
        {
            long orgPos = Tell();
            Seek(position, seekOrigin);
            string output = ReadAscii(length);
            Seek(orgPos);
            return output;
        }
    }
}