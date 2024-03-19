using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod.CalamityModSupport.Items;

public class OmegaHoney : CrossModItem {
    public override void SafeSetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.healLife = 240;
        Item.value = Item.buyPrice(gold: 7);
        Item.rare = CalamityMod.RarityDarkBlue_14;
        Item.buffType = BuffID.Honey;
        Item.buffTime = 3000;
        Item.width = 24;
        Item.height = 32;
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
        if (!CalamityMod.TryGetItem("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)) {
            return;
        }

        CreateRecipe(20)
            .AddIngredient<SupremeHoney>(20)
            .AddIngredient(ascendantSpiritEssence)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeModItem("OmegaHealingPotion", this)
            .DisableDecraft();
    }
}
