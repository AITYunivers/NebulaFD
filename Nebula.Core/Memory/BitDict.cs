namespace Nebula.Core.Memory
{
    public static class ByteFlag
    {
        public static bool GetFlag(uint flagbyte, int pos)
        {
            uint mask = (uint)(1 << pos);
            uint result = flagbyte & mask;
            return result == mask;
        }
    }

    public class BitDict
    {
        public string[] Keys = new string[0];
        public uint Value { get; set; }

        public BitDict()
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < 32; i++)
                keys.Add(i.ToString());
            Keys = keys.ToArray();
        }

        public BitDict(params string[] keys) => Keys = keys;
        public BitDict(Type baseEnum)
        {
            string[] enumKeys = Enum.GetNames(baseEnum);
            int[] enumValues = (int[])Enum.GetValues(baseEnum);
            int keyCount = 0;
            foreach (int val in enumValues)
                keyCount = Math.Max(keyCount, val);
            Keys = new string[keyCount + 1];
            for (int i = 0; i < enumValues.Length; i++)
                Keys[enumValues[i]] = enumKeys[i];
        }

        public bool this[string key]
        {
            get => GetFlag(key);
            set => SetFlag(key, value);
        }

        public bool this[int pos]
        {
            get => GetFlag(pos);
            set => SetFlag(pos, value);
        }

        public bool this[object pos]
        {
            get => GetFlag((int)pos);
            set => SetFlag((int)pos, value);
        }

        public static bool operator ==(BitDict dict, object pos) => dict.GetFlag((int)pos);
        public static bool operator !=(BitDict dict, object pos) => !dict.GetFlag((int)pos);

        public bool GetFlag(string key) => GetFlag(Array.IndexOf(Keys, key));
        public bool GetFlag(object pos) => GetFlag((int)pos);
        public bool GetFlag(int pos)
        {
            if (pos >= 0)
                return (Value & ((uint)Math.Pow(2, pos))) != 0;
            return false;
        }

        public void SetFlag(string key, bool flag) => SetFlag(Array.IndexOf(Keys, key), flag);
        public void SetFlag(object pos, bool flag) => SetFlag((int)pos, flag);
        public void SetFlag(int pos, bool flag)
        {
            if (flag)
                Value |= (uint)Math.Pow(2, pos);
            else
                Value &= ~(uint)Math.Pow(2, pos);
        }

        public override string ToString()
        {
            Dictionary<string, bool> actualKeys = new Dictionary<string, bool>();
            foreach (var key in Keys)
                actualKeys[key] = this[key];

            return string.Join(";\n", actualKeys.Select(kv => kv.Key + ": " + kv.Value).ToArray()) + ';';
        }
    }
}