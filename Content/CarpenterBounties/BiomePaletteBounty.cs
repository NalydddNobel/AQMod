using Aequus.Common;
using Aequus.Items;
using Terraria.ID;
using Terraria;

namespace Aequus.Content.CarpenterBounties
{
    public class BiomePaletteBounty : CarpenterBounty
    {
        public override bool CheckConditions(TileMapCache map, out string message, NPC carpenter = null)
        {
            message = "h";
            return false;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults(ItemID.Escargot);
        }
    }
}
