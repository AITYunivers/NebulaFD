/*
 * tinflate.c -- tiny inflate library
 *
 * Written by Andrew Church <achurch@achurch.org>
 * Re-written for C++ by Lucas "LAK132" Kleiss <https://github.com/LAK132>
 * Re-written for C# by Yunivers for Nebula <https://github.com/AITYunivers/NebulaFD>
 * 
 * This source code is public domain.
 */

using Nebula.Core.Memory;

namespace Nebula.Core
{
    public static class TinyInflate
    {

        public class DecompressionState
        {
            public ByteReader Data;

            public State State = State.Initial;
            public ulong CRC = 0;
            public uint BitAccum = 0;
            public uint NumBits = 0;
            public bool Final = false;
            public bool Anaconda = false;

            public byte FirstByte;
            public byte BlockType;
            public uint Counter;
            public uint Symbol;
            public uint LastValue;
            public uint RepeatLength;
            public uint RepeatCount;
            public uint Distance;

            public uint Length;
            public uint ILength;
            public uint NRead;

            public short[] LiteralTable = new short[0x23E]; // (288 * 2) - 2
            public short[] DistanceTable = new short[0x3E]; // (32 * 2) - 2;
            public uint LiteralCount;
            public uint DistanceCount;
            public uint CodeLenCount;
            public short[] CodeLenTable = new short[0x24]; // (19 * 2) - 2
            public byte[] LiteralLen = new byte[0x120]; // 288
            public byte[] DistanceLen = new byte[0x20]; // 32
            public byte[] CodeLenLen = new byte[0x13]; // 19

            public bool PeekBits(uint bits, ref byte output)
            {
                output = default!;
                while (NumBits < bits)
                {
                    if (!Data.HasMemory(1)) return true;
                    BitAccum |= ((uint)Data.ReadByte()) << (int)NumBits;
                    NumBits += 8;
                }
                output = (byte)(BitAccum & ((1UL << (int)bits) - 1));
                return false;
            }

            public bool GetBits(uint bits, out byte output)
            {
                uint outU = 0;
                bool outB = GetBits(bits, out outU);
                output = (byte)(outU);
                return outB;
            }

            public bool GetBits(uint bits, out uint output)
            {
                output = default!;
                while (NumBits < bits)
                {
                    if (!Data.HasMemory(1)) return true;
                    BitAccum |= ((uint)Data.ReadByte()) << (int)NumBits;
                    NumBits += 8;
                }
                output = (uint)(BitAccum & ((1UL << (int)bits) - 1));
                BitAccum >>= (int)bits;
                NumBits -= bits;
                return false;
            }

            public bool GetHuff(short[] table, out byte output)
            {
                uint outU = 0;
                bool outB = GetHuff(table, out outU);
                output = (byte)(outU);
                return outB;
            }

            public bool GetHuff(short[] table, out uint output)
            {
                output = default!;
                uint bitsUsed = 0;
                uint index = 0;
                while (true)
                {
                    if (NumBits <= bitsUsed)
                    {
                        if (!Data.HasMemory(1)) return true;
                        BitAccum |= ((uint)Data.ReadByte()) << (int)NumBits;
                        NumBits += 8;
                    }
                    index += (BitAccum >> (int)bitsUsed) & 1;
                    bitsUsed++;
                    if (table[index] >= 0) break;
                    index = (uint)~(table[index]);
                }
                BitAccum >>= (int)bitsUsed;
                NumBits -= bitsUsed;
                output = (uint)table[index];
                return false;
            }
        }

        public static Error Tinflate(byte[] input, out List<byte> output, out ulong CRC)
        {
            return Tinflate(new ByteReader(input), out output, new(), out CRC);
        }

        public static Error Tinflate(ByteReader input, out List<byte> output, out ulong CRC)
        {
            return Tinflate(input, out output, new(), out CRC);
        }

        public static Error Tinflate(byte[] input, out List<byte> output, DecompressionState state, out ulong CRC)
        {
            return Tinflate(new ByteReader(input), out output, state, out CRC);
        }

        public static Error Tinflate(ByteReader input, out List<byte> output, DecompressionState state, out ulong CRC)
        {
            state.Data = input;
            output = new List<byte>();
            CRC = 0;

            Error er = Error.Unknown;
            if ((er = TinflateHeader(state)) != Error.OK)
                return er;

            do
            {
                if ((er = TinflateBlock(ref output, state)) != Error.OK)
                    return er;
            } while (!state.Final);

            CRC = state.CRC;
            return Error.OK;
        }

        public static Error TinflateHeader(DecompressionState state)
        {
            long size = state.Data.Size() - state.Data.Tell();
            if (size <= 0) return Error.OutOfData;

            if (state.State == State.Initial || state.State == State.PartialZlibHeader)
            {
                ushort zlibHeader;
                if (size == 0)
                    return Error.OutOfData;

                if (state.State == State.Initial && size == 1)
                {
                    state.FirstByte = state.Data.ReadByte();
                    state.State = State.PartialZlibHeader;
                    return Error.OutOfData;
                }

                if (state.State == State.PartialZlibHeader)
                    zlibHeader = (ushort)((state.FirstByte << 8) | state.Data.GetByteAt(0));
                else
                    zlibHeader = (ushort)((state.Data.GetByteAt(0) << 8) | state.Data.GetByteAt(1));

                if ((zlibHeader & 0x8F00) == 0x0800 && (zlibHeader % 31) == 0)
                {
                    if ((zlibHeader & 0x0020) != 0)
                        return Error.CustomDictionary;

                    state.Data.Seek(state.State == State.PartialZlibHeader ? 1 : 2);
                    if (state.Data.Tell() >= state.Data.Size())
                        return Error.OutOfData;
                }
                else if (state.State == State.PartialZlibHeader)
                {
                    state.BitAccum = state.FirstByte;
                    state.NumBits = 8;
                }
                state.State = State.Header;
            }

            return Error.OK;
        }

        private static void _push(byte val, ref ulong iCRC, ref List<byte> output)
        {
            output.Add(val);
            iCRC = CRCTable[(iCRC & 0xFF) ^ val] ^ ((iCRC >> 8) & 0xFFFFFFUL);
        }

        private static Error _outOfData(ref DecompressionState state, ref ulong iCRC)
        {
            state.CRC = ~iCRC & 0xFFFFFFFFUL;
            return Error.OutOfData;
        }

        public static Error TinflateBlock(ref List<byte> output, DecompressionState state)
        {
            ulong iCRC = ~state.CRC;

            byte[] codelenOrder = new byte[] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };
            byte[] codelenOrderAnaconda = new byte[] { 18, 17, 16, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            if (state.State == State.Initial ||
                state.State == State.PartialZlibHeader)
                return Error.InvalidState;

            if (state.State != State.Header)
                throw new NotImplementedException("Starting from non-header states is unsupported!");

            if (state.Anaconda)
            {
                if (state.GetBits(4, out state.BlockType)) return _outOfData(ref state, ref iCRC);
                state.Final = state.BlockType >> 3 != 0;
                state.BlockType &= 0x7;

                if (state.BlockType == 7)
                    state.BlockType = 0;
                else if (state.BlockType == 5)
                    state.BlockType = 1;
                else if (state.BlockType == 6)
                    state.BlockType = 2;
                else
                    state.BlockType = 3;
            }
            else
            {
                if (state.GetBits(3, out state.BlockType)) return _outOfData(ref state, ref iCRC);
                state.Final = (state.BlockType & 0x1) != 0;
                state.BlockType >>= 1;
            }

            if (state.BlockType == 3)
            {
                state.CRC = ~iCRC & 0xFFFFFFFFUL;
                return Error.InvalidBlockCode;
            }

            if (state.BlockType == 0)
            {
                state.NumBits = 0;
                state.State = State.UncompressedLen;
                if (state.GetBits(16, out state.Length)) return _outOfData(ref state, ref iCRC);
                state.State = State.UncompressedILen;

                if (!state.Anaconda)
                {
                    if (state.GetBits(16, out state.ILength)) return _outOfData(ref state, ref iCRC);

                    if (state.ILength != (~state.Length & 0xFFFF))
                    {
                        state.CRC = ~iCRC & 0xFFFFFFFFUL;
                        return Error.CorruptStream;
                    }
                }

                state.NRead = 0;
                state.State = State.UncompressedData;

                while (state.NRead < state.Length)
                {
                    if (state.Data.Tell() >= state.Data.Size()) return _outOfData(ref state, ref iCRC);
                    _push(state.Data.ReadByte(), ref iCRC, ref output);
                    state.NRead++;
                }
                state.CRC = ~iCRC & 0xFFFFFFFFUL;
                state.State = State.Header;
                return Error.OK;
            }

            if (state.BlockType == 2)
            {
                state.State = State.LiteralCount;
                if (state.GetBits(5, out state.LiteralCount)) return _outOfData(ref state, ref iCRC);
                state.LiteralCount += 257;

                state.State = State.DistanceCount;
                if (state.GetBits(5, out state.DistanceCount)) return _outOfData(ref state, ref iCRC);
                state.DistanceCount++;

                state.State = State.CodeLenCount;
                if (state.GetBits(4, out state.CodeLenCount)) return _outOfData(ref state, ref iCRC);
                state.CodeLenCount += 4;
                state.Counter = 0;

                state.State = State.ReadCodeLengths;
                if (state.Anaconda)
                {
                    for (; state.Counter < state.CodeLenCount; ++state.Counter)
                        if (state.GetBits(3, out state.CodeLenLen[codelenOrderAnaconda[state.Counter]]))
                            return _outOfData(ref state, ref iCRC);

                    for (; state.Counter < 19; ++state.Counter)
                        state.CodeLenLen[codelenOrderAnaconda[state.Counter]] = 0;
                }
                else
                {
                    for (; state.Counter < state.CodeLenCount; ++state.Counter)
                        if (state.GetBits(3, out state.CodeLenLen[codelenOrder[state.Counter]]))
                            return _outOfData(ref state, ref iCRC);

                    for (; state.Counter < 19; ++state.Counter)
                        state.CodeLenLen[codelenOrder[state.Counter]] = 0;
                }

                if (GenHuffmanTable(19, state.CodeLenLen, false, ref state.CodeLenTable) != Error.OK)
                {
                    state.CRC = ~iCRC & 0xFFFFFFFFUL;
                    return Error.HuffmanTableGenFailed;
                }

                state.RepeatCount = 0;
                state.LastValue = 0;
                state.Counter = 0;

                state.State = State.ReadLengths;
                while (state.Counter < state.LiteralCount + state.DistanceCount)
                {
                    if (state.RepeatCount == 0)
                    {
                        if (state.GetHuff(state.CodeLenTable, out state.Symbol))
                            return _outOfData(ref state, ref iCRC);

                        if (state.Symbol < 16)
                        {
                            state.LastValue = state.Symbol;
                            state.RepeatCount = 1;
                        }
                        else if (state.Symbol == 16)
                        {
                            state.State = State.ReadLengths16;
                            if (state.GetBits(2, out state.RepeatCount))
                                return _outOfData(ref state, ref iCRC);
                            state.RepeatCount += 3;
                        }
                        else if (state.Symbol == 17)
                        {
                            state.LastValue = 0;

                            state.State = State.ReadLengths17;
                            if (state.GetBits(3, out state.RepeatCount))
                                return _outOfData(ref state, ref iCRC);
                            state.RepeatCount += 3;
                        }
                        else if (state.Symbol != 0)
                        {
                            state.LastValue = 0;

                            state.State = State.ReadLengths18;
                            if (state.GetBits(7, out state.RepeatCount))
                                return _outOfData(ref state, ref iCRC);
                            state.RepeatCount += 11;
                        }
                    }

                    if (state.Counter < state.LiteralCount)
                        state.LiteralLen[state.Counter] = (byte)state.LastValue;
                    else
                        state.DistanceLen[state.Counter - state.LiteralCount] = (byte)state.LastValue;

                    state.Counter++;
                    state.RepeatCount--;
                    state.State = State.ReadLengths;
                }

                if (GenHuffmanTable(state.LiteralCount, state.LiteralLen, false, ref state.LiteralTable) != Error.OK ||
                    GenHuffmanTable(state.DistanceCount, state.DistanceLen, true, ref state.DistanceTable) != Error.OK)
                {
                    state.CRC = ~iCRC & 0xFFFFFFFFUL;
                    return Error.HuffmanTableGenFailed;
                }
            }
            else
            {
                int nextFree = 2;
                int i;

                for (i = 0; i < 0x7E; ++i)
                {
                    state.LiteralTable[i] = (short)~nextFree;
                    nextFree += 2;
                }

                for (; i < 0x96; ++i)
                    state.LiteralTable[i] = (short)(i + (256 - 0x7E));

                for (; i < 0xFE; ++i)
                {
                    state.LiteralTable[i] = (short)~nextFree;
                    nextFree += 2;
                }

                for (; i < 0x18E; ++i)
                    state.LiteralTable[i] = (short)(i + (0 - 0xFE));

                for (; i < 0x196; ++i)
                    state.LiteralTable[i] = (short)(i + (280 - 0x18E));

                for (; i < 0x1CE; ++i)
                {
                    state.LiteralTable[i] = (short)~nextFree;
                    nextFree += 2;
                }

                for (; i < 0x23E; ++i)
                    state.LiteralTable[i] = (short)(i + (144 - 0x1CE));

                for (i = 0; i < 0x1E; ++i)
                    state.DistanceTable[i] = (short)~(i * 2 + 2);

                for (i = 0x1E; i < 0x3E; ++i)
                    state.DistanceTable[i] = (short)(i - 0x1E);
            }

            while (true)
            {
                state.State = State.ReadSymbol;
                if (state.GetHuff(state.LiteralTable, out state.Symbol))
                    return _outOfData(ref state, ref iCRC);

                if (state.Symbol < 256)
                {
                    _push((byte)state.Symbol, ref iCRC, ref output);
                    continue;
                }

                if (state.Symbol == 256)
                    break;

                if (state.Symbol <= 264)
                {
                    state.RepeatLength = (state.Symbol - 257) + 3;
                }
                else if (state.Symbol <= 284)
                {
                    state.State = State.ReadLength;

                    uint lengthBits = (state.Symbol - 261) / 4;
                    if (state.GetBits(lengthBits, out state.RepeatLength))
                        return _outOfData(ref state, ref iCRC);
                    state.RepeatLength += 3 + ((4 + ((state.Symbol - 265) & 3)) << (int)lengthBits);
                }
                else if (state.Symbol == 285)
                {
                    state.RepeatLength = 258;
                }
                else
                {
                    state.CRC = ~iCRC & 0xFFFFFFFFUL;
                    return Error.InvalidSymbol;
                }

                state.State = State.ReadDistance;
                if (state.GetHuff(state.DistanceTable, out state.Symbol))
                    return _outOfData(ref state, ref iCRC);

                if (state.Symbol <= 3)
                {
                    state.Distance = state.Symbol + 1;
                }
                else if (state.Symbol <= 29)
                {
                    state.State = State.ReadDistanceExtra;
                    uint distanceBits = (state.Symbol - 2) / 2;
                    if (state.GetBits(distanceBits, out state.Distance))
                        return _outOfData(ref state, ref iCRC);
                    state.Distance += 1 + ((2 + (state.Symbol & 1)) << (int)distanceBits);
                }
                else
                {
                    state.CRC = ~iCRC & 0xFFFFFFFFUL;
                    return Error.InvalidSymbol;
                }

                if (state.Distance > output.Count)
                {
                    state.CRC = ~iCRC & 0xFFFFFFFFUL;
                    return Error.InvalidDistance;
                }

                while (state.RepeatLength-- > 0)
                    _push(output[(int)(output.Count - state.Distance)], ref iCRC, ref output);
                state.RepeatLength = 0;
            }

            state.CRC = ~iCRC & 0xFFFFFFFFUL;
            state.State = State.Header;
            return Error.OK;
        }

        public static Error GenHuffmanTable(uint symbols, byte[] lengths, bool allowNoSymbols, ref short[] table)
        {
            ushort[] lengthCount = new ushort[16];
            ushort totalCount = 0;
            ushort[] firstCode = new ushort[16];

            for (uint i = 0; i < symbols; ++i)
                if (lengths[i] > 0)
                    lengthCount[lengths[i]]++;

            for (uint i = 1; i < 16; ++i)
                totalCount += lengthCount[i];

            if (totalCount == 0)
                return allowNoSymbols ? Error.OK : Error.NoSymbols;
            else if (totalCount == 1)
            {
                for (uint i = 0; i < symbols; ++i)
                    if (lengths[i] != 0)
                        table[0] = table[1] = (short)i;
                return Error.OK;
            }

            firstCode[0] = 0;
            for (uint i = 1; i < 16; ++i)
            {
                firstCode[i] = (ushort)((firstCode[i - 1] + lengthCount[i - 1]) << 1);
                if (firstCode[i] + lengthCount[i] > 1 << (int)i)
                    return Error.TooManySymbols;
            }
            if (firstCode[15] + lengthCount[15] != 1 << 15)
                return Error.ImcompleteTree;

            for (uint index = 0, i = 1; i < 16; ++i)
            {
                uint codeLimit = 1U << (int)i;
                uint nextCode = (uint)firstCode[i] + lengthCount[i];
                uint nextIndex = index + (codeLimit - firstCode[i]);

                for (uint j = 0; j < symbols; ++j)
                    if (lengths[j] == i)
                        table[index++] = (short)j;

                for (uint j = nextCode; j < codeLimit; ++j)
                {
                    table[index++] = (short)~nextIndex;
                    nextIndex += 2;
                }
            }

            return Error.OK;
        }

        public enum State
        {
            Initial,
            PartialZlibHeader,
            Header,
            UncompressedLen,
            UncompressedILen,
            UncompressedData,
            LiteralCount,
            DistanceCount,
            CodeLenCount,
            ReadCodeLengths,
            ReadLengths,
            ReadLengths16,
            ReadLengths17,
            ReadLengths18,
            ReadSymbol,
            ReadLength,
            ReadDistance,
            ReadDistanceExtra
        }

        public enum Error
        {
            OK,

            NoData,

            InvalidParameter,
            CustomDictionary,

            InvalidState,
            InvalidBlockCode,
            OutOfData,
            CorruptStream,
            HuffmanTableGenFailed,
            InvalidSymbol,
            InvalidDistance,

            NoSymbols,
            TooManySymbols,
            ImcompleteTree,

            Unknown
        }

        public static string GetErrorName(Error error)
        {
            return error switch
            {
                Error.OK => "OK",
                Error.NoData => "No Data",
                Error.InvalidParameter => "Invalid Parameter",
                Error.CustomDictionary => "Custom Dictionary",
                Error.InvalidState => "Invalid State",
                Error.InvalidBlockCode => "Invalid Block Code",
                Error.OutOfData => "Out Of Data",
                Error.CorruptStream => "Corrupt Stream",
                Error.HuffmanTableGenFailed => "Huffman Table Generation Failed",
                Error.InvalidSymbol => "Invalid Symbol",
                Error.InvalidDistance => "Invalid Distance",
                Error.NoSymbols => "No Symbols",
                Error.TooManySymbols => "Too Many Symbols",
                Error.ImcompleteTree => "Incomplete Tree",
                _ => "Unknown Error"
            };
        }

        static readonly uint[] CRCTable = new uint[]
        {
            0x00000000U, 0x77073096U, 0xEE0E612CU, 0x990951BAU,
            0x076DC419U, 0x706AF48FU, 0xE963A535U, 0x9E6495A3U,
            0x0EDB8832U, 0x79DCB8A4U, 0xE0D5E91EU, 0x97D2D988U,
            0x09B64C2BU, 0x7EB17CBDU, 0xE7B82D07U, 0x90BF1D91U,
            0x1DB71064U, 0x6AB020F2U, 0xF3B97148U, 0x84BE41DEU,
            0x1ADAD47DU, 0x6DDDE4EBU, 0xF4D4B551U, 0x83D385C7U,
            0x136C9856U, 0x646BA8C0U, 0xFD62F97AU, 0x8A65C9ECU,
            0x14015C4FU, 0x63066CD9U, 0xFA0F3D63U, 0x8D080DF5U,
            0x3B6E20C8U, 0x4C69105EU, 0xD56041E4U, 0xA2677172U,
            0x3C03E4D1U, 0x4B04D447U, 0xD20D85FDU, 0xA50AB56BU,
            0x35B5A8FAU, 0x42B2986CU, 0xDBBBC9D6U, 0xACBCF940U,
            0x32D86CE3U, 0x45DF5C75U, 0xDCD60DCFU, 0xABD13D59U,
            0x26D930ACU, 0x51DE003AU, 0xC8D75180U, 0xBFD06116U,
            0x21B4F4B5U, 0x56B3C423U, 0xCFBA9599U, 0xB8BDA50FU,
            0x2802B89EU, 0x5F058808U, 0xC60CD9B2U, 0xB10BE924U,
            0x2F6F7C87U, 0x58684C11U, 0xC1611DABU, 0xB6662D3DU,
            0x76DC4190U, 0x01DB7106U, 0x98D220BCU, 0xEFD5102AU,
            0x71B18589U, 0x06B6B51FU, 0x9FBFE4A5U, 0xE8B8D433U,
            0x7807C9A2U, 0x0F00F934U, 0x9609A88EU, 0xE10E9818U,
            0x7F6A0DBBU, 0x086D3D2DU, 0x91646C97U, 0xE6635C01U,
            0x6B6B51F4U, 0x1C6C6162U, 0x856530D8U, 0xF262004EU,
            0x6C0695EDU, 0x1B01A57BU, 0x8208F4C1U, 0xF50FC457U,
            0x65B0D9C6U, 0x12B7E950U, 0x8BBEB8EAU, 0xFCB9887CU,
            0x62DD1DDFU, 0x15DA2D49U, 0x8CD37CF3U, 0xFBD44C65U,
            0x4DB26158U, 0x3AB551CEU, 0xA3BC0074U, 0xD4BB30E2U,
            0x4ADFA541U, 0x3DD895D7U, 0xA4D1C46DU, 0xD3D6F4FBU,
            0x4369E96AU, 0x346ED9FCU, 0xAD678846U, 0xDA60B8D0U,
            0x44042D73U, 0x33031DE5U, 0xAA0A4C5FU, 0xDD0D7CC9U,
            0x5005713CU, 0x270241AAU, 0xBE0B1010U, 0xC90C2086U,
            0x5768B525U, 0x206F85B3U, 0xB966D409U, 0xCE61E49FU,
            0x5EDEF90EU, 0x29D9C998U, 0xB0D09822U, 0xC7D7A8B4U,
            0x59B33D17U, 0x2EB40D81U, 0xB7BD5C3BU, 0xC0BA6CADU,
            0xEDB88320U, 0x9ABFB3B6U, 0x03B6E20CU, 0x74B1D29AU,
            0xEAD54739U, 0x9DD277AFU, 0x04DB2615U, 0x73DC1683U,
            0xE3630B12U, 0x94643B84U, 0x0D6D6A3EU, 0x7A6A5AA8U,
            0xE40ECF0BU, 0x9309FF9DU, 0x0A00AE27U, 0x7D079EB1U,
            0xF00F9344U, 0x8708A3D2U, 0x1E01F268U, 0x6906C2FEU,
            0xF762575DU, 0x806567CBU, 0x196C3671U, 0x6E6B06E7U,
            0xFED41B76U, 0x89D32BE0U, 0x10DA7A5AU, 0x67DD4ACCU,
            0xF9B9DF6FU, 0x8EBEEFF9U, 0x17B7BE43U, 0x60B08ED5U,
            0xD6D6A3E8U, 0xA1D1937EU, 0x38D8C2C4U, 0x4FDFF252U,
            0xD1BB67F1U, 0xA6BC5767U, 0x3FB506DDU, 0x48B2364BU,
            0xD80D2BDAU, 0xAF0A1B4CU, 0x36034AF6U, 0x41047A60U,
            0xDF60EFC3U, 0xA867DF55U, 0x316E8EEFU, 0x4669BE79U,
            0xCB61B38CU, 0xBC66831AU, 0x256FD2A0U, 0x5268E236U,
            0xCC0C7795U, 0xBB0B4703U, 0x220216B9U, 0x5505262FU,
            0xC5BA3BBEU, 0xB2BD0B28U, 0x2BB45A92U, 0x5CB36A04U,
            0xC2D7FFA7U, 0xB5D0CF31U, 0x2CD99E8BU, 0x5BDEAE1DU,
            0x9B64C2B0U, 0xEC63F226U, 0x756AA39CU, 0x026D930AU,
            0x9C0906A9U, 0xEB0E363FU, 0x72076785U, 0x05005713U,
            0x95BF4A82U, 0xE2B87A14U, 0x7BB12BAEU, 0x0CB61B38U,
            0x92D28E9BU, 0xE5D5BE0DU, 0x7CDCEFB7U, 0x0BDBDF21U,
            0x86D3D2D4U, 0xF1D4E242U, 0x68DDB3F8U, 0x1FDA836EU,
            0x81BE16CDU, 0xF6B9265BU, 0x6FB077E1U, 0x18B74777U,
            0x88085AE6U, 0xFF0F6A70U, 0x66063BCAU, 0x11010B5CU,
            0x8F659EFFU, 0xF862AE69U, 0x616BFFD3U, 0x166CCF45U,
            0xA00AE278U, 0xD70DD2EEU, 0x4E048354U, 0x3903B3C2U,
            0xA7672661U, 0xD06016F7U, 0x4969474DU, 0x3E6E77DBU,
            0xAED16A4AU, 0xD9D65ADCU, 0x40DF0B66U, 0x37D83BF0U,
            0xA9BCAE53U, 0xDEBB9EC5U, 0x47B2CF7FU, 0x30B5FFE9U,
            0xBDBDF21CU, 0xCABAC28AU, 0x53B39330U, 0x24B4A3A6U,
            0xBAD03605U, 0xCDD70693U, 0x54DE5729U, 0x23D967BFU,
            0xB3667A2EU, 0xC4614AB8U, 0x5D681B02U, 0x2A6F2B94U,
            0xB40BBE37U, 0xC30C8EA1U, 0x5A05DF1BU, 0x2D02EF8DU
        };
    }
}