namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static bool On_Player_PlaceThing_Tiles_CheckLavaBlocking(On_Player.orig_PlaceThing_Tiles_CheckLavaBlocking orig, Player player) {
        return player.GetModPlayer<AequusPlayer>().accLavaPlacement ? false : orig(player);
    }
}
