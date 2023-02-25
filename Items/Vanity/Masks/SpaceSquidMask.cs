using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class SpaceSquidMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemDefaults.RarityBossMasks;
            Item.vanity = true;
        }
    }
}