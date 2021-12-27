using AQMod.Content.Fishing;
using AQMod.Content.LegacyWorldEvents.GlimmerEvent;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Materials.Fish
{
    public class UltraEel : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Orange;
            item.maxStack = 999;
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && GlimmerEvent.GetTileDistance(player) < GlimmerEvent.MaxDistance;
        }
    }
}