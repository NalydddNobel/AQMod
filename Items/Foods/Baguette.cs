using AQMod.Content.DedicatedItemTags;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods
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
            item.rare = AQItem.Rarities.DedicatedItem;
            item.value = Item.buyPrice(silver: 80);
            item.buffType = BuffID.WellFed;
            item.buffTime = 216000;
        }

        public override bool CanBurnInLava()
        {
            return true;
        }

        Color IDedicatedItem.DedicatedItemColor => new Color(187, 142, 42, 255);
        IDedicationType IDedicatedItem.DedicationType => new BasicDedication();
    }
}