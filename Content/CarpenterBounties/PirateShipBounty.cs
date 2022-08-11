using Aequus.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.CarpenterBounties
{
    public class PirateShipBounty : CarpenterBounty
    {
        public override bool CheckConditions(Rectangle rect, out string message, NPC carpenter = null)
        {
            message = "";
            return true;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults(ItemID.PirateHat);
        }
    }
}