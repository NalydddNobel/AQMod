using Aequus.Common.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Vanity.Masks {
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