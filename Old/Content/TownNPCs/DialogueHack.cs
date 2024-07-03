using System;
using Terraria.Localization;

namespace Aequu2.Old.Content.TownNPCs;

internal class DialogueHack {
    public static string ReplaceKeys(ref string text, string needsToStartWith, string key, Func<string[], object> turnStringArgsIntoObject) {
        int i = text.IndexOf(needsToStartWith);
        if (i > -1) {
            string val = text[i..].Split('+')[0];
            text = text.Replace($"{val}+", Language.GetTextValueWith(key, turnStringArgsIntoObject(val.Replace('+', ' ').Split('|'))));
        }
        return text;
    }
}
