namespace Aequus.Common.Items;

public class ItemUseJammer : GlobalItem {
    public override System.Boolean? UseItem(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0 ? null : false;
    }

    public override System.Boolean CanShoot(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0;
    }
}