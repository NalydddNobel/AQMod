using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated
{
    public class Baguette : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.UseSound = SoundID.Item2;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(silver: 80);
            item.buffType = BuffID.WellFed;
            item.buffTime = 216000;
        }

        Color IDedicatedItem.DedicatedItemColor() => DedicatedColors.niker;
    }
}