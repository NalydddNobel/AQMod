using AQMod.Content.Fishing;
using AQMod.Content.LegacyWorldEvents.GlimmerEvent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Fish
{
    public class Blobfish : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && GlimmerEvent.GetTileDistance(player) < GlimmerEvent.MaxDistance;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish, 5);
            r.AddRecipe();
        }
    }
}