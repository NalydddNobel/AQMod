using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.Achievements;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for the breath conserver to restore breath upon breaking a tile.</summary>
    private void IL_Player_PickTile(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchPropertySetter(typeof(AchievementsHelper), nameof(AchievementsHelper.CurrentlyMining)))) {
            Mod.Logger.Error("Could not find mining AchievementsHelper.CurrentlyMining setter in Player.PickTile."); return;
        }

        c.Emit(OpCodes.Ldarg_0); // Player
        c.Emit(OpCodes.Ldarg_1); // X
        c.Emit(OpCodes.Ldarg_2); // Y
        c.Emit(OpCodes.Ldarg_3); // Pickaxe Power
        c.EmitDelegate((Player player, int X, int Y, int PickPower) => player.GetModPlayer<AequusPlayer>().RestoreBreathOnBrokenTile(X, Y));
    }
}
