namespace Aequus.Content.Items.Tools.Glowsticks.BlackGlowstick;

public class BlackGlowstick : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Glowstick);
    }

    public override void HoldItem(Player player) {
        base.HoldItem(player);
    }
}