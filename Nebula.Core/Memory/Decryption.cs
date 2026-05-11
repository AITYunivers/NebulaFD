namespace Nebula.Core.Memory
{
    public static class Decryption
    {
        public static byte[] DecryptionKey = new byte[0];

        public static byte[] KeyString(string str)
        {
            var result = new List<byte>();
            foreach (char code in str)
            {
                if ((code & 0xFF) != 0)
                    result.Add((byte)(code & 0xFF));

                if (((code >> 8) & 0xFF) != 0)
                    result.Add((byte)((code >> 8) & 0xFF));
            }
            return result.ToArray();
        }

        public static byte[] MakeKeyCombined(byte[] data)
        {
            int dataLen = data.Length;

            byte checksum = 0;

            for (int i = 0; i < dataLen; i++)
                checksum += data[i];

            Array.Resize(ref data, 128);
            Array.Resize(ref data, 256);
            if (dataLen < 255)
                data[dataLen + 1] = (byte)(checksum * 2);
            return data;
        }

        public static void MakeKey(params string[] data)
        {
            var bytes = new List<byte>();
            foreach (string s in data)
                bytes.AddRange(KeyString(s ?? ""));
            DecryptionKey = MakeKeyCombined(bytes.ToArray());
            InitDecryptionTable(DecryptionKey);
        }

        public static byte[] DecodeMode3(byte[] chunkData, int chunkId, out int decompressed)
        {
            var reader = new ByteReader(chunkData);
            var decompressedSize = reader.ReadUInt();

            var rawData = reader.ReadBytes((int)reader.Size());

            if ((chunkId & 1) == 1 && NebulaCore.Build > 285)
                rawData[0] ^= (byte)((chunkId & 0xFF) ^ (chunkId >> 0x8));

            TransformChunk(rawData);

            using (var data = new ByteReader(rawData))
            {
                var compressedSize = data.ReadUInt();
                decompressed = (int)decompressedSize;
                return Decompressor.DecompressBlock(data, (int)compressedSize);
            }
        }

        private static byte[] decodeBuffer = new byte[256];
        public static bool valid;

        public static bool InitDecryptionTable(byte[] key)
        {
            for (int i = 0; i < 256; i++)
                decodeBuffer[i] = (byte)i;

            byte i2 = 0;
            byte key_pos = 0;
            for (uint i = 0; i < 256; i++)
            {
                if (key[key_pos] == 0) key_pos = 0;
                i2 += (byte)(key[key_pos++] + decodeBuffer[i]);
                (decodeBuffer[i2], decodeBuffer[i]) = (decodeBuffer[i], decodeBuffer[i2]);
            }
            valid = true;
            return true;
        }

        public static bool TransformChunk(byte[] chunk)
        {
            if (!valid) return false;
            byte[] tempBuf = new byte[256];
            Array.Copy(decodeBuffer, tempBuf, 256);

            byte i = 0;
            byte i2 = 0;
            for (int j = 0; j < chunk.Length; j++)
            {
                i2 += tempBuf[++i];
                (tempBuf[i2], tempBuf[i]) = (tempBuf[i], tempBuf[i2]);
                chunk[j] ^= tempBuf[(byte)(tempBuf[i] + tempBuf[i2])];
            }
            return true;
        }
    }
}
