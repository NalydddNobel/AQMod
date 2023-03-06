using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class CinnamonRoll : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            this.StaticDefaultsToFood(new Color(194, 136, 36, 255), new Color(147, 103, 27, 255), new Color(100, 49, 2, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, BuffID.WellFed3, 36000);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 50);
        }
    }
}