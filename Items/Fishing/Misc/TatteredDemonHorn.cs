using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Fishing.Misc
{
    public class TatteredDemonHorn : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 16;
        }
    }
}
