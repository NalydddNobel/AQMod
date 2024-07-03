namespace Aequu2.Core.Entities.Items.Components;

public interface IApplyPotionDelay {
    bool ApplyPotionDelay(Player player);

    internal static void On_Player_ApplyPotionDelay(On_Player.orig_ApplyPotionDelay orig, Player player, Item sItem) {
        if (sItem.ModItem is IApplyPotionDelay applyPotionDelay && applyPotionDelay.ApplyPotionDelay(player)) {
            return;
        }
        orig(player, sItem);
    }
}