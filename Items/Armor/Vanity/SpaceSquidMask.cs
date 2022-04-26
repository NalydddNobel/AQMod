using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class SpaceSquidMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemRarityConstants.BossMasks;
        }
    }
}