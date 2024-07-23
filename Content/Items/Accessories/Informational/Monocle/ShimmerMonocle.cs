using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Content.Items.Accessories.Informational.Monocle;

[Gen.AequusPlayer_InfoField("accInfoShimmerMonocle")]
public class ShimmerMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoShimmerMonocle = true;
    }
}