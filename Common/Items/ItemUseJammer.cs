using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public class ItemUseJammer : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0 ? null : false;
    }

    public override bool CanShoot(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0;
    }
}