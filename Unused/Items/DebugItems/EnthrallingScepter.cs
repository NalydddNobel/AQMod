using Aequus.Items;
using Aequus.Items.Weapons.Necromancy.Scepters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.DebugItems {
    public class EnthrallingScepter : ScepterBase {
        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DebugFeatures;
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
            Item.staff[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(2, 1f, 96);
            Item.shootSpeed = 20f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}