using Terraria.ModLoader;

namespace Aequus.Items.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class BreadOfCthulhuMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemDefaults.RarityBossMasks;
            Item.vanity = true;
        }
    }
}