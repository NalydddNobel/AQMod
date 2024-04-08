namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static int On_Player_RollLuck(On_Player.orig_RollLuck orig, Player self, int range) {
        int rolled = orig(self, range);
#if !DEBUG
        self.GetModPlayer<AequusPlayer>().RerollLuckFull(ref rolled, range);
#endif
        return rolled;
    }
}
