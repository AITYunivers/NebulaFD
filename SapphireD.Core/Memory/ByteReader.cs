using System.Buffers.Binary;
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
        public void Seek(Int64 offset, SeekOrigin seekOrigin = SeekOrigin.Begin)=>BaseStream.Seek(offset, seekOrigin);
        public void Skip(Int64 count)=>BaseStream.Seek(count, SeekOrigin.Current);
        public Int64 Tell()=>BaseStream.Position;
        public Int64 Size()=>BaseStream.Length;
        public bool HasMemory(int size)=>Size() - Tell() >= size;



        public UInt16 PeekUInt16()
        {
            UInt16 value = ReadUInt16();
            Seek(-2, SeekOrigin.Current);
            return value;
        }
        public byte PeekByte()
        {
            byte value = ReadByte();
            Seek(-1, SeekOrigin.Current);
            return value;
        }
        public Int16 PeekInt16()
        {
            Int16 value = ReadInt16();
            Seek(-2, SeekOrigin.Current);
            return value;
        }


        public Int32 PeekInt32()
        {
            Int32 value = ReadInt32();
            Seek(-4, SeekOrigin.Current);
            return value;
        }

        public short ReadShort() => ReadInt16();
        public ushort ReadUShort() => ReadUInt16();
        public int ReadInt() => ReadInt32();
        public uint ReadUInt() => ReadUInt32();
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

        public string ReadYuniversal(int len = -1)
        {
            if (SapDCore.Unicode)
                return ReadWideString(len); 
            else
                return ReadAscii(len);
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
            if (count == -1) return base.ReadBytes((int)this.Size());
            return base.ReadBytes(count);
        }
    }
}