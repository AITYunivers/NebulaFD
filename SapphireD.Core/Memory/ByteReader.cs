using System.Drawing;
using System.Text;

namespace SapphireD.Core.Memory
{
    public class ByteReader : BinaryReader
    {
        public ByteReader(Stream input) : base(input){}
        public ByteReader(Stream input, Encoding encoding) : base(input, encoding){}
        public ByteReader(byte[] data) : base(new MemoryStream(data)){}
        public ByteReader(string path, FileMode fileMode) : base(new FileStream(path, fileMode)){}
        public void Seek(long offset, SeekOrigin seekOrigin = SeekOrigin.Begin) => BaseStream.Seek(offset, seekOrigin);
        public void Skip(long count) => BaseStream.Seek(count, SeekOrigin.Current);
        public long Tell() => BaseStream.Position;
        public long Size() => BaseStream.Length;
        public bool HasMemory(int size) => Size() - Tell() >= size;

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
        public float ReadFloat() => ReadSingle();

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

        public string ReadWideString(int length = -1)
        {
            string str = "";
            if (Tell() >= Size()) return str;
            if (length >= 0)
                for (int i = 0; i < length; i++)
                    str += Convert.ToChar(ReadUInt16());
            else
            {
                var b = ReadUInt16();
                while (b != 0)
                {
                    str += Convert.ToChar(b);
                    if (Tell() >= Size()) break;
                    b = ReadUInt16();
                }
            }

            return str;
        }

        public string ReadWideStringStop(int length = -1)
        {
            string str = "";
            long debut = Tell();
            if (length >= 0)
                for (int i = 0; i < length; i++)
                {
                    short ch = ReadShort();
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
            if (SapDCore.Unicode)
                return ReadWideString(len);
            else
                return ReadAscii(len);
        }

        public string ReadYuniversal(int len = -1)
        {
            if (SapDCore._unicode == null)
            {
                Skip(1);
                SapDCore._unicode = ReadByte() == 0;
                Skip(-2);
            }

            if (SapDCore.Unicode)
                return ReadWideString(len); 
            else
                return ReadAscii(len);
        }

        public string ReadYuniversalStop(int len = -1)
        {
            if (SapDCore._unicode == null)
            {
                Skip(1);
                SapDCore._unicode = ReadByte() == 0;
                Skip(-2);
            }

            if (SapDCore.Unicode)
                return ReadWideStringStop(len); 
            else
                return ReadAsciiStop(len);
        }

        public Color ReadColor()
        {
            var r = ReadByte();
            var g = ReadByte();
            var b = ReadByte();
            var a = ReadByte();

            return Color.FromArgb(a, r, g, b);
        }

        public override byte[] ReadBytes(int count = -1)
        {
            if (count == -1)
                return base.ReadBytes((int)Size());
            return base.ReadBytes(count);
        }
    }
}