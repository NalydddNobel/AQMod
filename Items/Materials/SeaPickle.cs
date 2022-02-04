using AQMod.Content.Fishing;
using Terraria;

namespace AQMod.Items.Materials
{
    public class SeaPickle : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 10);
            item.maxStack = 999;
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer < FishLoader.WorldLayers.HellLayer && player.Biomes().zoneCrabCrevice;
        }
    }
}