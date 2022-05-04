using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusItem : GlobalItem
    {
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().permMoro && ItemsCatalogue.SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (player.GetModPlayer<AequusPlayer>().permMoro && ItemsCatalogue.SummonStaff.Contains(item.type))
            {
                return 2f;
            }
            return 1f;
        }
    }
}