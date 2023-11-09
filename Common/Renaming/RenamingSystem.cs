using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Renaming;

public class RenamingSystem : ModSystem {
    public const char CommandChar = '{';
    public const char CommandEndChar = '}';

    #region Commands 
    public static Dictionary<char, Action<string, string, List<DecodedText>>> Commands { get; private set; } = new() {
        ['$'] = LanguageKeyCommand,
    };

    private static void LanguageKeyCommand(string fullCommand, string commandInput, List<DecodedText> output) {
        string languageText = Language.GetTextValue(commandInput, Lang.CreateDialogSubstitutionObject());

        output.Add(new DecodedText(fullCommand, languageText, commandInput == languageText ? DecodeType.FailedCommand : DecodeType.LanguageKey));
    }
    #endregion

    public static Dictionary<DecodeType, Color> DecodeColors { get; private set; } = new() {
        [DecodeType.None] = Color.White,
        [DecodeType.FailedCommand] = new Color(255, 120, 120),
        [DecodeType.LanguageKey] = new Color(255, 255, 80),
    };

    private static readonly List<DecodedText> _decodedTextList = new();

    public static List<RenamedNPCMarker> RenamedNPCs;

    public static string GetPlainDecodedText(string input) {
        _decodedTextList.Clear();
        DecodeText(input, _decodedTextList);

        string result = "";
        foreach (var i in _decodedTextList) {
            result += i.Output;
        }
        return result;
    }

    public static string GetColoredDecodedText(string input, bool pulse = false) {
        _decodedTextList.Clear();
        DecodeText(input, _decodedTextList);

        string result = "";
        foreach (var i in _decodedTextList) {
            result += i.Type != DecodeType.None ? TextHelper.ColorCommand(i.Output, DecodeColors[i.Type], pulse) : i.Output;
        }

        return result;
    }

    public static void DecodeText(string input, List<DecodedText> output) {
        string text = "";
        for (int i = 0; i < input.Length; i++) {
            if (input[i] == CommandChar) {
                FlushOldOutput();

                string subString = input[i..];
                int endIndex = subString.IndexOf(CommandEndChar);
                if (endIndex == -1) {
                    output.Add(new DecodedText(subString, subString, DecodeType.FailedCommand));
                    break;
                }

                i += endIndex + 1;
                subString = subString[..(endIndex + 1)];
                if (subString.Length < 3) {
                    output.Add(new DecodedText(subString, subString, DecodeType.FailedCommand));
                    continue;
                }

                if (Commands.TryGetValue(subString[1], out var command)) {
                    command(subString, subString[2..^1], output);
                }
                else {
                    output.Add(new DecodedText(subString, subString, DecodeType.FailedCommand));
                    continue;
                }
                continue;
            }

            text += input[i];
        }
        FlushOldOutput();

        void FlushOldOutput() {
            if (!string.IsNullOrEmpty(text)) {
                output.Add(new DecodedText(text, text, DecodeType.None));
            }
        }
    }
}