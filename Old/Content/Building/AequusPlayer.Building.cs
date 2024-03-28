using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Aequus;

public partial class AequusPlayer {
    public bool accLavaPlacement;

    private static bool OverrideLavaBlockPlacement(On_Player.orig_PlaceThing_Tiles_CheckLavaBlocking orig, Player player) {
        return player.GetModPlayer<AequusPlayer>().accLavaPlacement ? false : orig(player);
    }

    private void IL_Player_PlaceThing_ValidTileForReplacement(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<WorldGen>(nameof(WorldGen.WouldTileReplacementBeBlockedByLiquid)) && i.Previous?.MatchLdcI4(1) == true)) {
            Mod.Logger.Error($"Could not find {nameof(WorldGen)}.{nameof(WorldGen.WouldTileReplacementBeBlockedByLiquid)} method."); return;
        }

        c.Emit(OpCodes.Ldarg_0); // Push Player
        c.EmitDelegate<Func<bool, Player, bool>>((originalValue, player) => {
            bool value = player.GetModPlayer<AequusPlayer>().accLavaPlacement ? false : originalValue;
            //Main.NewText(value);
            return value;
        });
    }
}
