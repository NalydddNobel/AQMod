using AQMod.Content.Fishing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fish
{
    public class Nessie : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 40);
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && AQMod.Content.World.EventGlimmer.GetTileDistanceUsingPlayer(player) < AQMod.Content.World.EventGlimmer.MaxDistance;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish);
            r.AddRecipe();
        }
    }
}