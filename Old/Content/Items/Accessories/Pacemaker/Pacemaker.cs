using Aequus.Old.Content.TownNPCs.PhysicistNPC.Analysis;
using System;

namespace Aequus.Old.Content.Items.Accessories.Pacemaker;

[LegacyName("HyperJet")]
public class Pacemaker : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
        AnalysisSystem.PhysicistPrimaryRewardItems.Add(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateInfoAccessory(Player player) {
        var pacemakerPlayer = player.GetModPlayer<PacemakerPlayer>();
        pacemakerPlayer.respawnSpeedupTime = Math.Max(pacemakerPlayer.respawnSpeedupTime, 1);
    }
}