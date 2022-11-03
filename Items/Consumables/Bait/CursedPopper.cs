using Aequus.Common.ModPlayers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Bait
{
    public class CursedPopper : ModItem, ItemHooks.IModifyFishingPower
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 30;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        void ItemHooks.IModifyFishingPower.ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel)
        {
            if (player.ZoneCorrupt)
                fishingLevel += 0.3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.UnholyWater, 10)
                .AddIngredient(ItemID.CursedFlame, 10)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.EnchantedNightcrawler);
        }
    }
}