namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Re-rolls luck for the Rabbit's Foot.</summary>
    private static int On_Player_RollLuck(On_Player.orig_RollLuck orig, Player self, int range) {
        int rolled = orig(self, range);
#if !DEBUG
        self.GetModPlayer<AequusPlayer>().RerollLuckFull(ref rolled, range);
#endif
        return rolled;
    }
}
