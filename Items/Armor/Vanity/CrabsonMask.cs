using Terraria.ModLoader;

namespace Aequus.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CrabsonMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemDefaults.RarityBossMasks;
        }
    }
}