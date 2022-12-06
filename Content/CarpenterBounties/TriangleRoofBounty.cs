using Aequus.Items;
using Aequus.Items.Tools.Misc;
using Terraria;

namespace Aequus.Content.CarpenterBounties
{
    public class TriangleRoofBounty : PirateShipBounty
    {
        public override bool IsBountyAvailable()
        {
            return false;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults<WhiteFlag>(); // Line Block Tool
        }
    }
}