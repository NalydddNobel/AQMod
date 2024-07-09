using AequusRemake.Systems.Items;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    internal static void On_Player_ApplyPotionDelay(On_Player.orig_ApplyPotionDelay orig, Player player, Item sItem) {
        if (sItem.ModItem is IModifyPotionDelay applyPotionDelay && applyPotionDelay.ApplyPotionDelay(player)) {
            return;
        }
        orig(player, sItem);
    }
}
