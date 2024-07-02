using Aequus.Content.Dedicated.BeyondCoin;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    private void IL_Player_UpdateManaRegen(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>(nameof(Player.usedArcaneCrystal)))) {
            Mod.Logger.Error($"Could not find {nameof(Player)}.{nameof(Player.usedArcaneCrystal)} ldfld."); return;
        }

        // Create a label so we can get back here later.
        ILLabel label = c.MarkLabel();

        // Subtract 1 from the index so we are right before the Player.usedArcaneCrystal loading code.
        c.Index--;

        // ldarg.0 is taken from ldfld, and it's also the jump label for a previous branch statement.
        c.EmitDelegate(ShimmerCoinPlayer.ArcaneCrystalEffectDelay);
        c.Emit(OpCodes.Ldarg_0); // Push Player

        // Go to label so we can find the next instance of Player.usedArcaneCrystal. (Instead of finding the one we already found.)
        c.GotoLabel(label);

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>(nameof(Player.usedArcaneCrystal)))) {
            Mod.Logger.Error($"Could not find {nameof(Player)}.{nameof(Player.usedArcaneCrystal)} ldfld."); return;
        }

        // ldarg.0 is taken from ldfld
        c.EmitDelegate(ShimmerCoinPlayer.ArcaneCrystalEffect);
        c.Emit(OpCodes.Ldarg_0); // Push Player
    }
}
