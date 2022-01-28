using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Map
{
    public class RetroGoggles : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            ItemID.Sets.ItemIconPulse[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
            item.consumable = true;
            item.useAnimation = 50;
            item.useTime = 50;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item8;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<PlayerMapUpgrades>().BlightedSoul == PlayerMapUpgrades.MapUpgrade.NotObtained;
        }

        public override bool UseItem(Player player)
        {
            var mapUpgrades = player.GetModPlayer<PlayerMapUpgrades>();
            mapUpgrades.BlightedSoul = PlayerMapUpgrades.MapUpgrade.Visible;
            return true;
        }
    }
}