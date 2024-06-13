using Aequus.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aequus.Core.Debugging.CheatCodes;

public class CheatCodeManager : ModSystem {
    private static readonly List<ICheatCode> Codes = [];
    private static bool CheatCodesNeedSaving { get; set; }

    private static readonly string CheatCodesFilePath = $"{Aequus.DEBUG_FILES_PATH}/Mods/Aequus/Cheats".Replace('/', Path.DirectorySeparatorChar);

    // Actually handle cheat code input.
    public override void PostUpdateInput() {
        if (!Main.chatText.StartsWith("aequus", StringComparison.CurrentCultureIgnoreCase) || Main.oldKeyState == Main.keyState) {
            return;
        }

        bool anyPressed = false;
        foreach (ICheatCode code in Codes) {
            if (!anyPressed && code.CheckKeys(in Main.keyState)) {
                if (!code.IsPressed) {
                    code.States.OnPress(code);
                }

                code.IsPressed = true;
                CheatCodesNeedSaving = true;
                anyPressed = true;
            }
            else if (code.IsPressed) {
                code.States.OnDepress(code);
                code.IsPressed = false;
            }
        }
    }

    internal static void Register(ICheatCode code) {
        foreach (ICheatCode compareCode in Codes) {
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

    private static void SaveCheatCodesFile(bool toCloud) {
        if (!CheatCodesNeedSaving || toCloud) {
            return;
        }

        CheatCodesNeedSaving = false;
        try {
            using (FileStream stream = File.Create(CheatCodesFilePath)) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    foreach (ICheatCode code in Codes.Where(c => c.Params.HasFlag(Params.SaveAndLoad))) {
                        writer.WriteLiteral($"\"{code.GetType().Name}\": \"{code.States.WriteData()}\"\n");
                    }
                }
            }

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A)) {
                Utils.OpenFolder($"{Aequus.DEBUG_FILES_PATH}/Mods/Aequus/");
            }
        }
        catch (Exception ex) {
            Log.Error(ex);
        }
    }

    private void LoadCheatCodesFile() {
        if (!File.Exists(CheatCodesFilePath)) {
            return;
        }

        string[] lines = File.ReadAllLines(CheatCodesFilePath);
        foreach (string line in lines) {
            try {
                string name = line[1..];
                name = name[..name.IndexOf('\"')];
                char value = line[(name.Length + 5)..][0];

                foreach (ICheatCode code in Codes) {
                    if (code.Name == name) {
                        code.States.ReadData(value);
                    }
                }
            }
            catch (Exception ex) {
                Mod.Logger.Error(ex);
            }
        }
    }
}
