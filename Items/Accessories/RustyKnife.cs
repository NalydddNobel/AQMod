using AQMod.Content.DedicatedItemTags;
using AQMod.Items.Dedicated;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class RustyKnife : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Yellow;
            item.accessory = true;
            item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().omori = true;
        }

        Color IDedicatedItem.DedicatedItemColor => new Color(30, 255, 60, 255);
        IDedicationType IDedicatedItem.DedicationType => new BasicDedication();
    }
}