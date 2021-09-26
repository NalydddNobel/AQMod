using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Localization;

namespace AQMod.Common.DeveloperTools
{
    internal static class LangHelper
    {
        public static void Merge(GameCulture culture)
        {
            string fileLocation = "Localization/" + culture.CultureInfo.Name + ".lang";
            var aQMod = AQMod.Instance;
            var stream = aQMod.GetFileStream(fileLocation, false);
            aQMod.Logger.Debug(fileLocation);
            if (stream == null)
            {
                aQMod.Logger.Error("Stream to this localization file doesn't exist");
                return;
            }
            var reader = new StreamReader(stream);
            string s = "";
            List<string> lines = new List<string>();
            long endPosition = -1;
            while (s != "# Merge End")
            {
                s = reader.ReadLine();
                lines.Add(s);
            }
            endPosition = reader.BaseStream.Position;
            endPosition++;

            List<string> mergeKeys = new List<string>();
            List<string> mergeKeysValue = new List<string>();
            for (long l = endPosition; !reader.EndOfStream; l++)
            {
                s = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(s) || s[0] == '#')
                {
                    continue;
                }
                string key = "";
                foreach (char c in s)
                {
                    if (c != '=')
                    {
                        key += c;
                    }
                    else
                    {
                        break;
                    }
                }
                mergeKeys.Add(key);
                mergeKeysValue.Add(s);
            }
            for (int i = 0; i < lines.Count; i++)
            {
                string text = lines[i];
                if (string.IsNullOrWhiteSpace(text) || text[0] == '#')
                {
                    continue;
                }
                string key = "";
                foreach (char c in text)
                {
                    if (c != '=')
                    {
                        key += c;
                    }
                    else
                    {
                        break;
                    }
                }
                int sameKey = mergeKeys.FindIndex((mergingKey) => key.Equals(mergingKey));
                if (sameKey != -1)
                {
                    lines[i] = mergeKeysValue[sameKey];
                }
            }
            s = "";
            foreach (string text in lines)
            {
                aQMod.Logger.Debug(text);
                s += text + '\n';
            }
            fileLocation = Main.SavePath + Path.DirectorySeparatorChar + "Mods/Cache/AQMod/Localization/";
            Directory.CreateDirectory(fileLocation);
            fileLocation += culture.CultureInfo.Name + ".lang";
            var streamWriter = File.CreateText(fileLocation);
            streamWriter.Write(s);
            streamWriter.Close();
            aQMod.Logger.Debug("saved at: " + fileLocation);
        }
    }
}
