using System;
using Terraria.Localization;

namespace Aequus.Old.Content.TownNPCs;

internal class DialogueHack {
    public static String ReplaceKeys(ref String text, String needsToStartWith, String key, Func<String[], Object> turnStringArgsIntoObject) {
        Int32 i = text.IndexOf(needsToStartWith);
        if (i > -1) {
            String val = text[i..].Split('+')[0];
            text = text.Replace($"{val}+", Language.GetTextValueWith(key, turnStringArgsIntoObject(val.Replace('+', ' ').Split('|'))));
        }
        return text;
    }
}
