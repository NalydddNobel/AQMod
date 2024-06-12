using Aequus.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aequus.Core.Debugging.CheatCodes;

public class CheatCodeManager : ModSystem {
    private static readonly List<CheatCode> Codes = [];
    private static bool CheatCodesNeedSaving { get; set; }

    private static readonly string CheatCodesFilePath = $"{Aequus.DEBUG_FILES_PATH}/Mods/Aequus/Cheats".Replace('/', Path.DirectorySeparatorChar);

    internal static void Register(CheatCode code) {
        foreach (CheatCode compareCode in Codes) {
            if (code.MatchingKeys(compareCode)) {
                Log.Error($"{code.GetType().FullName} matches keys with {compareCode.GetType().FullName}");
                return;
            }
        }

        code.Type = Codes.Count;
        Codes.Add(code);
    }

    public override void Load() {
        SaveActions.PreSaveWorld += SaveCheatCodesFile;
        LoadCheatCodesFile();
    }

    public override void Unload() {
        Codes.Clear();
    }

    public override void PostUpdateInput() {
        if (!Main.chatText.StartsWith("aequus", StringComparison.CurrentCultureIgnoreCase) || Main.oldKeyState == Main.keyState) {
            return;
        }

        foreach (CheatCode code in Codes) {
            if (code.CheckKeys(in Main.keyState)) {
                code.Toggle();
                CheatCodesNeedSaving = true;
            }
            else if (code.Active && !code.Params.HasFlag(Params.Toggle)) {
                code.Toggle();
            }
        }
    }

    private static void SaveCheatCodesFile(bool toCloud) {
        if (!CheatCodesNeedSaving) {
            return;
        }

        CheatCodesNeedSaving = false;
        try {
            using (FileStream stream = File.Create(CheatCodesFilePath)) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    foreach (CheatCode code in Codes.Where(c => c.Params.HasFlag(Params.SaveAndLoad))) {
                        writer.WriteLiteral($"\"{code.GetType().Name}\": \"{code.Active.ToInt()}\"\n");
                    }
                }
            }
        }
        catch (Exception ex) {
            Log.Error(ex);
        }
    }

    private static void LoadCheatCodesFile() {
        if (!File.Exists(CheatCodesFilePath)) {
            return;
        }

        string[] lines = File.ReadAllLines(CheatCodesFilePath);
        foreach (string line in lines) {
            try {
                string name = line[1..];
                name = name[..name.IndexOf('\"')];
                char value = line[(name.Length + 5)..][0];
                if (value != '1') {
                    continue;
                }

                foreach (CheatCode code in Codes) {
                    if (code.Name == name) {
                        code.SetActive();
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
