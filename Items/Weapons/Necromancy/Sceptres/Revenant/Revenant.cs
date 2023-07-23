using Aequus.Common;
using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Revenant {
    [AutoloadGlowMask]
    [WorkInProgress]
    public class Revenant : SceptreBase {
        public override Color GlowColor => Color.Blue;
        public override int DustSpawn => ModContent.DustType<RevenantParticle>();

        public override void SetStaticDefaults() {
#if DEBUG
            ChestLootDataset.AequusDungeonChestLoot.Add(Type);
#endif
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.DefaultToNecromancy(10);
            Item.SetWeaponValues(10, 1f, 0);
            Item.shootSpeed = 2.5f;
            Item.shoot = ModContent.ProjectileType<RevenantSceptreProj>();
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.ValueDungeon;
            Item.mana = 6;
            Item.autoReuse = true;
        }
    }
}