using AQMod.Content.Fishing;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Accessories
{
    public class BloodPlasma : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 2);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().healBeforeDeath = true;
        }

        public override bool RandomCatchFail()
        {
            return Main.rand.NextBool(25);
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && Main.bloodMoon;
        }
    }
}