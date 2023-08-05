using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Healing.Restoration;
public class SuperRestorationPotion : ModItem, ItemHooks.IApplyPotionDelay {
    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void SetDefaults() {
        Item.UseSound = SoundID.Item3;
        Item.healLife = 190;
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
            .TryRegisterAfter(ItemID.RestorationPotion)
            .DisableDecraft();
    }
}
