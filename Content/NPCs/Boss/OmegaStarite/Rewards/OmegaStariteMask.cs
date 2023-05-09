using Aequus.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.NPCs.Boss.OmegaStarite.Rewards {
    [AutoloadEquip(EquipType.Head)]
    public class OmegaStariteMask : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemDefaults.RarityBossMasks;
            Item.vanity = true;
        }
    }
}