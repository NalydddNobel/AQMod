using Aequus.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Bait
{
    public class IchorPopper : ModItem, Hooks.IModifyFishingPower
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 31;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        void Hooks.IModifyFishingPower.ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel)
        {
            if (player.ZoneCrimson)
                fishingLevel += 0.35f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.Ichor, 10)
                .AddIngredient(ItemID.BloodWater)
                .AddTile(TileID.Bottles)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.EnchantedNightcrawler));
        }
    }
}