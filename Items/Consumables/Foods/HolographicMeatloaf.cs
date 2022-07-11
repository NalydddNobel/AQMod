using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class HolographicMeatloaf : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            this.StaticDefaultsToFood(Color.Brown, Color.RosyBrown, Color.Red);
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, BuffID.WellFed3, 14400);
            Item.maxStack = 1;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(silver: 80);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * 0.75f;
        }
    }
}