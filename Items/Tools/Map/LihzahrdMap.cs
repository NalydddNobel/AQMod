using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Map
{
    public class LihzahrdMap : ModItem
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
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
        }

        public override bool UseItem(Player player)
        {
            var mapUpgrades = player.GetModPlayer<PlayerMapUpgrades>();
            mapUpgrades.Cabbage = PlayerMapUpgrades.MapUpgrade.Visible;
            return true;
        }
    }
}