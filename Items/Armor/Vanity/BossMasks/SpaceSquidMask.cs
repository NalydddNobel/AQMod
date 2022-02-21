using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Vanity.BossMasks
{
    [AutoloadEquip(EquipType.Head)]
    public class SpaceSquidMask : ModItem
    {
        public override void SetDefaults()
        {
            int oldHead = item.headSlot;
            item.CloneDefaults(ItemID.SkeletronMask);
            item.headSlot = oldHead;
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = true;
        }
    }
}