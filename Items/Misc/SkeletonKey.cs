using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class SkeletonKey : ModItem, IUpdateBank
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.rare++;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.hasSkeletonKey = true;
        }
    }
}