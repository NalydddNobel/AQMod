namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for the Lavaproof Mitten to grant block placement in lava.</summary>
    private static bool On_Player_PlaceThing_Tiles_CheckLavaBlocking(On_Player.orig_PlaceThing_Tiles_CheckLavaBlocking orig, Player player) {
        return orig(player);
    }
}
