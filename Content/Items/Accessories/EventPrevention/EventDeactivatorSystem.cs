using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace AequusRemake.Content.Items.Accessories.EventPrevention;
public class EventDeactivatorSystem : ModSystem {
    public override void Load() {
        On_NPC.SpawnNPC += SpawnNPCUndoFlags;
        On_Projectile.FishingCheck += OverrideFishingFlags;
        IL_NPC.SpawnNPC += SpawnNPCOverrideEventsPerPlayer;
        IL_Main.UpdateAudio += OverrideEventsForMusic;
        IL_Main.CalculateWaterStyle += OverrideBloodMoonWaterStyle;
    }

    #region Hooks
    private static void OverrideFishingFlags(On_Projectile.orig_FishingCheck orig, Projectile projectile) {
        EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.player[projectile.owner]);
        orig(projectile);
        EventDeactivatorPlayer.UndoPlayerFlagOverrides();
    }

    private static void SpawnNPCUndoFlags(On_NPC.orig_SpawnNPC orig) {
        orig();
        EventDeactivatorPlayer.UndoPlayerFlagOverrides();
    }

    private void SpawnNPCOverrideEventsPerPlayer(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(typeof(Main), nameof(Main.slimeRain)))) {
            Mod.Logger.Error($"Could not find Main.slimeRain ldsfld code."); return;
        }

        ILLabel slimeRainCheckLabel = c.MarkLabel();

        int loc = -1;
        if (!c.TryGotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.player)) && i.Next.MatchLdloc(out loc)) && loc != -1) {
            Mod.Logger.Error($"Could not find Main.player ldsfld code."); return;
        }

        c.GotoLabel(slimeRainCheckLabel);

        c.Emit(OpCodes.Ldloc, loc);
        c.EmitDelegate((int player) => {
            EventDeactivatorPlayer.UndoPlayerFlagOverrides();
            EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.player[player]);
        });
    }

    private void OverrideBloodMoonWaterStyle(ILContext il) {
        ILCursor c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.bloodMoon)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.bloodMoon)} ldsfld-branching code."); return;
        }

        c.EmitDelegate((bool bloodMoon) => bloodMoon && (!Main.LocalPlayer.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator) || !eventDeactivator.accDisableBloodMoon));
    }

    private void OverrideEventsForMusic(ILContext il) {
        const string SwapMusicField = "swapMusic";
        const BindingFlags SwapMusicBindings = BindingFlags.Static | BindingFlags.NonPublic;

        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), SwapMusicField))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{SwapMusicField} ldsfld code."); return;
        }

        c.EmitDelegate((bool swapMusic) => EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.LocalPlayer));
        c.EmitLdsfld(typeof(Main).GetField(SwapMusicField, SwapMusicBindings));

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.musicBox2)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.musicBox2)} ldsfld code."); return;
        }

        c.EmitDelegate((int musicBox2) => EventDeactivatorPlayer.UndoPlayerFlagOverrides());
        c.EmitLdsfld(typeof(Main).GetField(nameof(Main.musicBox2), BindingFlags.Static | BindingFlags.Public));
    }
    #endregion
}
