using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class CrabCasing : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = MaterialSort.PlatinumBar;
            this.SetResearch(25);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 3);
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;
        }
    }
}