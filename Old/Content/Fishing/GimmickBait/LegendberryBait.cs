using Aequus.Common.Items.Components;
using Aequus.Core.ContentGeneration;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Fishing.GimmickBait;
public class LegendberryBait : UnifiedModBait, IModifyFishAttempt {
    public override void SetDefaults() {
        Item.width = 6;
        Item.height = 6;
        Item.bait = 30;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.value = Item.sellPrice(silver: 20);
        Item.rare = ItemRarityID.LightPurple;
    }

    public bool PreCatchFish(Projectile bobber, ref FishingAttempt fisher) {
        if (fisher.crate) {
            return true;
        }

        fisher.common = false;
        fisher.uncommon = true;
        int rolledTier = Main.rand.Next(4);

        if (rolledTier >= 3) {
            fisher.legendary = true;
            fisher.fishingLevel *= 2;
        }
        if (rolledTier >= 2) {
            fisher.veryrare = true;
            fisher.fishingLevel *= 2;
        }
        if (rolledTier >= 1) {
            fisher.rare = true;
            fisher.fishingLevel *= 2;
        }
        return true;
    }
}