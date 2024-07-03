using Aequu2.Core.CodeGeneration;

namespace Aequu2.Content.Items.Accessories.Informational.Monocle;

[Gen.AequusPlayer_InfoField("accInfoShimmerMonocle")]
public class ShimmerMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoShimmerMonocle = true;
    }
}