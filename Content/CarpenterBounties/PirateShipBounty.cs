using Aequus.Items;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.CarpenterBounties
{
    public class PirateShipBounty : CarpenterBounty
    {
        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults(ItemID.PirateHat);
        }
    }
}