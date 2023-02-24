using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Fishing
{
    public class IcebergFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FrostMinnow);
        }
    }
}