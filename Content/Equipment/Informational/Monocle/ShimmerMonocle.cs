using Aequus.Core.CodeGeneration;

namespace Aequus.Content.Equipment.Informational.Monocle;

[PlayerGen.InfoField("accInfoShimmerMonocle")]
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