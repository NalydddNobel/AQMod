namespace Aequus.Old.Content.Items.Potions;

public class PotionGlobalItem : GlobalItem {
    public override void GrabRange(Item item, Player player, ref int grabRange) {
        if ((item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
            && player.TryGetModPlayer(out PotionsPlayer potionPlayer) && potionPlayer.empoweredPotionId == BuffID.Heartreach) {
            grabRange *= 2;
        }
    }
}
