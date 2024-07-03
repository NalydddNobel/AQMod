using Aequu2.Old.Content.TownNPCs.PhysicistNPC.Analysis;

namespace Aequu2.Old.Content.Items.Accessories.HoloLens;

public class HoloLens : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
        AnalysisSystem.PhysicistPrimaryRewardItems.Add(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<Aequu2Player>().accHoloLens = true;
        if (player.whoAmI == Main.myPlayer) {
            ModContent.GetInstance<ChestLensInterface>().Activate();
        }
    }
}