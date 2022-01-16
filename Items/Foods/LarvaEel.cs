using AQMod.Content.Fishing;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Foods
{
    public class LarvaEel : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 15);
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
            item.consumable = true;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 20;
            item.useTime = 20;
            item.potion = true;
            item.healLife = 100;
            item.buffType = BuffID.Honey;
            item.buffTime = 10800;
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Honey && worldLayer < FishLoader.WorldLayers.HellLayer;
        }
    }
}