using Aequus.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class SpicyEel : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            this.StaticDefaultsToFood(new Color(255, 123, 91, 255), new Color(219, 57, 57, 255), new Color(107, 36, 36, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, ModContent.BuffType<SpicyEelBuff>(), 36000);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 50);
        }
    }

    public class SpicyEelWingStatModifier : GlobalItem
    {
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            if (!player.Aequus().buffSpicyEel)
            {
                return;
            }
            speed += 1f;
            speed *= 1.1f;
            acceleration *= 1.1f;
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            if (!player.Aequus().buffSpicyEel)
            {
                return;
            }
            ascentWhenFalling *= 1.1f;
            ascentWhenRising *= 1.1f;
            maxCanAscendMultiplier *= 1.1f;
            maxAscentMultiplier *= 1.1f;
            constantAscend *= 1.1f;
        }
    }
}