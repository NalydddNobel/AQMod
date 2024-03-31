using Aequus.Common.Items.Components;
using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod.CalamityModSupport.Items;

public class OmegaRestoration : CrossModItem, IApplyPotionDelay {
    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void OnSetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 32;
        Item.useTurn = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.healLife = 270;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.UseSound = SoundID.Item3;
        Item.consumable = true;
        Item.potion = true;
        Item.value = Item.buyPrice(gold: 7);
        Item.rare = CalamityMod.RarityDarkBlue_14;
    }

    protected override void SafeAddRecipes() {
        if (!CalamityMod.TryGetItem("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)) {
            return;
        }

        CreateRecipe(20)
            .AddIngredient<SupremeRestoration>(20)
            .AddIngredient(ascendantSpiritEssence)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeModItem("OmegaHealingPotion", this)
            .DisableDecraft();
    }
}
