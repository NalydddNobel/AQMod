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

            this.StaticDefaultsToFood(Color.Brown.UseA(0) * 0.75f, Color.RosyBrown.UseA(0) * 0.75f, Color.Red.UseA(0) * 0.75f);
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
            return lightColor.MaxRGBA(128).UseA(150) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0.66f, 1f);
        }
    }
}