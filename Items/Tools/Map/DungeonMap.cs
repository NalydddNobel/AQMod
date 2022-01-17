using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Map
{
    public class DungeonMap : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
            item.consumable = true;
            item.useAnimation = 50;
            item.useTime = 50;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<PlayerMapUpgrades>().VialOfBlood == PlayerMapUpgrades.MapUpgrade.NotObtained;
        }

        public override bool UseItem(Player player)
        {
            var mapUpgrades = player.GetModPlayer<PlayerMapUpgrades>();
            mapUpgrades.VialOfBlood = PlayerMapUpgrades.MapUpgrade.Visible;
            return true;
        }
    }
}