using MonoMod.Cil;
using System;
using Terraria.Audio;

namespace Aequus.Core.Debug;

internal class AudioLogger : ModSystem {
    public static bool DisableLogs;

    public override void Load() {
        IL_SoundEngine.PlaySound_refSoundStyle_Nullable1_SoundUpdateCallback += IL_SoundEngine_PlaySound_refSoundStyle_Nullable1_SoundUpdateCallback;
    }

    private static void IL_SoundEngine_PlaySound_refSoundStyle_Nullable1_SoundUpdateCallback(MonoMod.Cil.ILContext il) {
        ILCursor cursor = new ILCursor(il);

        cursor.EmitDelegate(LogSound);
    }

    private static void LogSound() {
        if (Main.drawDiag && !DisableLogs) {
            DisableLogs = true;
            Main.NewText("------------------------------------", Main.errorColor);
            foreach (var line in Environment.StackTrace.Split('\n')) {
                Main.NewText(line, Color.White);
            }
            DisableLogs = false;
        }
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DEBUG_MODE;
    }
}
