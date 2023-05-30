using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods {
    public class SuspiciousLookingSteak : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            this.StaticDefaultsToFood(Color.Red, Color.DarkRed);
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Red, };
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Steak);
            Item.buffTime = 18000;
        }
    }
}