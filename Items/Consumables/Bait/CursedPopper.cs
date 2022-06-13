using Aequus.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Bait
{
    public class CursedPopper : ModItem, IModifyFishingPower
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(50);
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 30;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        void IModifyFishingPower.ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel)
        {
            if (player.ZoneCorrupt)
                fishingLevel += 0.3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.CursedFlame, 10)
                .AddIngredient(ItemID.UnholyWater)
                .AddTile(TileID.Bottles)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.EnchantedNightcrawler));
        }
    }
}