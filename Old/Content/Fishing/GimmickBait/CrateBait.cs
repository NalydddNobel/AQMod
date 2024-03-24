using Aequus.Common.Items.Components;
using Aequus.Content.Fishing;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Fishing.GimmickBait;
public class CrateBait : ModBait, IModifyFishAttempt {
    public override void SetDefaults() {
        Item.width = 6;
        Item.height = 6;
        Item.bait = 30;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.value = Item.sellPrice(silver: 1);
        Item.rare = ItemRarityID.Orange;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ItemID.Amber, 5)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.EnchantedNightcrawler);
    }

    public bool PreCatchFish(Projectile bobber, ref FishingAttempt fisher) {
        if (!fisher.crate) {
            fisher.crate = Main.rand.NextBool();
        }

        if (fisher.crate && !fisher.inHoney) {
            //fisher.fishingLevel += 1000000;
            fisher.common = true;
            fisher.uncommon = Main.rand.NextBool();
            fisher.rare = Main.rand.NextBool();
            fisher.veryrare = Main.rand.NextBool();
            fisher.legendary = Main.rand.NextBool();
        }
        return true;
    }
}