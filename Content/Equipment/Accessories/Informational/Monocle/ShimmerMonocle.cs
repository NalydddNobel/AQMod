using Aequus.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.Informational.Monocle;

[WorkInProgress]
public class ShimmerMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accShimmerMonocle = true;
    }
}