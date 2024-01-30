using Aequus.Common.Items.Components;

namespace Aequus.Content.Potions.Healing.Restoration;

public class SuperRestorationPotion : ModItem, IApplyPotionDelay {
    public System.Boolean ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.UseSound = SoundID.Item3;
        Item.healLife = 180;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.potion = true;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Lime;
    }

    public override void AddRecipes() {
        CreateRecipe(4)
            .AddIngredient<GreaterRestorationPotion>(4)
            .AddIngredient(ItemID.FragmentNebula)
            .AddIngredient(ItemID.FragmentSolar)
            .AddIngredient(ItemID.FragmentStardust)
            .AddIngredient(ItemID.FragmentVortex)
            .AddTile(TileID.Bottles)
            .Register()
            .DisableDecraft();
    }
}
