using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class SiphonTentacle : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(25);
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = ItemRarities.GaleStreams;
            Item.value = Item.sellPrice(silver: 15);
        }
    }
}