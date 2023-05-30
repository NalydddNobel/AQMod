using Aequus.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods {
    public class SpicyEel : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            this.StaticDefaultsToFood(new Color(255, 123, 91, 255), new Color(219, 57, 57, 255), new Color(107, 36, 36, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, ModContent.BuffType<SpicyEelBuff>(), 36000);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 50);
        }
    }
}