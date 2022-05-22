using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Hexoplasm : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = MaterialSort.Ectoplasm;
            this.SetResearch(25);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Ectoplasm);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }
    }
}