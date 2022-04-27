using Aequus.Buffs;
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
            this.SetResearch(1);
            ItemID.Sets.IsFood[Type] = true;

            ItemTooltips.Catalogue.Dedicated[Type] = new ItemTooltips.Catalogue.ItemDedication(new Color(187, 142, 42, 255));

            Main.RegisterItemAnimation(Type, new FoodFramingHack());
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, ModContent.BuffType<BaguetteBuff>(), 216000);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(silver: 80);
        }
    }
}