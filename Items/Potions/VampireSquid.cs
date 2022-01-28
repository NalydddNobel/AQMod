using AQMod.Buffs.Vampire;
using AQMod.Content.Fishing;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
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
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
            item.useTime = 17;
            item.useAnimation = 17;
            item.buffTime = 10800;
            item.buffType = ModContent.BuffType<Vampirism>();
            item.maxStack = 999;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<VampirismPlayer>().IsVampire;
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<VampirismPlayer>().GiveVampirism(item.buffTime);
            return base.UseItem(player);
        }

        public override bool RandomCatchFail()
        {
            return Main.rand.NextBool(65);
        }

        public override bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water && worldLayer <= FishLoader.WorldLayers.Overworld
                && !Main.dayTime && Main.bloodMoon && Main.hardMode;
        }
    }
}