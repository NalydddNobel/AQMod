namespace AQMod.Common
{
    public static class AQStringCodes
    {
        private const string AQMod_Key = "@";
        private const string Split_Key = "$";
        private const string Polarities_Key = "%";
        private const string CalamityMod_Key = "!";

        public static string EncodeModName(object obj)
        {
            var text = obj.ToString();
            switch (text)
            {
                default:
                    return text;

                case "AQMod":
                    return AQMod_Key;

                case "Split":
                    return Split_Key;

                case "Polarities":
                    return Polarities_Key;

                case "CalamityMod":
                    return CalamityMod_Key;
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

                case Split_Key:
                    return "Split";

                case Polarities_Key:
                    return "Polarities";

                case CalamityMod_Key:
                    return "CalamityMod";
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

        public static string ExtractParameterText(string text, int position = 0)
        {
            return ExtractParameterText(text, ref position);
        }

        public static string ExtractParameterText(string text, ref int position)
        {
            string extract = "";
            for (; position < text.Length; position++)
            {
                if (text[position] == ':')
                {
                    break;
                }
                if (text[position] == '#')
                {
                    continue;
                }
                extract += text[position];
            }
            return extract;
        }
    }
}