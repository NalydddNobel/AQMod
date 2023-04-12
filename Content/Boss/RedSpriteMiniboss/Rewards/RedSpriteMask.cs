using Aequus.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.RedSpriteMiniboss.Rewards
{
    [AutoloadEquip(EquipType.Head)]
    public class RedSpriteMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemDefaults.RarityBossMasks;
            Item.vanity = true;
            Item.Aequus().itemGravityCheck = 255;
        }
    }
}