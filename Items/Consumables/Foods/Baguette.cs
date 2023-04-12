using Aequus.Buffs;
using Aequus.Items.GlobalItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class Baguette : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;

            AequusItem.Dedicated[Type] = new(new Color(187, 142, 42, 255));

            this.StaticDefaultsToFood(new Color(194, 136, 36, 255), new Color(147, 103, 27, 255), new Color(100, 49, 2, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, ModContent.BuffType<BaguetteBuff>(), 216000);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(silver: 80);
        }
    }
}