namespace AQMod.Common
{
    public static class AQStringCodes
    {
        private const string AQMod_Key = "@";

        public static string EncodeModName(object obj)
        {
            var text = obj.ToString();
            switch (text)
            {
                default:
                return text;

                case "AQMod":
                return AQMod_Key;
            }
        }

        public static string DecodeModName(string text)
        {
            switch (text)
            {
                default:
                return text;

                case AQMod_Key:
                return "AQMod";
            }
        }

        public static string EncodeName(string mod, string name)
        {
            return EncodeModName(mod) + ":" + name;
        }

        public static bool DecodeName(string key, out string mod, out string name)
        {
            if (key.Contains(":"))
            {
                int cursor = 0;
                mod = "";
                while (key[cursor] != ':')
                {
                    mod += key[cursor];
                    cursor++;
                }
                cursor++;
                mod = DecodeModName(mod);
                name = "";
                while (cursor < key.Length)
                {
                    name += key[cursor];
                    cursor++;
                }
                return true;
            }
            mod = default(string);
            name = default(string);
            return false;
        }
    }
}