/*
 * tinflate.c -- tiny inflate library
 *
 * Written by Andrew Church <achurch@achurch.org>
 * This source code is public domain.
 *//*

// Ported to C# by Yunivers
namespace TinyInflate
{
    public static unsafe class TinyInflate
    {
        public struct DecompressionState
        {
            public enum _state
            {
                INITIAL = 0,
                PARTIAL_ZLIB_HEADER,
                HEADER,
                UNCOMPRESSED_LEN,
                UNCOMPRESSED_ILEN,
                UNCOMPRESSED_DATA,
                LITERAL_COUNT,
                DISTANCE_COUNT,
                CODELEN_COUNT,
                READ_CODE_LENGTHS,
                READ_LENGTHS,
                READ_LENGTHS_16,
                READ_LENGTHS_17,
                READ_LENGTHS_18,
                READ_SYMBOL,
                READ_LENGTH,
                READ_DISTANCE,
                READ_DISTANCE_EXTRA
            };

            public _state state;
            public byte[] in_ptr;
            public uint in_ptr_index;
            public byte[] in_top;
            public uint in_top_index;
            public byte[] out_base;
            public uint out_base_index;
            public ulong out_ofs;
            public ulong out_size;

            public ulong crc;
            public ulong bit_accum;
            public byte num_bits;
            public byte final;

            public byte first_byte;

            public byte block_type;
            public uint counter;
            public uint symbol;
            public uint last_value;
            public uint repeat_length;

            public uint len;
            public uint ilen;
            public uint nread;

            public short[] literal_table;// [288 * 2 - 2];
            public uint literal_table_index;
            public short[] distance_table;// [32 * 2 - 2];
            public uint distance_table_index;
            public uint literal_count;
            public uint distance_count;
            public uint codelen_count;
            public short[] codelen_table;// [19 * 2 - 2];
            public uint codelen_table_index;
            public byte[] literal_len;// [288];
            public uint literal_len_index;
            public byte[] distance_len;// [32];
            public uint distance_len_index;
            public byte[] codelen_len;// [19];
            public uint codelen_len_index;
        }

        static ulong[] crc32_table = new ulong[256]
        {
            0x00000000UL, 0x77073096UL, 0xEE0E612CUL, 0x990951BAUL, 0x076DC419UL,
            0x706AF48FUL, 0xE963A535UL, 0x9E6495A3UL, 0x0EDB8832UL, 0x79DCB8A4UL,
            0xE0D5E91EUL, 0x97D2D988UL, 0x09B64C2BUL, 0x7EB17CBDUL, 0xE7B82D07UL,
            0x90BF1D91UL, 0x1DB71064UL, 0x6AB020F2UL, 0xF3B97148UL, 0x84BE41DEUL,
            0x1ADAD47DUL, 0x6DDDE4EBUL, 0xF4D4B551UL, 0x83D385C7UL, 0x136C9856UL,
            0x646BA8C0UL, 0xFD62F97AUL, 0x8A65C9ECUL, 0x14015C4FUL, 0x63066CD9UL,
            0xFA0F3D63UL, 0x8D080DF5UL, 0x3B6E20C8UL, 0x4C69105EUL, 0xD56041E4UL,
            0xA2677172UL, 0x3C03E4D1UL, 0x4B04D447UL, 0xD20D85FDUL, 0xA50AB56BUL,
            0x35B5A8FAUL, 0x42B2986CUL, 0xDBBBC9D6UL, 0xACBCF940UL, 0x32D86CE3UL,
            0x45DF5C75UL, 0xDCD60DCFUL, 0xABD13D59UL, 0x26D930ACUL, 0x51DE003AUL,
            0xC8D75180UL, 0xBFD06116UL, 0x21B4F4B5UL, 0x56B3C423UL, 0xCFBA9599UL,
            0xB8BDA50FUL, 0x2802B89EUL, 0x5F058808UL, 0xC60CD9B2UL, 0xB10BE924UL,
            0x2F6F7C87UL, 0x58684C11UL, 0xC1611DABUL, 0xB6662D3DUL, 0x76DC4190UL,
            0x01DB7106UL, 0x98D220BCUL, 0xEFD5102AUL, 0x71B18589UL, 0x06B6B51FUL,
            0x9FBFE4A5UL, 0xE8B8D433UL, 0x7807C9A2UL, 0x0F00F934UL, 0x9609A88EUL,
            0xE10E9818UL, 0x7F6A0DBBUL, 0x086D3D2DUL, 0x91646C97UL, 0xE6635C01UL,
            0x6B6B51F4UL, 0x1C6C6162UL, 0x856530D8UL, 0xF262004EUL, 0x6C0695EDUL,
            0x1B01A57BUL, 0x8208F4C1UL, 0xF50FC457UL, 0x65B0D9C6UL, 0x12B7E950UL,
            0x8BBEB8EAUL, 0xFCB9887CUL, 0x62DD1DDFUL, 0x15DA2D49UL, 0x8CD37CF3UL,
            0xFBD44C65UL, 0x4DB26158UL, 0x3AB551CEUL, 0xA3BC0074UL, 0xD4BB30E2UL,
            0x4ADFA541UL, 0x3DD895D7UL, 0xA4D1C46DUL, 0xD3D6F4FBUL, 0x4369E96AUL,
            0x346ED9FCUL, 0xAD678846UL, 0xDA60B8D0UL, 0x44042D73UL, 0x33031DE5UL,
            0xAA0A4C5FUL, 0xDD0D7CC9UL, 0x5005713CUL, 0x270241AAUL, 0xBE0B1010UL,
            0xC90C2086UL, 0x5768B525UL, 0x206F85B3UL, 0xB966D409UL, 0xCE61E49FUL,
            0x5EDEF90EUL, 0x29D9C998UL, 0xB0D09822UL, 0xC7D7A8B4UL, 0x59B33D17UL,
            0x2EB40D81UL, 0xB7BD5C3BUL, 0xC0BA6CADUL, 0xEDB88320UL, 0x9ABFB3B6UL,
            0x03B6E20CUL, 0x74B1D29AUL, 0xEAD54739UL, 0x9DD277AFUL, 0x04DB2615UL,
            0x73DC1683UL, 0xE3630B12UL, 0x94643B84UL, 0x0D6D6A3EUL, 0x7A6A5AA8UL,
            0xE40ECF0BUL, 0x9309FF9DUL, 0x0A00AE27UL, 0x7D079EB1UL, 0xF00F9344UL,
            0x8708A3D2UL, 0x1E01F268UL, 0x6906C2FEUL, 0xF762575DUL, 0x806567CBUL,
            0x196C3671UL, 0x6E6B06E7UL, 0xFED41B76UL, 0x89D32BE0UL, 0x10DA7A5AUL,
            0x67DD4ACCUL, 0xF9B9DF6FUL, 0x8EBEEFF9UL, 0x17B7BE43UL, 0x60B08ED5UL,
            0xD6D6A3E8UL, 0xA1D1937EUL, 0x38D8C2C4UL, 0x4FDFF252UL, 0xD1BB67F1UL,
            0xA6BC5767UL, 0x3FB506DDUL, 0x48B2364BUL, 0xD80D2BDAUL, 0xAF0A1B4CUL,
            0x36034AF6UL, 0x41047A60UL, 0xDF60EFC3UL, 0xA867DF55UL, 0x316E8EEFUL,
            0x4669BE79UL, 0xCB61B38CUL, 0xBC66831AUL, 0x256FD2A0UL, 0x5268E236UL,
            0xCC0C7795UL, 0xBB0B4703UL, 0x220216B9UL, 0x5505262FUL, 0xC5BA3BBEUL,
            0xB2BD0B28UL, 0x2BB45A92UL, 0x5CB36A04UL, 0xC2D7FFA7UL, 0xB5D0CF31UL,
            0x2CD99E8BUL, 0x5BDEAE1DUL, 0x9B64C2B0UL, 0xEC63F226UL, 0x756AA39CUL,
            0x026D930AUL, 0x9C0906A9UL, 0xEB0E363FUL, 0x72076785UL, 0x05005713UL,
            0x95BF4A82UL, 0xE2B87A14UL, 0x7BB12BAEUL, 0x0CB61B38UL, 0x92D28E9BUL,
            0xE5D5BE0DUL, 0x7CDCEFB7UL, 0x0BDBDF21UL, 0x86D3D2D4UL, 0xF1D4E242UL,
            0x68DDB3F8UL, 0x1FDA836EUL, 0x81BE16CDUL, 0xF6B9265BUL, 0x6FB077E1UL,
            0x18B74777UL, 0x88085AE6UL, 0xFF0F6A70UL, 0x66063BCAUL, 0x11010B5CUL,
            0x8F659EFFUL, 0xF862AE69UL, 0x616BFFD3UL, 0x166CCF45UL, 0xA00AE278UL,
            0xD70DD2EEUL, 0x4E048354UL, 0x3903B3C2UL, 0xA7672661UL, 0xD06016F7UL,
            0x4969474DUL, 0x3E6E77DBUL, 0xAED16A4AUL, 0xD9D65ADCUL, 0x40DF0B66UL,
            0x37D83BF0UL, 0xA9BCAE53UL, 0xDEBB9EC5UL, 0x47B2CF7FUL, 0x30B5FFE9UL,
            0xBDBDF21CUL, 0xCABAC28AUL, 0x53B39330UL, 0x24B4A3A6UL, 0xBAD03605UL,
            0xCDD70693UL, 0x54DE5729UL, 0x23D967BFUL, 0xB3667A2EUL, 0xC4614AB8UL,
            0x5D681B02UL, 0x2A6F2B94UL, 0xB40BBE37UL, 0xC30C8EA1UL, 0x5A05DF1BUL,
            0x2D02EF8DUL
        };

        public static int tinflate_state_size()
        {
            return sizeof(DecompressionState);
        }

        public static long tinflate(byte[] compressed_data, long compressed_size,
                                    ref byte[] output_buffer, ulong output_size,
                                    ref ulong crc_ret)
        {
            DecompressionState state = new DecompressionState();
            ulong size = 0;
            int result = 0;
            output_buffer = new byte[output_size];

            if (compressed_data.Length == 0 || compressed_size < 0 || output_size < 0)
                return -1;

            state.state = DecompressionState._state.INITIAL;
            state.out_ofs = 0;
            state.crc = 0;
            state.bit_accum = 0;
            state.num_bits = 0;
            state.final = 0;
            state.literal_table = new short[288 * 2 - 2];
            state.distance_table = new short[32 * 2 - 2];
            state.codelen_table = new short[19 * 2 - 2];
            state.literal_len = new byte[288];
            state.distance_len = new byte[32];
            state.codelen_len = new byte[19];
            
            result = tinflate_partial(compressed_data, compressed_size,
                                      ref output_buffer, output_size,
                                      ref size, ref crc_ret, ref state);

            if (result != 0)
                return -1;

            return (long)size;
        }

        public static int tinflate_partial(byte[] compressed_data, long compressed_size,
                                           ref byte[] output_buffer, ulong output_size,
                                           ref ulong size_ret, ref ulong crc_ret,
                                           ref DecompressionState state)
        {
            if (compressed_data.Length == 0 | compressed_size < 0 || output_size < 0)
                return -1;

            state.in_ptr = compressed_data;
            state.in_ptr_index = 0;
            state.in_top_index = (uint)(state.in_ptr_index + compressed_size);
            state.out_base = output_buffer;
            state.out_base_index = 0;
            state.out_size = output_size;

            if (state.state == DecompressionState._state.INITIAL || state.state == DecompressionState._state.PARTIAL_ZLIB_HEADER)
            {
                uint zlib_header = 0;

                if (compressed_size == 0)
                    return 1;

                if (state.state == DecompressionState._state.INITIAL && compressed_size == 1)
                {
                    state.first_byte = state.in_ptr[0];
                    state.state = DecompressionState._state.PARTIAL_ZLIB_HEADER;
                    return 1;
                }

                if (state.state == DecompressionState._state.PARTIAL_ZLIB_HEADER)
                    zlib_header = (uint)state.first_byte << 8 | state.in_ptr[0];
                else
                    zlib_header = (uint)state.in_ptr[0] << 8 | state.in_ptr[1];

                if ((zlib_header & 0x8F00) == 0x0800 && zlib_header % 31 == 0)
                {
                    if ((zlib_header & 0x0020) != 0)
                        return -1;

                    state.in_ptr_index += state.state == DecompressionState._state.PARTIAL_ZLIB_HEADER ? 1u : 2u;
                }
                else if (state.state == DecompressionState._state.PARTIAL_ZLIB_HEADER)
                {
                    state.bit_accum = state.first_byte;
                    state.num_bits = 8;
                }

                state.state = DecompressionState._state.HEADER;
            }

            do
            {
                int res = tinflate_block(ref state);
                if (res != 0)
                    return res;
                if (state.out_ofs < 0)
                    return -1;
            } while (state.final == 0);

            if (size_ret != 0)
                size_ret = state.out_ofs;
            if (crc_ret != 0)
                crc_ret = state.crc;

            return 0;
        }

        public static int tinflate_block(ref DecompressionState state)
        {
            byte[] in_ptr = new byte[state.in_ptr.Length];
            Array.Copy(state.in_ptr, in_ptr, in_ptr.Length);
            uint in_ptr_index = state.in_ptr_index;
            byte[] in_top = new byte[state.in_top.Length];
            Array.Copy(state.in_top, in_top, in_top.Length);
            uint in_top_index = state.in_top_index;
            byte[] out_base = new byte[state.out_base.Length];
            Array.Copy(state.out_base, out_base, out_base.Length);
            uint out_base_index = state.out_base_index;
            ulong out_ofs = state.out_ofs;
            ulong out_size = state.out_size;
            ulong bit_accum = state.bit_accum;
            uint num_bits = state.num_bits;

            ulong icrc = ~state.crc;

            state.in_ptr = in_ptr;
            state.out_ofs = out_ofs;
            state.crc = ~icrc & 0xFFFFFFFFUL;
            state.bit_accum = bit_accum;
            state.num_bits = (byte)num_bits;
            state.state = DecompressionState._state.HEADER;
            return 0;

        out_of_data:
            state.in_ptr = in_ptr;
            state.out_ofs = out_ofs;
            state.crc = ~icrc & 0xFFFFFFFFUL;
            state.bit_accum = bit_accum;
            state.num_bits = (byte)num_bits;
            return 1;

        error_return:
            state.in_ptr = in_ptr;
            state.out_ofs = out_ofs;
            state.crc = ~icrc & 0xFFFFFFFFUL;
            state.bit_accum = bit_accum;
            state.num_bits = (byte)num_bits;
            return -1;
        }

        private static int GETBITS(uint n, ref ulong var, ref uint num_bits, ref int in_ptr_index, int in_top_index, ref ulong bit_accum)
        {
            uint __n = n;
            while (num_bits < __n)
            {
                if (in_ptr_index >= in_top_index)
                    return 1;
                bit_accum |= (ulong)in_ptr_index << (int)num_bits;
                num_bits += 8;
                in_ptr_index++;
            }
            var = bit_accum & (__n - 1);
            bit_accum >>= (int)__n;
            num_bits -= __n;
            return 0;
        }

        private static int GETHUFF(ref ulong var, uint[] table, uint num_bits, uint in_ptr_index, uint in_top_index, ref ulong bit_accum)
        {
            uint bits_used = 0;
            uint index = 0;
            for (;;)
            {
                if (num_bits <= bits_used)
                {
                    if (in_ptr_index >= in_top_index)
                        return 1;
                    bit_accum |= (ulong)in_ptr_index << (int)num_bits;
                    num_bits += 8;
                    in_ptr_index++;
                }
                index += (uint)(bit_accum >> (int)bits_used) & 1;
                bits_used++;
                if (table[index] >= 0)
                    break;
                index = ~table[index];
            }
            bit_accum >>= (int)bits_used;
            num_bits -= bits_used;
            var = table[index];
            return 0;
        }

        private static void PUTBYTE(byte _byte, ref ulong out_ofs, ulong out_size, ref byte[] out_base, ref ulong icrc)
        {
            byte __byte = _byte;
            if (out_ofs < out_size)
                out_base[out_ofs] = __byte;
            out_ofs++;
            UPDATECRC(__byte, ref icrc);
        }

        private static void PUTBYTESAFE(byte _byte, ref ulong out_ofs, ref byte[] out_base, ref ulong icrc)
        {
            byte __byte = _byte;
            out_base[out_ofs] = __byte;
            out_ofs++;
            UPDATECRC(__byte, ref icrc);
        }

        private static void UPDATECRC(byte _byte, ref ulong icrc)
        {
            byte __val = _byte;
            icrc = crc32_table[(icrc & 0xFF) ^ __val] ^ ((icrc >> 8) & 0xFFFFFFUL);
        }

        private static int CHECK_STATE(ref DecompressionState state, ref uint num_bits, ref int in_ptr_index, int in_top_index, ref ulong bit_accum,
                                       byte[] in_ptr, ref ulong out_ofs, ulong out_size, ref byte[] out_base, ref ulong icrc)
        {
            
            if (state.state == DecompressionState._state.UNCOMPRESSED_LEN)
                goto state_UNCOMPRESSED_LEN;

            ulong blocktype = state.block_type;
            int e = GETBITS(3, ref blocktype, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
            if (e != 0) return e;
            state.block_type = (byte)blocktype;
            state.final = (byte)(state.block_type & 1);
            state.block_type >>= 1;

            if (state.block_type == 3)
                return -1;

            if (state.block_type == 0)
            {
                num_bits = 0;
                state.state = DecompressionState._state.UNCOMPRESSED_LEN;
            state_UNCOMPRESSED_LEN:
                ulong statelen = state.len;
                e = GETBITS(16, ref statelen, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.len = (uint)statelen;
                state.state = DecompressionState._state.UNCOMPRESSED_ILEN;
            state_UNCOMPRESSED_ILEN:
                ulong stateilen = state.ilen;
                e = GETBITS(16, ref stateilen, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.ilen = (uint)stateilen;
                if (state.ilen != (~state.len & 0xFFFF))
                    return -1;
                state.nread = 0;
                state.state = DecompressionState._state.UNCOMPRESSED_DATA;
            state_UNCOMPRESSED_DATA:
                while (state.nread < state.len)
                {
                    if (in_ptr_index < in_top_index)
                        return 1;
                    PUTBYTE(in_ptr[in_ptr_index++], ref out_ofs, out_size, ref out_base, ref icrc);
                    state.nread++;
                }
                state.in_ptr = in_ptr;
                state.out_ofs = out_ofs;
                state.crc = ~icrc & 0xFFFFFFFFUL;
                state.bit_accum = bit_accum;
                state.num_bits = (byte)num_bits;
                state.state = DecompressionState._state.HEADER;
                return 0;
            }

            if (state.block_type == 2)
            {
                byte[] codelen_order = new byte[19]
                {
                            16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
                };

                state.state = DecompressionState._state.LITERAL_COUNT;
            state_LITERAL_COUNT:
                ulong literalcount = state.literal_count;
                e = GETBITS(5, ref literalcount, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.literal_count = (uint)literalcount;
                state.literal_count += 257;
                state.state = DecompressionState._state.DISTANCE_COUNT;
            state_DISTANCE_COUNT:
                ulong distancecount = state.distance_count;
                e = GETBITS(5, ref distancecount, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.distance_count = (uint)distancecount;
                state.distance_count += 1;
                state.state = DecompressionState._state.CODELEN_COUNT;
            state_CODELEN_COUNT:
                ulong codelencount = state.codelen_count;
                e = GETBITS(4, ref codelencount, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.codelen_count = (uint)codelencount;
                state.codelen_count += 4;

                state.counter = 0;
                state.state = DecompressionState._state.READ_CODE_LENGTHS;
            state_READ_CODE_LENGTHS:
                while (state.counter < state.codelen_count)
                {
                    ulong value1 = state.codelen_len[codelen_order[state.counter]];
                    e = GETBITS(3, ref value1, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                    if (e != 0) return e;
                    state.codelen_len[codelen_order[state.counter]] = (byte)value1;
                    state.counter++;
                }

                for (; state.counter < 19; state.counter++)
                    state.codelen_len[codelen_order[state.counter]] = 0;

                if (gen_huffman_table(19, state.codelen_len, 0, ref state.codelen_table) == 0)
                    return -1;

                uint repeat_count;

                state.last_value = 0;
                state.counter = 0;
                state.state = DecompressionState._state.READ_LENGTHS;
            state_READ_LENGTHS:
                repeat_count = 0;
                while (state.counter < state.literal_count + state.distance_count)
                {
                    if (repeat_count == 0)
                    {
                        uint[] codelentable = new uint[state.codelen_table.Length];
                        for (int i = 0; i < state.codelen_table.Length; i++)
                            codelentable[i] = (uint)state.codelen_table[i];
                        ulong statesymbol = state.symbol;
                        e = GETHUFF(ref statesymbol, codelentable, num_bits, (uint)in_ptr_index, (uint)in_top_index, ref bit_accum);
                        if (e != 0) return e;
                        state.symbol = (uint)statesymbol;
                        for (int i = 0; i < state.codelen_table.Length; i++)
                            state.codelen_table[i] = (short)codelentable[i];

                        if (state.symbol < 16)
                        {
                            state.last_value = state.symbol;
                            repeat_count = 1;
                        }
                        else if (state.symbol == 16)
                        {
                            state.state = DecompressionState._state.READ_LENGTHS_16;
                        state_READ_LENGTHS_16:
                            ulong repeatcount = repeat_count;
                            e = GETBITS(2, ref repeatcount, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                            if (e != 0) return e;
                            repeat_count = (uint)repeatcount;
                            repeat_count += 3;
                        }
                        else if (state.symbol == 17)
                        {
                            state.last_value = 0;
                            state.state = DecompressionState._state.READ_LENGTHS_17;
                        state_READ_LENGTHS_17:
                            ulong repeatcount = repeat_count;
                            e = GETBITS(3, ref repeatcount, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                            if (e != 0) return e;
                            repeat_count = (uint)repeatcount;
                            repeat_count += 3;
                        }
                        else
                        {
                            state.last_value = 0;
                            state.state = DecompressionState._state.READ_LENGTHS_18;
                        state_READ_LENGTHS_18:
                            ulong repeatcount = repeat_count;
                            e = GETBITS(7, ref repeatcount, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                            if (e != 0) return e;
                            repeat_count = (uint)repeatcount;
                            repeat_count += 11;
                        }
                    }

                    if (state.counter < state.literal_count)
                        state.literal_len[state.counter] = (byte)state.last_value;
                    else
                        state.distance_len[state.counter - state.literal_count] = (byte)state.last_value;

                    state.counter++;
                    repeat_count--;
                    state.state = DecompressionState._state.READ_LENGTHS;
                }

                if (gen_huffman_table(state.literal_count, state.literal_len, 0, ref state.literal_table) == 0 ||
                    gen_huffman_table(state.distance_count, state.distance_len, 1, ref state.distance_table) == 0)
                    return -1;
            }
            else
            {
                int next_free = 2;
                int i = 0;

                for (i = 0; i < 0x7E; i++)
                {
                    state.literal_table[i] = (short)~next_free;
                    next_free += 2;
                }

                for (; i < 0x96; i++)
                    state.literal_table[i] = (short)(i + (256 - 0x7E));

                for (; i < 0xFE; i++)
                {
                    state.literal_table[i] = (short)~next_free;
                    next_free += 2;
                }

                for (; i < 0x18E; i++)
                    state.literal_table[i] = (short)(i + (0 - 0xFE));

                for (; i < 0x196; i++)
                    state.literal_table[i] = (short)(i + (280 - 0x18E));

                for (; i < 0x1CE; i++)
                {
                    state.literal_table[i] = (short)~next_free;
                    next_free += 2;
                }

                for (; i < 0x23E; i++)
                    state.literal_table[i] = (short)(i + (144 - 0x1CE));

                for (i = 0; i < 0x1E; i++)
                    state.distance_table[i] = (short)~(i * 2 + 2);

                for (i = 0x1E; i < 0x3E; i++)
                    state.distance_table[i] = (short)(i - 0x1E);
            }

            for (; ; )
            {
                uint distance = 0;

                if (out_ofs < 0)
                    return -1;

                state.state = DecompressionState._state.READ_SYMBOL;
            state_READ_SYMBOL:
                uint[] literaltable = new uint[state.literal_table.Length];
                for (int i = 0; i < state.literal_table.Length; i++)
                    literaltable[i] = (uint)state.literal_table[i];
                ulong statesymbol = state.symbol;
                e = GETHUFF(ref statesymbol, literaltable, num_bits, (uint)in_ptr_index, (uint)in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.symbol = (uint)statesymbol;
                for (int i = 0; i < state.literal_table.Length; i++)
                    state.literal_table[i] = (short)literaltable[i];

                if (state.symbol < 256)
                {
                    PUTBYTE((byte)state.symbol, ref out_ofs, out_size, ref out_base, ref icrc);
                    continue;
                }

                if (state.symbol == 256)
                    break;

                if (state.symbol <= 264)
                    state.repeat_length = state.symbol - 257 + 3;
                else if (state.symbol <= 284)
                {
                    state.state = DecompressionState._state.READ_LENGTH;
                state_READ_LENGTH:
                    uint length_bits = (state.symbol - 261) / 4;
                    ulong repeatlength = state.repeat_length;
                    e = GETBITS(length_bits, ref repeatlength, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                    if (e != 0) return e;
                    state.repeat_length = (uint)repeatlength;
                    state.repeat_length += 3 + ((4 + ((state.symbol - 265) & 3)) << (int)length_bits);
                }
                else if (state.symbol == 285)
                    state.repeat_length = 258;
                else
                    return -1;

                state.state = DecompressionState._state.READ_DISTANCE;
            state_READ_DISTANCE:
                uint[] distancetable = new uint[state.distance_table.Length];
                for (int i = 0; i < state.distance_table.Length; i++)
                    distancetable[i] = (uint)state.distance_table[i];
                statesymbol = state.symbol;
                e = GETHUFF(ref statesymbol, distancetable, num_bits, (uint)in_ptr_index, (uint)in_top_index, ref bit_accum);
                if (e != 0) return e;
                state.symbol = (uint)statesymbol;
                for (int i = 0; i < state.distance_table.Length; i++)
                    state.distance_table[i] = (short)distancetable[i];

                if (state.symbol <= 3)
                    distance = state.symbol + 1;
                else if (state.symbol <= 29)
                {
                    state.state = DecompressionState._state.READ_DISTANCE_EXTRA;
                state_READ_DISTANCE_EXTRA:
                    uint distance_bits = (state.symbol - 2) / 2;
                    ulong dist = distance;
                    e = GETBITS(distance_bits, ref dist, ref num_bits, ref in_ptr_index, in_top_index, ref bit_accum);
                    if (e != 0) return e;
                    distance = (uint)dist;
                    distance += 1 + ((2 + (state.symbol & 1)) << (int)distance_bits);
                }
                else
                    return -1;

                if (out_ofs < distance)
                    return -1;

                uint repeat_length = state.repeat_length;
                uint overflow = 0;

                if (out_ofs + repeat_length > out_size)
                {
                    if (out_ofs > out_size)
                        overflow = repeat_length;
                    else
                        overflow = (uint)(out_ofs - out_size) + repeat_length;
                    repeat_length -= overflow;
                }

                for (; repeat_length > 0; repeat_length--)
                    PUTBYTESAFE(out_base[out_ofs - distance], ref out_ofs, ref out_base, ref icrc);

                out_ofs += overflow;
            }
            return 0;
        }

        private static int gen_huffman_table(uint symbols, byte[] lengths,
                                             int allow_no_symbols, ref short[] table)
        {
            ushort[] length_count = new ushort[16];
            ushort total_count = 0;
            ushort[] first_code = new ushort[16];
            uint index = 0;

            uint i = 0;

            for (i = 0; i < 16; i++)
                length_count[i] = 0;

            for (i = 0; i < symbols; i++)
                if (lengths[i] > 0)
                    length_count[lengths[i]]++;

            total_count = 0;
            for (i = 1; i < 16; i++)
                total_count += length_count[i];

            if (total_count == 0)
                return allow_no_symbols;
            else if (total_count == 1)
            {
                for (i = 0; i < symbols; i++)
                    if (lengths[i] != 0)
                        table[0] = table[1] = 1;
                return 1;
            }

            first_code[0] = 0;
            for (i = 1; i < 16; i++)
            {
                first_code[i] = (ushort)((first_code[i - 1] + length_count[i - 1]) << 1);
                if (first_code[i] + length_count[i] > 1 << (int)i)
                    return 0;
            }

            if (first_code[15] + length_count[15] != 1 << 15)
                return 0;

            index = 0;
            for (i = 1; i < 16; i++)
            {
                uint code_limit = 1U << (int)i;
                uint next_code = (uint)first_code[i] + length_count[i];
                uint next_index = index + (code_limit - first_code[i]);
                uint j = 0;

                for (j = 0; j < symbols; j++)
                    if (lengths[j] == i)
                        table[index++] = j;

                for (j = next_code; j < code_limit; j++)
                {
                    table[index++] = (short)~next_index;
                    next_index += 2;
                }
            }

            return 1;
        }
    }
}
*/