using AQMod.Content.Fishing;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Potions
{
    public class PalePufferfish : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.consumable = true;
            item.value = Item.sellPrice(silver: 50);
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
            item.useTime = 17;
            item.useAnimation = 17;
            item.maxStack = 999;
        }

        public override bool RandomCatchFail()
        {
            return Main.rand.NextBool(35);
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && Main.bloodMoon;
        }

        public override bool UseItem(Player player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (player.buffTime[i] > 0 && AQBuff.Sets.CanBeRemovedByWhiteBloodCell[player.buffType[i]])
                {
                    player.DelBuff(i);
                    i--;
                }
            }
            player.GetModPlayer<VampirismPlayer>().Vampirism = 0;
            return true;
        }
    }
}