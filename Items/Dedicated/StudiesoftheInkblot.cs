using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated
{
    public class StudiesoftheInkblot : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Yellow;
            item.magic = true;
            item.damage = 100;
            item.crit = 10;
            item.mana = 15;
            item.useTime = 45;
            item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = Item.sellPrice(gold: 30);
        }

        Color IDedicatedItem.DedicatedItemColor() => DedicatedColors.starlightmp4;
    }
}
