using Aequus.Common.Items;
using Aequus.Content.Items.Materials.OmniGem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Healing.Restoration;
public class GreaterRestorationPotion : ModItem, ItemHooks.IApplyPotionDelay {
    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void SetDefaults() {
        Item.UseSound = SoundID.Item3;
        Item.healLife = 140;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.potion = true;
        Item.value = Item.sellPrice(silver: 20);
        Item.rare = ItemRarityID.Orange;
    }

    public override void AddRecipes() {
        CreateRecipe(3)
            .AddIngredient(ItemID.BottledHoney)
            .AddIngredient(ItemID.PinkGel)
            .AddIngredient(ItemID.PixieDust, 3)
            .AddIngredient<OmniGem>()
            .AddTile(TileID.Bottles)
            .TryRegisterAfter(ItemID.RestorationPotion)
            .DisableDecraft();
    }
}