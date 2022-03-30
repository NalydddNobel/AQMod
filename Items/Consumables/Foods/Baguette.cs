using Aequus.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class Baguette : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsFood[Type] = true;
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