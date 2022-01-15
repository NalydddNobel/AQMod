using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Map
{
    public class CosmicTelescope : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
            item.consumable = true;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 50;
            item.useTime = 50;
        }

        public override bool UseItem(Player player)
        {
            var mapUpgrades = player.GetModPlayer<PlayerMapUpgrades>();
            mapUpgrades.CosmicTelescope = PlayerMapUpgrades.MapUpgrade.Visible;
            return true;
        }
    }
}