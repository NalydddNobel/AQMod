using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Fish.Legendary
{
    public class GoreFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 2;
            AequusItem.LegendaryFish.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Batfish);
            Item.maxStack = 9999;
        }
    }
}