using Aequus.Items;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.Crabson.Rewards {
    [AutoloadEquip(EquipType.Head)]
    public class CrabsonMask : ModItem {
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