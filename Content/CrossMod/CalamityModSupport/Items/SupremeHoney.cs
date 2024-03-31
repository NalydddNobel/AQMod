using Aequus.Content.Potions.Healing.Honey;
using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod.CalamityModSupport.Items;

public class SupremeHoney : CrossModItem {
    public override void OnSetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.healLife = 200;
        Item.rare = ItemRarityID.Purple;
        Item.value = Item.buyPrice(gold: 6, silver: 50);
        Item.buffType = BuffID.Honey;
        Item.buffTime = 2400;
        Item.width = 28;
        Item.height = 18;
        Item.useTurn = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.UseSound = SoundID.Item3;
        Item.consumable = true;
        Item.potion = true;
    }

    protected override void SafeAddRecipes() {
        if (!CalamityMod.TryGetItem("Bloodstone", out ModItem bloodstone)) {
            return;
        }

        CreateRecipe(4)
            .AddIngredient<SuperHoney>(4)
            .AddIngredient(bloodstone, 3)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeModItem("SupremeHealingPotion", this)
            .DisableDecraft();
    }
}
