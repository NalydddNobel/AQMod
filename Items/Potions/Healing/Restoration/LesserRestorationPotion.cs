using Aequus.Common.Items.Components;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Healing.Restoration;

public class LesserRestorationPotion : ModItem, IApplyPotionDelay {
    public override string Texture => AequusTextures.Item(ItemID.LesserRestorationPotion);

    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.UseSound = SoundID.Item3;
        Item.healLife = 45;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.potion = true;
        Item.value = Item.sellPrice(silver: 1, copper: 50);
        Item.rare = ItemRarityID.White;
    }

    public override void AddRecipes() {
        CreateRecipe(3)
            .AddIngredient(ItemID.Mushroom)
            .AddIngredient(ItemID.Amethyst)
            .AddIngredient(ItemID.Bottle, 3)
            .AddTile(TileID.Bottles)
            .Register()
            .DisableDecraft();
        CreateRecipe(1)
            .AddIngredient(Type, 2)
            .AddIngredient(ItemID.GlowingMushroom)
            .AddTile(TileID.Bottles)
            .Register()
            .DisableDecraft();
    }
}