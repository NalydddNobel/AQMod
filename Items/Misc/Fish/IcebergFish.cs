using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Fish
{
    public class IcebergFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(3);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FrostMinnow);
        }
    }
}