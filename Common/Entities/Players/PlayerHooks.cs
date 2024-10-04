using Aequus.Common.Utilities.Cil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.Achievements;

namespace Aequus.Common.Entities.Players;

internal class PlayerHooks : LoadedType {
    public static bool QuickBuff { get; private set; }

    protected override void Load() {
        On_Player.QuickBuff += On_Player_QuickBuff;
        IL_Player.DashMovement += IL_Player_DashMovement;
        IL_Player.PickTile += IL_Player_PickTile;
    }

    void IL_Player_DashMovement(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdfld(typeof(Player), nameof(Player.eocHit)))) {
            Mod.Logger.Error("Could not find ldfld of Player.eocHit."); return;
        }

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchCall(typeof(Player), nameof(Player.GetTotalDamage)))) {
            Mod.Logger.Error("Could not find call for Player.GetTotalDamage."); return;
        }

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchStloc(out _))) {
            Mod.Logger.Error("Could not find stloc for eoc shield's damage stat modifier."); return;
        }

        // StatModifier for the Shield of Cthulhu is already on the stack.

        // Push player onto stack to be used in the method call.
        c.Emit(OpCodes.Ldarg_0);

        // Modify the Shield of Cthulhu's damage StatModifier by calling this method and returning a modified StatModifier.
        c.EmitDelegate(ModifyEocShieldDamage);

        // Afterward this method call, the code will continue by storing the StatModifier into a local variable and utilizing it.
        MonoModHooks.DumpIL(Mod, il);
    }

    static StatModifier ModifyEocShieldDamage(StatModifier modifier, Player player) {
        if (player.TryGetModPlayer(out AequusPlayer aequus)) {
            modifier = modifier.CombineWith(aequus.accessoryDamage);
        }

        return modifier;
    }

    static void On_Player_QuickBuff(On_Player.orig_QuickBuff orig, Player player) {
        QuickBuff = true;
        orig(player);
        QuickBuff = false;
    }

    static bool _player_PickTile;
    void IL_Player_PickTile(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchPropertySetter(typeof(AchievementsHelper), nameof(AchievementsHelper.CurrentlyMining)))) {
            Mod.Logger.Error("Could not find mining AchievementsHelper.CurrentlyMining setter in Player.PickTile."); return;
        }

        c.Emit(OpCodes.Ldarg_0); // Player
        c.Emit(OpCodes.Ldarg_1); // X
        c.Emit(OpCodes.Ldarg_2); // Y
        c.Emit(OpCodes.Ldarg_3); // Pickaxe Power
        c.EmitDelegate((Player player, int X, int Y, int PickPower) => {
            player.GetModPlayer<AequusPlayer>().OnBreakTile(X, Y);
            if (!_player_PickTile) {
                _player_PickTile = true;
                Aequus.Instance.Logger.Info("Pick tile hook success!");
            }
        });
    }
}
