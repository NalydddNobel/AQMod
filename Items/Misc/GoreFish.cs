using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class GoreFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(2);
            AequusItem.LegendaryFish.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Batfish);
            Item.stack = 999;
        }
    }
}