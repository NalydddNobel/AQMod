using Aequus.Items;
using Aequus.Items.Consumables.Coatings;
using Terraria;

namespace Aequus.Content.CarpenterBounties
{
    public class ThickHouseBounty : PirateShipBounty
    {
        public override bool IsBountyAvailable()
        {
            return false;
        }

        public override Item ProvideBountyRewardItem()
        {
            var reward = AequusItem.SetDefaults<ImpenetrableCoating>();
            reward.stack = 250;
            return reward;
        }
    }
}