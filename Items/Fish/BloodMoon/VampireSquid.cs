using AQMod.Buffs.Timers;
using AQMod.Content.Fishing;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fish.BloodMoon
{
    public class VampireSquid : FishingItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 3);
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.buffTime = 10800;
            item.buffType = ModContent.BuffType<Vampirism>();
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<VampirismPlayer>().GiveVampirism(item.buffTime);
            return base.UseItem(player);
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && Main.bloodMoon;
        }
    }
}