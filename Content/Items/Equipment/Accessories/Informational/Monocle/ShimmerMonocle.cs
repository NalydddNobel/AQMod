using Aequus.Common.Items.EquipmentBooster;
using Aequus.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Informational.Monocle;

[WorkInProgress]
public class ShimmerMonocle : ModItem {
    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accShimmerMonocle = true;
    }
}