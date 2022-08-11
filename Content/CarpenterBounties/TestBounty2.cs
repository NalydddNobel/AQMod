using Aequus.Common;
using Aequus.Items;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.CarpenterBounties
{
    public class TestBounty2 : CarpenterBounty
    {
        public override bool CheckConditions(TileMapCache map, out string message, NPC carpenter = null)
        {
            message = "";
            return true;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults(ItemID.TrifoldMap);
        }
    }
}