using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.BossMasks
{
    [AutoloadEquip(EquipType.Head)]
    public class CrabsonMask : ModItem
    {
        public override void SetDefaults()
        {
            int oldHead = item.headSlot;
            item.CloneDefaults(ItemID.SkeletronMask);
            item.headSlot = oldHead;
        }
    }
}