using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for the Lavaproof Mitten to grant block placement in lava for blockswap.</summary>
    private void IL_Player_PlaceThing_ValidTileForReplacement(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<WorldGen>(nameof(WorldGen.WouldTileReplacementBeBlockedByLiquid)) && i.Previous?.MatchLdcI4(1) == true)) {
            Mod.Logger.Error($"Could not find {nameof(WorldGen)}.{nameof(WorldGen.WouldTileReplacementBeBlockedByLiquid)} method."); return;
        }

        c.Emit(OpCodes.Ldarg_0); // Push Player
        c.EmitDelegate<Func<bool, Player, bool>>((originalValue, player) => {
#if !DEBUG
            originalValue = player.GetModPlayer<AequusPlayer>().accLavaPlacement ? false : originalValue;
            //Main.NewText(originalValue);
#endif
            return originalValue;
        });
    }
}
