using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Fish
{
    public class Depthscale : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 999;
        }
    }
}